using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDragZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Camera _camera;
    
    [Header("Settings")]
    [SerializeField] private float _rotationSpeed = 0.5f;
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0, 2f, -5f); // Камера выше и сзади
    
    private bool _isDragging;
    private Vector2 _lastTouchPos;
    private float _currentYRotation = 0f; 

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(0, _currentYRotation, 0);
        Vector3 desiredPosition = _player.position + rotation * _cameraOffset;
        
        _camera.transform.position = desiredPosition;
        _camera.transform.LookAt(_player.position + Vector3.up * 1f); 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
        _lastTouchPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        
        Vector2 delta = eventData.position - _lastTouchPos;
        _currentYRotation += delta.x * _rotationSpeed;
        _lastTouchPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
    }
}