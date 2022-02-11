using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public bool aiming;
    public bool canRun;
    public bool dontRun;
    public bool walk;
    public bool shoot;
    public bool actualShooting;
    public bool reloading;
    public bool onGround;

    public bool crouching;
    public float stance;
    public float coverPercentage;
    public CoverPosition coverPos;
    public bool inCover;
    public int coverDirection;
    public bool canAim;
    public bool crouchCover;
    public bool aimAtSides;

    public bool vaulting;
    bool climb;
    public BezierCurve vaultCurve;
    public BezierCurve climbCurve;
    Vector3 curvePos;
    bool initVault;

    public float horizontal;
    public float vertical;
    public Vector3 lookPosition;
    public Vector3 lookHitPosition;

    public LayerMask layerMask;

    public CharacterAudioManager audioManager;

    [HideInInspector]
    public HandleShooting handleShooting;
    [HideInInspector]
    public HandleAnimations handleAnim;
    [HideInInspector]
    public WeaponManager weaponManager;

    private void Start()
    {
        audioManager = GetComponent<CharacterAudioManager>();
        handleShooting = GetComponent<HandleShooting>();
        handleAnim = GetComponent<HandleAnimations>();
        weaponManager = GetComponent<WeaponManager>();
    }
    private void FixedUpdate()
    {
        onGround = IsOnGround();

        walk = inCover;

        HandleStance();
       // HandleVault();
    }
    float targetStance;
    private void HandleStance()
    {
        if (!crouching)
        {
            targetStance = 1;
        }
        else
        {
            targetStance = 0;
        }

        stance = Mathf.Lerp(stance, targetStance, Time.deltaTime * 3f);
    }

    public void GetInCover(CoverPosition cover)
    {
        float disFromPos1 = Vector3.Distance(transform.position, cover.curvePath.GetPointAt(0));

        coverPercentage = disFromPos1 / cover.length;

        //Vector3 dir = cover.pos2.position - cover.pos1.position;
        //dir.Normalize();

        Vector3 targetPos = cover.curvePath.GetPointAt(coverPercentage);
        //(dir * disFromPos1) + cover.pos1.position;

        StartCoroutine(LerpToCoverPositionPercentage(targetPos));

        coverPos = cover;
        inCover = true;
    }

    public void Vault(bool climb = false)
    {
        this.climb = false;
        this.climb = climb;

        BezierCurve curve = (climb) ? climbCurve : vaultCurve;

        curve.transform.rotation = transform.rotation;
        curve.transform.position = transform.position;

        string desiredAnimation = (climb) ? "Climb" : "Vault";

        handleAnim.anim.CrossFade(desiredAnimation, 0.2f);
        curve.close = false;
        percentage = 0;
        vaulting = true;
    }
    float percentage;
    bool ignorevault;

    void HandleVault()
    {
        if (vaulting)
        {
            BezierCurve curve = (climb) ? climbCurve : vaultCurve;

            float lineLength = curve.length;

            float speedModifier = handleAnim.anim.GetFloat("CurveSpeed");

            float speed = (climb) ? 4 * speedModifier : 6;

            float movement = speed * Time.deltaTime;
            float lerpMovement = movement / lineLength;

            percentage += lerpMovement;

            if (percentage > 1)
            {
                vaulting = false;
            }

            Vector3 targetposition = curve.GetPointAt(percentage);

            transform.position = targetposition;

        }
    }
    IEnumerator LerpToCoverPositionPercentage(Vector3 targetPos)
    {
        Vector3 startingPos = transform.position;
        Vector3 tPos = targetPos;
        targetPos.y = transform.position.y;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5;

            transform.position = Vector3.Lerp(startingPos, tPos, t);
            yield return null;
        }
    }
    bool IsOnGround()
    {
        bool retVal = false;

        Vector3 origin = transform.position + new Vector3(0, 0.05f, 0);
        RaycastHit hit;

        if(Physics.Raycast(origin,-Vector3.up,out hit, 0.5f, layerMask))
        {
            retVal = true;
        }

        return retVal;
    }
}
