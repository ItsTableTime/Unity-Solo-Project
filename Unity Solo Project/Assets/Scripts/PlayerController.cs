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

    Ray interactRay;
    RaycastHit interactHit;
    GameObject pickupObj;
    public PlayerInput input;
    public Transform weaponSlot;
    public Weapon currentWeapon;
    Ray ray;
    Rigidbody rb;
    float verticalMove;
    float horizontalMove;

    public float XSensitivity = 1.0f;
    public float YSensitivity = 1.0f;
    public int Health = 3;
    public int MaxHealth = 3;
    public float JumpDistance = 10f;
    public float speed = 15f;
    public float reloadmovement = 5f;
    public float basemovement = 15f;
    public float jumpHeight = 10000f;
    public float CameraRotationLimit = 90.0f;
    public float InteractDistance = 1f;

    public bool attacking = false;


    public void Start()
    {
        input = GetComponent<PlayerInput>();
        ray = new Ray(transform.position, transform.forward);
        interactRay = new Ray(transform.position, transform.forward);
        CameraOffset = new Vector3(0, .3f, .35f);
        rb = GetComponent<Rigidbody>();
        PlayerCamera = Camera.main;
        LookVector = GetComponent<PlayerInput>().currentActionMap.FindAction("Look");
        CameraRotation = Vector2.zero;
        RespawnPoint = new Vector3(0, 2, 0);
        weaponSlot = PlayerCamera.transform.GetChild(0);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        // Movement System
        Vector3 temp = rb.linearVelocity;
        temp.x = verticalMove * speed;
        temp.z = horizontalMove * speed;
        ray.origin = transform.position;
        ray.direction = -transform.up;
        rb.linearVelocity = (temp.x * transform.forward) + (temp.y * transform.up) + (temp.z * transform.right);

        interactRay.origin = PlayerCamera.transform.position;
        interactRay.direction = PlayerCamera.transform.forward;

        if (Physics.Raycast(interactRay, out interactHit, InteractDistance))
        {
            if (interactHit.collider.gameObject.tag == "Weapon")
            {
                pickupObj = interactHit.collider.gameObject;
            }
            else
            {
                pickupObj = null;
            }
        }
        if (currentWeapon)
        {
            if (currentWeapon.holdToAttack && attacking == currentWeapon)
            {
                currentWeapon.fire();
            }
            if (currentWeapon.reloading)
            {
                speed = reloadmovement;
            }
            else
            {
                speed = basemovement;
            }
        }
        else
        {
            speed = basemovement;
        }

        // Camera System
        PlayerCamera.transform.position = (transform.position + CameraOffset);

        CameraRotation.x += LookVector.ReadValue<Vector2>().x * XSensitivity;
        CameraRotation.y += LookVector.ReadValue<Vector2>().y * YSensitivity;
        CameraRotation.y = Mathf.Clamp(CameraRotation.y, -CameraRotationLimit, CameraRotationLimit);

        PlayerCamera.transform.rotation = Quaternion.Euler(-CameraRotation.y, CameraRotation.x, 0);
        transform.localRotation = Quaternion.AngleAxis(CameraRotation.x, Vector3.up);

        if (Health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 InputAxis = context.ReadValue<Vector2>();

        verticalMove = InputAxis.y;
        horizontalMove = InputAxis.x;

    }
    public void Jump()
    {
        if (Physics.Raycast(ray, JumpDistance))
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);


        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DeathZone")
        {
            Health = 0;
        }
        if ((other.tag == "HealPickup") & (Health+10 < MaxHealth))
        {
            Destroy(other.gameObject);
            Health+= 50;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "DamagePart")
        {
            Health--;
        }
        if (collision.gameObject.tag == "BasicEnemy")
        {
            Health--;
        }
        if (collision.gameObject.tag == "StrongEnemy")
        {
            Health -= 3;
        }
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (currentWeapon)
        {
            if (currentWeapon.holdToAttack)
            {
                if (context.ReadValueAsButton())
                {
                    attacking = true;
                }
                else
                {
                    attacking = false;
                }
            }
        }
    }
    public void Reload()
    {
        if (currentWeapon)
        {
            if (!currentWeapon.reloading)
            {
                currentWeapon.reload();
            }
        }
    }
    public void Interact()
    {
        if (pickupObj)
        {
            if (pickupObj.tag == "Weapon")
            {
                if (currentWeapon)
                {
                    
                }
                else
                {
                    pickupObj.GetComponent<Weapon>().equip(this);
                }
            }
            else
            {
                Reload();
            }

        }
    }
    public void DropWeapon()
    {
        if (currentWeapon)
        {
            currentWeapon.GetComponent<Weapon>().unequip();
        }
    }
}
