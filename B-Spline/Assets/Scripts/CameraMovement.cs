using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float lookSpeedH = 5f;
    [SerializeField] private float lookSpeedV = 5f;

    private float _yaw;
    private float _pitch;

    private void Awake()
    {
        var currentAngle = transform.eulerAngles;
        _pitch = currentAngle.x;
        _yaw = currentAngle.y;
    }

    private void Update()
    {
        MoveIfNeeded();
        ChangeLookDirectionIfNeeded();
    }

    private void MoveIfNeeded()
    {
        if (Input.GetKey(KeyCode.W)) transform.position += transform.forward * (movementSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) transform.position -= transform.right * (movementSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S)) transform.position -= transform.forward * (movementSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) transform.position += transform.right * (movementSpeed * Time.deltaTime);
    }

    private void ChangeLookDirectionIfNeeded()
    {
        if (!Input.GetMouseButton(1)) return;

        _yaw += lookSpeedH * Input.GetAxis("Mouse X");
        _pitch -= lookSpeedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
    }
}
