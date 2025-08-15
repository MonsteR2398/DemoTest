using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    public Image ItemIcon;
    public Image BackgroundIcon;
    public float ItemSize { get; private set; }
    public TextMeshProUGUI CountText;
    [SerializeField] private LayerMask groundLayer;

    private Item _item;
    private int _count;
    private Item _phantomItem;
    private Camera _mainCamera;
    private Renderer[] _phantomRenderers;
    private Color[] _originalColors;
    private bool _canPlace;
    int exceptMask;

    private void Awake()
    {
        _mainCamera = Camera.main;
        exceptMask = CreateMaskExcept("Player");
    }

    public void Initialize(Item item, int count)
    {
        _item = item;
        _count = count;
        var variant = item.GetVariant();
        if (variant != Variant.Default)
        {
            BackgroundIcon.gameObject.SetActive(true);
            BackgroundIcon.color = item.GetColorWithVariant(variant);
        }
        else
        {
            BackgroundIcon.gameObject.SetActive(false);
        }
        ItemIcon.sprite = item.GetIcon();
        if (item is Brainrot rot)
            ItemSize = rot.GetSize();
        UpdateUI();
    }

    public Item GetItem() => _item;

    public void AddCount(int amount)
    {
        _count += amount;
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        ItemIcon.enabled = _count > 0;
        switch (_item)
        {
            case Egg egg:
                CountText.text = $"x{_count}";
                break;
            case Brainrot rot:
                CountText.text = $"${rot.Data.Income}/s"; 
                break;
            default:
                CountText.enabled = false;
                break;
        }

        // if (_item is Brainrot rot)
        //     CountText.text = $"${rot.Data.Income}/s";
        // else if (_item is Egg egg)
        //     CountText.text = _count.ToString();
        // else
        //     CountText.enabled = false;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if (_count <= 0 || _phantomItem != null || _item == null) return;
        if ((_item is Hammer))
        {
            if (InventorySystem.Instance.itemInHand == null)
            {
                _item.transform.gameObject.SetActive(true);
                InventorySystem.Instance.SetItemInHand(_item);
            }
            else
            {
                _item.transform.gameObject.SetActive(false);
                InventorySystem.Instance.SetItemInHand(null);
            }
            return;
        }

        _phantomItem = Instantiate(_item, new Vector3(-0, -0, -0), Quaternion.identity);
        _phantomItem.transform.gameObject.SetActive(true);
        
        //заменить
        if (_phantomItem is Brainrot brainrot)
            brainrot.SetSize(ItemSize);
        if (_phantomItem is Egg egg)
        { 
            if(_item is Egg egg1)
                egg.Data.SetUniqueId(egg1.Data.UniqueId);
        }
        
        _phantomRenderers = _phantomItem.GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_phantomRenderers.Length];

        for (int i = 0; i < _phantomRenderers.Length; i++)
            if (_phantomRenderers[i].material != null)
                _originalColors[i] = _phantomRenderers[i].material.color;

        foreach (var col in _phantomItem.GetComponentsInChildren<Collider>())
            col.enabled = false;

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_phantomItem == null) return;
        if ((_item is Hammer)) return;


        Ray ray = _mainCamera.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, exceptMask))
        {
            _phantomItem.transform.position = hit.point;
            if (((1 << hit.collider.gameObject.layer) & groundLayer) != 0)
            {
                _canPlace = true;
                SetPhantomMaterial(true, true);
            }
            else
            {
                _canPlace = false;
                SetPhantomMaterial(false, true);
            }
        }
        else
        {
            _phantomItem.transform.position = ray.GetPoint(15);
            _canPlace = false;
            SetPhantomMaterial(false, true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_phantomItem == null) return;
        if ((_item is Hammer)) return;

        if (_canPlace)
        {
            SetPhantomMaterial(false, false);
            _count--;
            UpdateUI();
            _phantomItem.OnSpawnToGround();
            // убрать дубляж
            if (_phantomItem is Egg egg)
            {
                LoadInitializer.Instance.SaveLoadManager.RemoveEggByUniqueId(egg.Data.UniqueId);
                egg.Data.HasSpawned = true;
                LoadInitializer.Instance.SaveLoadManager.AddEggData(egg.Data);
                LoadInitializer.Instance.SaveLoadManager.SaveEggsData();
            }
            else if (_phantomItem is Brainrot brainrot)
            {
                LoadInitializer.Instance.SaveLoadManager.RemoveBrainrotByUniqueId(brainrot.Data.UniqueId);
                _phantomItem.OnSpawnToGround();
                LoadInitializer.Instance.SaveLoadManager.AddBrainrotData(brainrot.Data);
                LoadInitializer.Instance.SaveLoadManager.SaveBrainrotsData();
            }
            
            if (_phantomItem is ITimer timerItem)
                timerItem.ActivateTimer();
            if (_count <= 0)
            {
                InventorySystem.Instance.RemoveSlot(_item.GetItemID());
                Destroy(_item.gameObject);
                Destroy(gameObject);
            }
            foreach (var col in _phantomItem.GetComponentsInChildren<Collider>())
                col.enabled = true;
            if (_phantomItem.TryGetComponent(out BaseInteractionZone interZone))
            {
                for (int i = 0; i < _phantomRenderers.Length; i++)
                    if (_phantomRenderers[i].material != null)
                        interZone.OriginalColors[i] = _originalColors[i];
                        
            }
            _phantomItem = null;
        }
        else
        {
            Destroy(_phantomItem.gameObject);
        }
        
        if(_phantomItem != null)
             _phantomItem = null;

        _phantomRenderers = null;
        _originalColors = null;
    }

    int CreateMaskExcept(params string[] excludedLayers)
    {
       int mask = 0;

       // Добавляем все слои (0–31), кроме исключённых
       for (int i = 0; i < 32; i++)
       {
           bool exclude = false;
           foreach (var layerName in excludedLayers)
           {
               if (i == LayerMask.NameToLayer(layerName))
               {
                   exclude = true;
                   break;
               }
           }

           if (!exclude)
               mask |= 1 << i;
       }

       return mask;
    }
    private void SetPhantomMaterial(bool canPlace, bool isTransparent)
    {
        if (_phantomRenderers == null) return;

        Color color = canPlace ? Color.green : Color.red;
        if (isTransparent)
            color.a = 0.5f;

        foreach (var renderer in _phantomRenderers)
            if (renderer.material != null)
                renderer.material.color = color;

        if (isTransparent) return;
        for (int i = 0; i < _phantomRenderers.Length; i++)
            if (_phantomRenderers[i].material != null)
                _phantomRenderers[i].material.color = _originalColors[i];
    }
}
