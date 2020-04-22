using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlanetPlayerController : MonoBehaviour
{
    [Header("Player Setting")]
    public CharacterController characterController;
    public Transform groundCheck;
    public float speed = 12f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    [Header("JoyStick")]
    public Joystick joystick;
    [Header("Map Mask")]
    public LayerMask groundMask;
    public Transform planet;

    Vector3 m_velocity;
    Vector3 m_movement;
    float m_groundDistance = 0.4f;
    float m_currentGravityForce = 0f;
    bool m_isGrounded;
    bool m_moving;
    float m_currentSpeed = 0f;

    void Start()
    {

    }

    void Update()
    {
        CheckIfGrounded();
        UpdateMovement();
        GravityMove();
    }

    void UpdateMovement()
    {
        bool joystickX = joystick.Horizontal > 0.2 || joystick.Horizontal < -0.2;
        bool joystickZ = joystick.Vertical > 0.2 || joystick.Vertical < -0.2;

        float x = joystickX ? joystick.Horizontal : Input.GetAxis("Horizontal");
        float z = joystickZ ? joystick.Vertical : Input.GetAxis("Vertical");

        m_movement = transform.right * x + transform.forward * z;

        characterController.Move(m_movement * speed * Time.deltaTime);
        if (!m_movement.Equals(Vector3.zero))
        {
            // TODO Rotation Towards
            //RotationTowards(movement);
            m_moving = true;
            m_currentSpeed = m_movement.magnitude;
        }
        else
        {
            m_moving = false;
            m_currentSpeed = 0f;
        }

    }

    void GravityMove()
    {
        if (!m_moving)
            return ;
        m_velocity = planet.position - groundCheck.position;
        m_velocity.Normalize();

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -m_velocity) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Fall
        if (m_isGrounded && m_currentGravityForce < 0)
            m_currentGravityForce = -2f;
        else
            // * 2 for fast falling
            m_currentGravityForce += gravity * Time.deltaTime * 2;

        // Jump
        if (Input.GetButtonDown("Jump") && m_isGrounded)
            m_currentGravityForce = Mathf.Sqrt(jumpHeight * -2f * gravity);

        characterController.Move(transform.up * m_currentGravityForce * Time.deltaTime);
    }

    void CheckIfGrounded()
    {
        m_isGrounded = Physics.CheckSphere(groundCheck.position, m_groundDistance, groundMask);
    }

    void RotationTowards(Vector3 movement)
    {
        Quaternion rotation = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

}
