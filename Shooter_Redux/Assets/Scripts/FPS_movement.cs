using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_movement : MonoBehaviour
{
    StateManager states;
    Rigidbody rb;
    public Transform fpsCameraHolder;

    Vector3 lookPosition;
    Vector3 storeDirection;

    public float runSpeed = 3;
    public float walkSpeed = 1.5f;
    public float aimSpeed = 1;
    public float speedMultiplier = 10;
    public float rotateSpeed = 2;
    public float turnSpeed = 5;

    float horizontal;
    float vertical;

    Vector3 lookDirection;

    PhysicMaterial zFriction;
    PhysicMaterial mFriction;
    Collider col;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        states = GetComponent<StateManager>();
        col = GetComponent<Collider>();

        zFriction = new PhysicMaterial();
        zFriction.dynamicFriction = 0;
        zFriction.staticFriction = 0;

        mFriction = new PhysicMaterial();
        mFriction.dynamicFriction = 1;
        mFriction.staticFriction = 1;

    }
    private void FixedUpdate()
    {
        lookPosition = states.lookPosition;
        lookDirection = lookPosition - transform.position;

        horizontal = states.horizontal;
        vertical = states.vertical;

        bool onGround = states.onGround;

        if (horizontal != 0 || vertical != 0 || !onGround)
        {
            col.material = zFriction;
        }
        else
        {
            col.material = mFriction;
        }

        Vector3 v = fpsCameraHolder.forward * vertical;
        Vector3 h = fpsCameraHolder.right * horizontal;

        v.y = 0;
        h.y = 0;

        HandleMovement(h, v, onGround);
        HandleRotation();

        if (onGround)
        {
            rb.drag = 4;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void HandleRotation()
    {
        lookDirection.y = 0;

        lookDirection += transform.position;
        transform.LookAt(lookDirection);
    }

    private void HandleMovement(Vector3 h, Vector3 v, bool onGround)
    {
        if (onGround)
        {
            rb.AddForce((v + h).normalized * speed());
        }
    }
    float speed()
    {
        float speed = 0;

        if (states.aiming)
        {
            speed = aimSpeed;
        }
        else
        {
            if (states.walk || states.reloading)
            {
                speed = walkSpeed;
            }
            else
            {
                speed = runSpeed;
            }
        }
        speed *= speedMultiplier;
        return speed;
    }
}
