using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    private Animator _animator;

    //Inputs
    private InputAction _moveAction;
    public Vector2 _moveInput;

    private InputAction _jumpAction;

    //Movement
    float _speed = 8;

    //GroundSensor
    [SerializeField] Transform _sensor;
    [SerializeField] float _sensorRadius;
    [SerializeField] LayerMask _groundLayer;

    //Jump & Gravity
    private float _jumpHeight = 2f;
    private float _gravity = -10;
    private Vector3 _playerGravity;

    //Camera
    [SerializeField] Camera _mainCamera;
    float _smoothTime = 1;
    float _currentVelocity;
    


    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        //Inputs
        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
    }

    void Start()
    {
        
    }

    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();

        Movement();
        Gravity();

        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            
            _animator.SetTrigger("hasJumped");
            Jump();
        }

        //_animator.SetBool("isGrounded", !IsGrounded());
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);
        _characterController.Move(direction * _speed * Time.deltaTime);

        _animator.SetFloat("Horizontal", direction.x);
        _animator.SetFloat("Vertical", direction.magnitude);

        //Queda acabar esto
        if(direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(_mainCamera.transform.eulerAngles.y, targetAngle, ref _currentVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            _mainCamera.transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
            //Vector3 newDirection = new Vector3(smoothAngle, 0, 0);
            //_characterController.Move(smoothAngle * Time.deltaTime);
        }
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensor.position, _sensorRadius, _groundLayer);
    }

    void Gravity()
    {
        if(!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if(_playerGravity.y < 0 && IsGrounded())
        {
            _playerGravity.y = -3;
        }
        _characterController.Move(_playerGravity * Time.deltaTime);
    }

    void Jump()
    {
        _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
        _characterController.Move(_playerGravity * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_sensor.position, _sensorRadius);
    }
}
