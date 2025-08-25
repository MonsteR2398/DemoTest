using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemCanvasSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxDisplayedTexts = 20;
    [SerializeField] private float _maxDisplayDistance = 10f;
    [SerializeField] private float _maxBuyDisplayDistance = 3f;
    [SerializeField] private Vector3 _buyDisplayOffset = new Vector3(0, 1f, 0);
    [SerializeField] private Vector3 _textOffset = new Vector3(0, 1f, 0);
    [SerializeField] private float _updateInterval = 1f;

    [Header("References")]
    [SerializeField] private TextDisplay _textDisplayPrefab;
    [SerializeField] private BuyDisplay _buyDisplayPrefab;
    [SerializeField] private BuyDisplay _equipDisplayPrefab;
    [SerializeField] private Transform _playerTransform;
    
    private Canvas _itemCanvas;
    [SerializeField] private Canvas _buyDisplayCanvas;
    private Camera _worldCamera;
    private BuyDisplay _activeBuyDisplay;
    private static BuyDisplay _activeEquipDisplay;
    private IBuyable _currentBuyable;
    
    private List<TextData> _activeTexts = new();
    private readonly HashSet<IObjectTextDisplay> _allDisplays = new();
    private readonly List<IObjectTextDisplay> _nearestDisplays = new();

    public static ItemCanvasSystem Instance;

    private void Awake()
    {
        if (_itemCanvas == null)
            _itemCanvas = GetComponent<Canvas>();
        if (_itemCanvas != null)
            _worldCamera = _itemCanvas.worldCamera;


        if (_buyDisplayPrefab != null && _buyDisplayCanvas != null)
        {
            _activeBuyDisplay = Instantiate(_buyDisplayPrefab, _buyDisplayCanvas.transform);
            _activeBuyDisplay.Initialize(HandleBuy, _buyDisplayOffset);
            _activeBuyDisplay.SetActive(false);
        }
        if (_equipDisplayPrefab != null && _buyDisplayCanvas != null)
        {
            _activeEquipDisplay = Instantiate(_equipDisplayPrefab, _buyDisplayCanvas.transform);
        }

        Instance = this;
    }

    private void OnEnable()
    {
        TextDisplayEvents.OnDisplayEnabled += RegisterDisplay;
        TextDisplayEvents.OnDisplayDisabled += UnregisterDisplay;
    }

    private void OnDisable()
    {
        TextDisplayEvents.OnDisplayEnabled -= RegisterDisplay;
        TextDisplayEvents.OnDisplayDisabled -= UnregisterDisplay;
    }

    private void LateUpdate()
    {
        UpdateNearestDisplays();
        UpdateTextDisplay();
        UpdateBuyDisplay();
        OrientTextsToCamera();
    }
    public BuyDisplay GetEquipDisplay() => _activeEquipDisplay;

    private void RegisterDisplay(IObjectTextDisplay display)
    {
        _allDisplays.Add(display);
    }

    private void UnregisterDisplay(IObjectTextDisplay display)
    {
        _allDisplays.RemoveWhere(d => d == display);
    }

    private void UpdateNearestDisplays()
    {
        _nearestDisplays.Clear();

        foreach (var display in _allDisplays)
            if (display.IsAlwaysVisible() && display is MonoBehaviour)
                _nearestDisplays.Add(display);

        foreach (var display in _allDisplays)
        {
            if (display.IsAlwaysVisible()) continue;
            if (display is not MonoBehaviour mono) continue;

            var distance = Vector3.Distance(_playerTransform.position, mono.transform.position);
            if (distance <= _maxDisplayDistance)
                _nearestDisplays.Add(display);
        }

        if (_nearestDisplays.Count > _maxDisplayedTexts)
        {
            _nearestDisplays.Sort((a, b) =>
            {
                bool aAlways = a.IsAlwaysVisible();
                bool bAlways = b.IsAlwaysVisible();

                if (aAlways && !bAlways) return -1;
                if (!aAlways && bAlways) return 1;

                var aMono = (MonoBehaviour)a;
                var bMono = (MonoBehaviour)b;

                return Vector3.Distance(_playerTransform.position, aMono.transform.position)
                    .CompareTo(Vector3.Distance(_playerTransform.position, bMono.transform.position));
            });

            int alwaysVisibleCount = _nearestDisplays.Count(d => d.IsAlwaysVisible());
            int removableCount = _nearestDisplays.Count - alwaysVisibleCount;

            if (removableCount > _maxDisplayedTexts)
            {
                int removeFromIndex = alwaysVisibleCount + _maxDisplayedTexts;
                _nearestDisplays.RemoveRange(removeFromIndex, removableCount - _maxDisplayedTexts);
            }
        }
    }

    private void UpdateTextDisplay()
    {
        foreach (var textData in _activeTexts)
            textData.TextDisplay.SetActive(false);
            

        for (int i = 0; i < _nearestDisplays.Count; i++)
        {
            TextData displayData;

            if (i >= _activeTexts.Count)
            {
                var newDisplay = Instantiate(_textDisplayPrefab, _itemCanvas.transform);
                displayData = new TextData { TextDisplay = newDisplay };
                _activeTexts.Add(displayData);
            }
            else
            {
                displayData = _activeTexts[i];
            }

            var display = _nearestDisplays[i];

            displayData.TextDisplay.SetActive(true);
            displayData.TextDisplay.Text = display.GetTextOnDisplay();

            if (display is MonoBehaviour monoBehaviour)
            {
                //Debug.Log(monoBehaviour.transform.position + "  |  " + display.DisplayOffset);
                displayData.TextDisplay.SetPosition(monoBehaviour.gameObject.transform.position, display.DisplayOffset);
            }

            displayData.Display = display;
        }
    }

    private void UpdateBuyDisplay()
    {
        if (_activeBuyDisplay == null) return;
    
        if (TryGetNearestBuyable(out IBuyable nearestBuyable, out Transform nearestTransform))
        {
            // Теперь nearestTransform всегда соответствует nearestBuyable
            SetupDisplay(_activeBuyDisplay, nearestBuyable, nearestTransform);
            _currentBuyable = nearestBuyable;
        }
        else
        {
            _activeBuyDisplay.SetActive(false);
            _currentBuyable = null;
        }
    }

    private void SetDisplayText(BuyDisplay display, IBuyable buyable)
    {
        if (buyable is not Item item) return;

        if (item.NowEnemy)
        {
            display.SetText("Украсть");
            return;
        }

        if (item is ISpawned spawned && !spawned.HasSpawned)
        {
            display.SetText($"Купить ({buyable.GetPrice()})");
            return;
        }

        if (item is ITimer timerObj && timerObj.HasActiveTimer())
            display.SetText($"Пропустить оживление? ({buyable.GetPrice()})");
        else
            display.SetText($"Вылупить ({buyable.GetPrice()})");
    }

public bool TryGetNearestBuyable(out IBuyable nearestBuyable, out Transform nearestTransform)
{
    nearestBuyable = null;
    nearestTransform = null;
    float minDistance = float.MaxValue;
    
    foreach (var display in _nearestDisplays)
    {
        if (display is MonoBehaviour mono)
        {
            float distance = Vector3.Distance(_playerTransform.position, mono.transform.position);
            
            // Ищем только IBuyable объекты
            if (display is IBuyable buyable)
            {
                if (distance < minDistance && distance <= _maxBuyDisplayDistance)
                {
                    nearestBuyable = buyable;
                    nearestTransform = mono.transform; // Гарантируем соответствие!
                    minDistance = distance;
                }
            }
        }
    }

    return nearestBuyable != null;
}

    private void SetupDisplay(BuyDisplay display, IBuyable buyable, Transform targetTransform)
    {
        if (display != null && targetTransform != null)
        {
            display.SetTarget(targetTransform);
            display.SetActive(true);
        }
        else if (display != null)
        {
            display.SetActive(false);
        }

        if (display != null && buyable != null)
        {
            display.SetCanBuy(buyable.CanBuy());
            SetDisplayText(display, buyable);
        }
    }

    private void HandleBuy()
    {
        if (_currentBuyable == null || !_currentBuyable.CanBuy()) return;
        _currentBuyable.Buy();
        //_currentBuyable.SetCanBuy(false);
        _activeBuyDisplay.SetActive(false);
        if (_currentBuyable is Egg egg)
        {
            bool hasSpawned = egg.Data.HasSpawned;
            UnregisterDisplay(egg);
            if (!hasSpawned)
                InventorySystem.Instance.AddItem(egg, 1);
            egg.ReturnToPool();
        }
    }

    private void OrientTextsToCamera()
    {
        if (!_worldCamera) return;

        var targetRotation = _worldCamera.transform.rotation;
        foreach (var text in _activeTexts)
        {
            if (!text.TextDisplay.gameObject.activeSelf ||
                text.Display == null ||
                !text.Display.ShouldOrientToCamera())
                continue;

            text.TextDisplay.transform.rotation = targetRotation;
        }
    }

    private class TextData
    {
        public TextDisplay TextDisplay;
        public IObjectTextDisplay Display;
    }
}
