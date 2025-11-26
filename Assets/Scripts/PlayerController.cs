using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;

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
    private float _jumpHeight = 5;
    private float _gravity = -10;
    private Vector3 _playerGravity;


    void Awake()
    {
        _characterController = GetComponent<CharacterController>();

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

        if(_jumpAction.WasPressedThisFrame() && isGrounded())
        {
            Jump();
        }
    }

    void Movement()
    {
        Vector3 characterMovement = new Vector3(_moveInput.x, 0, _moveInput.y);
        _characterController.Move(characterMovement * _speed * Time.deltaTime);
    }

    bool isGrounded()
    {
        return Physics.CheckSphere(_sensor.position, _sensorRadius, _groundLayer);
    }

    void Gravity()
    {
        if(!isGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if(_playerGravity.y < 0 && isGrounded())
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
