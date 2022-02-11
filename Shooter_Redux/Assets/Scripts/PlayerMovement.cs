using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    InputHandler ih;
    StateManager states;
    Rigidbody rb;

    Vector3 lookPosition;
    Vector3 storeDirection;

    public float runSpeed = 3;
    public float walkSpeed = 1.5f;
    public float aimSpeed = 1;
    public float speedMultiplier = 10;
    public float rotateSpeed = 2;
    public float turnSpeed = 5;

    public float coverAcceleration = 0.5f;
    public float coverMaxSpeed = 2;

    float horizontal;
    float vertical;

    Vector3 lookDirection;

    PhysicMaterial zFriction;
    PhysicMaterial mFriction;
    Collider col;

    List<CoverPosition> ignoreCover = new List<CoverPosition>();

    private void Start()
    {
        ih = GetComponent<InputHandler>();
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

        if (!states.inCover && !states.vaulting)
        {
            HandleMovementNormal();

            if (horizontal != 0 || vertical != 0)
            {
                SearchForCover();
            }
        }
        else
        {
            if (!states.aiming && !states.vaulting)
            {
                HandleCoverMovement();
            }

            GetOutOfCover();
        }

        col.isTrigger = states.vaulting;
    }

    private void GetOutOfCover()
    {
        if (vertical < -0.5f)
        {
            if (!states.aiming)
            {
                states.coverPos = null;
                states.inCover = false;

                StartCoroutine("ClearIgnoreList");
            }
        }
    }

    IEnumerator ClearIgnoreList()
    {
        yield return new WaitForSeconds(1);
        ignoreCover.Clear();
    }
    private void HandleCoverMovement()
    {
        if (horizontal != 0)
        {
            if (horizontal < 0)
            {
                states.coverDirection = -1;
            }
            else
            {
                states.coverDirection = 1;
            }
        }

        //Quaternion targetRotation = Quaternion.LookRotation(states.coverPos.curvePath.GetPointAt(0));
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);

        float lineLength = states.coverPos.length;

        float movement = ((horizontal * coverAcceleration)
                        * coverMaxSpeed) * Time.deltaTime;

        float lerpMovement = movement / lineLength;

        states.coverPercentage -= lerpMovement;

        states.coverPercentage = Mathf.Clamp01(states.coverPercentage);

        Vector3 curvePathPosition = states.coverPos.curvePath.GetPointAt(states.coverPercentage);

        curvePathPosition.y = 0;// transform.position.y;

        //HandleCoverRotation();
        transform.position = curvePathPosition;
    }
    void HandleCoverRotation()
    {
        float forwardPerc = states.coverPercentage + 0.1f;

        if (forwardPerc > 0.99f)
        {
            forwardPerc = 1;
        }

        Vector3 positionNow = states.coverPos.curvePath.GetPointAt(states.coverPercentage);
        Vector2 positionForward = states.coverPos.curvePath.GetPointAt(forwardPerc);


        Vector3 direction = Vector3.Cross(positionNow, positionForward);

        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);

    }
    private void SearchForCover()
    {
        Vector3 origin = transform.position + Vector3.up / 2;
        Vector3 direction = transform.forward;
        RaycastHit hit;

        if(Physics.Raycast(origin,direction,out hit, 2))
        {
            float distance = Vector3.Distance(origin, hit.point);

            //if(distance < 1.5f)

            if (hit.transform.GetComponentInParent<CoverPosition>())
            {

                if (!ignoreCover.Contains(hit.transform.GetComponentInParent<CoverPosition>()))
                {
                        CoverPosition cov = hit.transform.GetComponentInParent<CoverPosition>();

                        //if (distance < 0.5f && !states.vaulting)
                        //{
                            states.GetInCover(hit.transform.GetComponentInParent<CoverPosition>());

                            ignoreCover.Add(hit.transform.GetComponentInParent<CoverPosition>());
                        //}
                        //else
                        //{
                        //    if (Input.GetKey(KeyCode.Space))
                        //    {
                        //        if (!states.vaulting)
                        //        {
                        //            bool climb = false;

                        //            if (cov.coverType == CoverPosition.CoverType.full)
                        //            {
                        //                climb = true;
                        //            }
                        //           // states.Vault(climb);
                        //        }
                        //    }
                        //}
                }
            }
        }
    }

    void HandleMovementNormal()
    {
        bool onGround = states.onGround;

        if (horizontal != 0 || vertical != 0 || !onGround || states.vaulting)
        {
            col.material = zFriction;
        }
        else
        {
            col.material = mFriction;
        }

        Vector3 v = ih.camTrans.forward * vertical;
        Vector3 h = ih.camTrans.right * horizontal;

        v.y = 0;
        h.y = 0;

        HandleMovement(h, v, onGround);
        HandleRotation(h, v, onGround);

        if (onGround)
        {
            rb.drag = 4;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void HandleRotation(Vector3 h, Vector3 v, bool onGround)
    {
        if (states.aiming && !states.inCover)
        {
            lookDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotateSpeed);

        }
        else
        {
            if (!states.inCover)
            {
                storeDirection = transform.position + h + v;

                Vector3 dir = storeDirection - transform.position;
                dir.y = 0;

                if (horizontal != 0 || vertical != 0)
                {
                    float angl = Vector3.Angle(transform.forward, dir);

                    if (angl != 0)
                    {
                        float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));
                        if (angle != 0)
                        {
                            rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
                        }
                    }
                }
            }
        }
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
