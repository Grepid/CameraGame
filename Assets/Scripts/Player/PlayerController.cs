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
    // Player Controls

    private bool acceptingInputs;

    // Player Components
    private Camera cam;
    private Rigidbody rb;

    // Player Movement
    private Vector2 moveDirection;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float xSens;
    [SerializeField] private float ySens;
    private Vector2 lookDelta;
    private Vector2 lookXY;

    // Player Jumping
    private bool isOnGround;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float gravity;

    private void Awake()
    {
        cam = Camera.main;
        lookXY = new Vector2(cam.transform.eulerAngles.x, transform.eulerAngles.y);
    }

    private void Start()
    {
        Initialise();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Physics.gravity = new Vector3(0, -gravity, 0);

    }

    private void Initialise()
    {

        rb = GetComponent<Rigidbody>();

    }
    bool downwardsPressure = true;
    private void Update()
    {
        AssignConstantVariables();
        LookUpdate();
        InputCheck();
        MovePlayer();
        //if (rb.velocity.y < 0.1f && rb.velocity.y > -0.1f) isOnGround = true;
        if (downwardsPressure)
        {
            rb.AddForce(Vector3.down * 100);
        }
    }

    private Vector2 oldLookPos = Vector2.zero;
    private void AssignConstantVariables()
    {
        //lookDelta = (Vector2)Input.mousePosition - oldLookPos;
        lookDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //oldLookPos = Input.mousePosition;
    }

    private void FixedUpdate()
    {

    }

    private void MovePlayer()
    {
        Vector3 direction = transform.forward * moveDirection.y + transform.right * moveDirection.x;
        rb.velocity = new Vector3(direction.normalized.x * moveSpeed * Time.deltaTime, rb.velocity.y, direction.normalized.z * moveSpeed * Time.deltaTime);
        //rb.AddForce(direction.normalized * moveSpeed, ForceMode.Force);
    }

    private void LookUpdate()
    {
        lookXY.x -= lookDelta.y * xSens * Time.smoothDeltaTime;
        lookXY.y += lookDelta.x * ySens * Time.smoothDeltaTime;
        lookXY.x = Mathf.Clamp(lookXY.x, -90f, 90f);
        transform.rotation = Quaternion.Euler(0, lookXY.y, 0);
        cam.transform.rotation = Quaternion.Euler(lookXY.x, lookXY.y, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isOnGround = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isOnGround = false;
    }
    private void InputCheck()
    {
        if (Input.GetKeyDown(KeyCode.Space)) OnJump();
        if(Input.GetKeyUp(KeyCode.Space)) ReleaseJump();
        if (Input.GetKeyDown(KeyCode.E)) Interact();
    }

    void ReleaseJump()
    {
        downwardsPressure = true;
    }

    private void OnJump()
    {
        print(isOnGround);
        if (isOnGround)
        {
            downwardsPressure = false;
            rb.AddForce(0, jumpVelocity, 0);
        }
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
