using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerConveyor : MonoBehaviour, ITriggerEnterHandler, ITriggerStayHandler, ITriggerExitHandler
{
    [Header("Настройки конвейера")]
    [SerializeField] private float _pushSpeed = 3f;
    [SerializeField] private Vector3 _pushDirection = Vector3.forward;
    [SerializeField] private bool _isActive = true;

    private CharacterController _playerController;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 start = transform.position;
        Vector3 end = start + transform.TransformDirection(_pushDirection.normalized) * 2f;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.1f);
    }

    public void HandleTriggerEnter(Collider other)
    {
        _playerController = other.GetComponent<CharacterController>();
    }

    public void HandleTriggerStay(Collider other)
    {
        if (!_isActive || _playerController == null) return;
        Vector3 direction = transform.TransformDirection(_pushDirection).normalized;
        _playerController.Move(direction * _pushSpeed * Time.deltaTime);
    }

    public void HandleTriggerExit(Collider other)
    {
        _playerController = null;
    }
}