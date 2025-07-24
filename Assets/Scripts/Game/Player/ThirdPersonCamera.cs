using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour
{
    private static Camera _camera;
    
    public Transform target;
    public Vector2 sensitivity = new Vector2(100f, 100f);
    public float distance = 5f;
    public float minY = -20f, maxY = 60f;

    private float yaw, pitch;

    private void Awake() => _camera = GetComponent<Camera>();
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity.x * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity.y * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = target.position + rotation * direction;
        transform.LookAt(target);
    }
}