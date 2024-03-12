using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Transform _orientation;
    [SerializeField] private float _groundDrag;
    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundMask;
    [Header("Jump")]
    [SerializeField] private float _jumpCoolDown;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _airMultiplier;
    [SerializeField] private float _playerHeight;
    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _moveDirection;
    private Rigidbody _rb;
    private bool _grounded;
    private bool _readyToJump = true;
    private Gun _gun;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _gun = GetComponent<Gun>();
        _rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        _grounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundMask);
        PlayerInput();
        SpeedControl();

        _rb.drag = _grounded ? _groundDrag : 0;

        if (Input.GetMouseButtonDown(0))
        {
            _gun.Fire();
            // Debug.Log("Fire");
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void PlayerInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && _readyToJump && _grounded)
        {
            _readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), _jumpCoolDown);
        }
    }

    private void MovePlayer()
    {
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

        if (_grounded) _rb.AddForce(_moveDirection * _moveSpeed * 10f, ForceMode.Force);
        else _rb.AddForce(_moveDirection * _moveSpeed * _airMultiplier * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

        if (flatVel.magnitude > _moveSpeed)
        {
            Vector3 limitVel = flatVel.normalized * _moveSpeed;
            _rb.velocity = new Vector3(limitVel.x, _rb.velocity.y, limitVel.z);
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }
}
