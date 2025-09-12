using NUnit.Framework.Constraints;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Vector2 CameraRotation;
    Vector3 CameraOffset;
    public Vector3 RespawnPoint;
    InputAction LookVector;
    Camera PlayerCamera;

    Rigidbody rb;
    float verticalMove;
    float horizontalMove;

    public float XSensitivity = 1.0f;
    public float YSensitivity = 1.0f;
    public int Health = 3;
    public int MaxHealth = 3;
    public float speed = 15f;
    public float jumpHeight = 10f;
    public float CameraRotationLimit = 90.0f;

    public void Start()
    {
        CameraOffset = new Vector3(0, .3f, .35f);
        rb = GetComponent<Rigidbody>();
        PlayerCamera = Camera.main;
        LookVector = GetComponent<PlayerInput>().currentActionMap.FindAction("Look");
        CameraRotation = Vector2.zero;
        RespawnPoint = new Vector3(0, 2, 0);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        if (Health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Movement System
        Vector3 temp = rb.linearVelocity;
        temp.x = verticalMove * speed;
        temp.z = horizontalMove * speed;
        rb.linearVelocity = (temp.x * transform.forward) + (temp.y * transform.up) + (temp.z * transform.right);

        // Camera System
        PlayerCamera.transform.position = (transform.position + CameraOffset);

        CameraRotation.x += LookVector.ReadValue<Vector2>().x * XSensitivity;
        CameraRotation.y += LookVector.ReadValue<Vector2>().y * YSensitivity;
        CameraRotation.y = Mathf.Clamp(CameraRotation.y, -CameraRotationLimit, CameraRotationLimit);

        PlayerCamera.transform.rotation = Quaternion.Euler(-CameraRotation.y, CameraRotation.x, 0);
        transform.localRotation = Quaternion.AngleAxis(CameraRotation.x, Vector3.up);
       

    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 InputAxis = context.ReadValue<Vector2>();

        verticalMove = InputAxis.y;
        horizontalMove = InputAxis.x;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DeathZone")
        {
            Health = 0;
        }

        if (other.tag == "DamagePart")
        {
            Destroy(other);
            other.transform.position = new Vector3(0, -20, 0);
            Health -= 1;
        }
    }

}
