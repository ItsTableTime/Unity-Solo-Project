using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    float verticalMove;
    float horizontalMove;
    public float speed = 5f;
    public float jumpHeight = 10f;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        Vector3 temp = rb.linearVelocity;
        temp.x = verticalMove * speed;
        temp.z = horizontalMove * speed;
        rb.linearVelocity = (temp.x * transform.forward) + (temp.y * transform.up) + (temp.z * transform.right);

    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 InputAxis = context.ReadValue<Vector2>();

        verticalMove = InputAxis.y;
        horizontalMove = InputAxis.x;

    }
}
