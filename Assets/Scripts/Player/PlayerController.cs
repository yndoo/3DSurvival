using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    private Vector3 curMovementInput;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform CameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot; 
    public float lookSensitiity;
    private Vector2 mouseDelta;
    public bool canLook = true;

    public Action inventory;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Ŀ���� ������ �ʰ� ��. 
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x; // curMovementInput�� x, y ���� 3D ��ǥ�迡���� x, y�� ���� �ƴ�. 
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y; // ������ ���� ���� ���Ʒ��� �����̱� ����....? 

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        // ī�޶� x�� ȸ���� ����
        camCurXRot += mouseDelta.y * lookSensitiity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        CameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0); 

        // �÷��̾� �¿� ȸ���� y�� ȸ��
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitiity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask)) // 0.1f ����, groundLayerMask�� �ش�Ǵ� �ֵ鸸 ���� 
            {
                return true;
            }
        }

        return false;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
