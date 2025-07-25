using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class WorldTextSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxDisplayedTexts = 5;
    [SerializeField] private float _maxDisplayDistance = 10f;
    [SerializeField] private Vector3 _textOffset = new Vector3(0, 1f, 0);
    
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _textPrefab;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Canvas _worldCanvas;
    private Camera _worldCamera;
    

    private List<TextData> _activeTexts = new();

    private readonly HashSet<IObjectTextDisplay> _allDisplays = new();
    private readonly List<IObjectTextDisplay> _nearestDisplays = new();
    


    private void Awake()
    {
        if (_worldCanvas != null)
            _worldCamera = _worldCanvas.worldCamera;
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
        OrientTextsToCamera();
    }

    private void RegisterDisplay(IObjectTextDisplay display)
    {
        _allDisplays.Add(display);
    }

    private void UnregisterDisplay(IObjectTextDisplay display)
    {
        _allDisplays.Remove(display);
    }

    private void UpdateNearestDisplays()
    {
        _nearestDisplays.Clear();
        foreach (var display in _allDisplays)
        {
            if (display is not MonoBehaviour mono)
                continue;

            var distance = Vector3.Distance(
                _playerTransform.position,
                mono.transform.position
            );

            if (distance <= _maxDisplayDistance)
                _nearestDisplays.Add(display);
        }

        if (_nearestDisplays.Count <= _maxDisplayedTexts) return;

        _nearestDisplays.Sort((a, b) =>
            Vector3.Distance(_playerTransform.position, ((MonoBehaviour)a).transform.position)
                .CompareTo(Vector3.Distance(_playerTransform.position, ((MonoBehaviour)b).transform.position)));

        if (_nearestDisplays.Count > _maxDisplayedTexts)
            _nearestDisplays.RemoveRange(_maxDisplayedTexts, _nearestDisplays.Count - _maxDisplayedTexts);

    }

    private void SetInitializeOnTexts()
    {
        foreach (var text in _activeTexts)
            text.IsInitialized = false;
    }

    private void UpdateTextDisplay() 
    {
        while (_activeTexts.Count < _nearestDisplays.Count) 
        {
            var newText = Instantiate(_textPrefab, _worldCanvas.transform);
            _activeTexts.Add(new TextData 
            {
                TextComponent = newText,
                IsInitialized = false,
                Display = _nearestDisplays[_activeTexts.Count]
            });
        }
        for (int i = 0; i < _nearestDisplays.Count; i++) 
        {
            var textData = _activeTexts[i];
            var display = _nearestDisplays[i];

            if (!textData.TextComponent.gameObject.activeSelf) 
                textData.TextComponent.gameObject.SetActive(true);
            
            if (textData.Display != display || display.HasUpdatePerFrame())
                textData.IsInitialized = false;

            if (!textData.IsInitialized) 
            {
                textData.Display = display;
                textData.TextComponent.text = display.GetTextOnDisplay();
                textData.IsInitialized = true;
            }

            if (display is MonoBehaviour mono) 
                textData.TextComponent.transform.position = mono.transform.position + _textOffset;
        }

        for (int i = _nearestDisplays.Count; i < _activeTexts.Count; i++) 
            _activeTexts[i].TextComponent.gameObject.SetActive(false);
    }

    private void OrientTextsToCamera()
    {
        if (!_worldCamera) return;
    
        var targetRotation = _worldCamera.transform.rotation;
        foreach (var text in _activeTexts)
        {
            if (!text.TextComponent.gameObject.activeSelf) continue;
            text.TextComponent.transform.rotation = targetRotation; 
        }
    }
    
    private class TextData 
    {
        public TextMeshProUGUI TextComponent;
        public bool IsInitialized;
        public IObjectTextDisplay Display;
    }
}



