using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    public Image ItemIcon;
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

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void Initialize(Item item, int count)
    {
        _item = item;
        _count = count;
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
        if (_count > 1)
        {
            CountText.text = _count.ToString();
            CountText.enabled = true;
        }
        else
        {
            CountText.enabled = false;
        }
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
        
        if (_phantomItem is Brainrot brainrot)
            brainrot.SetSize(ItemSize);
        
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
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
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

    private void SetPhantomMaterial(bool canPlace, bool isTransparent)
    {
        if (_phantomRenderers == null) return;

        Color color = canPlace ? Color.green : Color.red;
        if (isTransparent)
            color.a = 0.5f;

        foreach (var renderer in _phantomRenderers)
            if(renderer.material != null)
                renderer.material.color = color;

        if (isTransparent) return;
        for (int i = 0; i < _phantomRenderers.Length; i++)
            if (_phantomRenderers[i].material != null)
                _phantomRenderers[i].material.color = _originalColors[i];
    }
}
