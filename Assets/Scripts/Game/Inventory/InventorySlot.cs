using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Collections.Generic;

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
    public Item _phantomItem;
    private Camera _mainCamera;
    private Renderer[] _phantomRenderers;
    private List<Color[]> _originalColors = new();
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
        {
            brainrot.SetSize(ItemSize);
            if (_item is Brainrot brainrot1)
                brainrot.Data.SetUniqueId(brainrot1.Data.UniqueId);

            Debug.Log(brainrot.Data.UniqueId);
        }
        if (_phantomItem is Egg egg)
        { 
            if(_item is Egg egg1)
                egg.Data.SetUniqueId(egg1.Data.UniqueId);
        }
        
    _phantomRenderers = _phantomItem.GetComponentsInChildren<Renderer>();
    _originalColors = new List<Color[]>(); 

    for (int i = 0; i < _phantomRenderers.Length; i++)
    {
        if (_phantomRenderers[i] != null && _phantomRenderers[i].materials != null)
        {
            Color[] rendererColors = new Color[_phantomRenderers[i].materials.Length];
            for (int j = 0; j < _phantomRenderers[i].materials.Length; j++)
            {
                rendererColors[j] = _phantomRenderers[i].materials[j].color;
            }
            _originalColors.Add(rendererColors);
        }
    }

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
                brainrot.NowEnemy = false;
                TextDisplayEvents.RaiseDisplayEnabled(brainrot);
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
                if (interZone.OriginalColors == null)
                    interZone.OriginalColors = new Dictionary<Renderer, Color[]>();
                
                for (int i = 0; i < _phantomRenderers.Length; i++)
                {
                    if (_phantomRenderers[i].material != null)
                    {
                        Color[] colors = new Color[_phantomRenderers[i].materials.Length];
                        for (int j = 0; j < colors.Length; j++)
                            colors[j] = _phantomRenderers[i].materials[j].color;
                        interZone.OriginalColors[_phantomRenderers[i]] = colors;
                    }
                }
                        
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

        for (int i = 0; i < _phantomRenderers.Length; i++)
        {
            if (_phantomRenderers[i] == null) continue;

            Material[] materials = _phantomRenderers[i].materials;
            if (materials == null) continue;

            for (int j = 0; j < materials.Length; j++)
            {
                if (materials[j] == null) continue;

                if (isTransparent)
                {
                    Color color = canPlace ? Color.green : Color.red;
                    color.a = 0.5f;
                    materials[j].color = color;
                }
                else
                {
                    if (i < _originalColors.Count && j < _originalColors[i].Length)
                        materials[j].color = _originalColors[i][j];
                }
            }

            _phantomRenderers[i].materials = materials;
        }
    }
}
