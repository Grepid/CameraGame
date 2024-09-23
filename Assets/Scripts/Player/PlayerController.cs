using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface Iinteractable
{
    public void Interact();
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public bool acceptingInputs;
    private CharacterController cc;
    private Camera cam;
    Vector2 moveInput;
    Vector2 lookXY;
    public float horizontalMouse;
    public float verticalMouse;
    private Vector3 velocity;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundLayerMask;
    bool isGrounded;

    public float speed;
    Vector3 moveDirection
    {
        get
        {
            return (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        }
    }
    private void Awake()
    {
        Physics.gravity *= 2;
        cc = GetComponent<CharacterController>();
        instance = this;
    }
    private void Start()
    {
        ToggleMouse();
        cam = Camera.main;
        groundCheck = transform.Find("GroundCheckLocation");
    }
    private void Update()
    {
        AssignVariables();
        if (!acceptingInputs)
        {
            Move();
            Rotate();
            CheckInputs();
        }
    }

    private void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKeyDown(KeyCode.E)) Interact();
        if (Input.GetKeyDown(KeyCode.L)) print(TargetManager.GetRandomFromTypes(new List<PhotoTargetType> {PhotoTargetType.Shiny, PhotoTargetType.Artifact, PhotoTargetType.MonsterDead }));
    }

    public void ToggleMouse()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public void UIModeToggle()
    {
        ToggleMouse();
        acceptingInputs = !acceptingInputs;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            velocity.y = 10f;
        }
    }
    private void Move()
    {
        cc.Move(moveDirection * speed* Time.deltaTime);

        if (isGrounded && velocity.y < 0) velocity = Vector3.down * 2;
        else
        {
            velocity += Physics.gravity * Time.deltaTime;
        }
        cc.Move(velocity * Time.deltaTime);
    }
    private void Rotate()
    {
        transform.eulerAngles = new Vector3(0, lookXY.x, 0);
        cam.transform.eulerAngles = new Vector3(lookXY.y, lookXY.x, 0);
    }

    private void AssignVariables()
    {
        Vector2 look;
        look.x = Input.GetAxis("Mouse X")*horizontalMouse*Time.smoothDeltaTime;
        look.y = Input.GetAxis("Mouse Y")*-verticalMouse*Time.smoothDeltaTime;

        lookXY += look;
        lookXY.y = Mathf.Clamp(lookXY.y, -90, 90);

        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayerMask);
    }
    private void Interact()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Physics.Raycast(ray, out RaycastHit hit);
        if (hit.collider == null) return;
        Iinteractable interact = hit.collider.GetComponent<Iinteractable>();
        if (interact == null) return;
        interact.Interact();

    }
}
