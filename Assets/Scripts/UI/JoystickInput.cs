using Assets.Scripts.Game.Player;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class JoystickInput : MonoBehaviour, IMovementInput, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform _joystickBackground;
    [SerializeField] private RectTransform _joystickHandle;
    [SerializeField] private float _maxOffset = 50f;
    
    private Vector2 _inputVector;
    private bool _isActive;

    public Vector2 GetMovementDirection() => _isActive ? _inputVector : Vector2.zero;

    public void OnPointerDown(PointerEventData eventData)
    {
        _joystickBackground.position = eventData.position;
        _isActive = true;
        _joystickHandle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _inputVector = Vector2.zero;
        _isActive = false;
        _joystickHandle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - (Vector2)_joystickBackground.position;
        _inputVector = Vector2.ClampMagnitude(direction, _maxOffset) / _maxOffset;
        _joystickHandle.anchoredPosition = _inputVector * _maxOffset;
    }
}