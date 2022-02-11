using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKhandler : MonoBehaviour
{
    Animator anim;
    StateManager states;

    public float lookWeight = 1;
    public float bodyWeight = 0.8f;
    public float headWeight = 1;
    public float clampWeight = 1;

    float targetWeight;

    public Transform weaponHolder;
    public Transform rightShoulder;


    public Transform overrideLookTarget;

    public Transform righthadnIkTarget;
    public Transform righthadnIkRotation;
    public Transform rightElbowTarget;
    
    [HideInInspector]
    public float rightHandIkWeight;
    float targetRHWeight;


    public Transform leftHandIkTarget;
    public Transform leftElbowTarget;
    [HideInInspector]
    public float leftHandIkWeight;
    float targetLHWeight;

    Transform aimHelperRS;
    private Vector3 secondHandLookPosition;
    Transform aimHelperLS;

    [HideInInspector]
    public bool LHIK_dis_notAiming;
    private bool enableTwoHandWield;
    private Transform secondaryWeaponHolder;

    private void Start()
    {
        aimHelperRS = new GameObject().transform;
        aimHelperLS = new GameObject().transform;
        anim = GetComponent<Animator>();
        states = GetComponent<StateManager>();
    }

    private void FixedUpdate()
    {

        HandleShoulders();

        AimWeight();
        HandleRightHandIkWeight();
        HandleLeftHandIkWeight();
        HandleShoulderRotation();

    }

    private void HandleLeftHandIkWeight()
    {
        float multiplier = 3;

        if (states.inCover)
        {
            targetRHWeight = 0;

            if (!LHIK_dis_notAiming)
            {
                targetLHWeight = 1;
                multiplier = 6;
            }
            else
            {
                multiplier = 10;

                if (states.aiming)
                {
                    targetLHWeight = 1;
                }
                else
                {
                    targetLHWeight = 0;
                    leftHandIkWeight = 0;
                }
            }
        }
        else
        {
            if (!LHIK_dis_notAiming)
            {
                targetLHWeight = 1;

                multiplier = 10;
            }
            else
            {
                multiplier = 10;
                targetLHWeight = (states.aiming) ? 1 : 0;

            }
        }

        if (states.reloading)
        {
            targetRHWeight = 0;
            multiplier = 5;
        }

        leftHandIkWeight = Mathf.Lerp(leftHandIkWeight, targetLHWeight, Time.deltaTime * multiplier);

    }

    private void HandleRightHandIkWeight()
    {
        float multiplier = 3;

        if (states.inCover)
        {
            targetRHWeight = 0;

            if (states.aiming)
            {
                targetRHWeight = 1;
                multiplier = 2;
            }
            else
            {
                multiplier = 10;
            }
        }
        else
        {
            rightHandIkWeight = lookWeight;
        }

        if (states.reloading)
        {
            targetRHWeight = 0;
            multiplier = 5;
        }

        rightHandIkWeight = Mathf.Lerp(rightHandIkWeight, targetRHWeight, Time.deltaTime * multiplier);
    }

    private void AimWeight()
    {
        if (states.aiming && !states.reloading)
        {
            Vector3 directionTowardsTarget = aimHelperRS.position - transform.position;
            float angle = Vector3.Angle(transform.forward, directionTowardsTarget);

            if (angle < 90)
            {
                targetWeight = 1;
            }
            else
            {
                targetWeight = 0;
            }
        }
        else
        {
            targetWeight = 0;
        }

        float multiplier = (states.aiming) ? 5 : 30;

        lookWeight = Mathf.Lerp(lookWeight, targetWeight, Time.deltaTime * multiplier);

        
    }

    private void HandleShoulders()
    {
        if (rightShoulder == null)
        {
            rightShoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
        }
        else
        {
            weaponHolder.position = rightShoulder.position;
        }
    }

    private void HandleShoulderRotation()
    {
        aimHelperRS.position = Vector3.Lerp(aimHelperRS.position, states.lookPosition, Time.deltaTime * 5);
        weaponHolder.LookAt(aimHelperRS.position);
        righthadnIkTarget.parent.transform.LookAt(aimHelperRS.position);
        righthadnIkTarget.transform.LookAt(aimHelperRS.position);

        if (enableTwoHandWield)
        {
            secondHandLookPosition = states.lookPosition;

            aimHelperLS.position = Vector3.Lerp(aimHelperLS.position, secondHandLookPosition, Time.deltaTime * 5);
            secondaryWeaponHolder.LookAt(aimHelperLS.position);
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetLookAtWeight(lookWeight, bodyWeight, headWeight, headWeight, clampWeight);

        Vector3 filterDirection = states.lookPosition;
        //filterDirection.y = offsetY; if needed

        anim.SetLookAtPosition(
            (overrideLookTarget != null) ?
            overrideLookTarget.position : filterDirection
            );

        if (leftHandIkTarget)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandIkWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIkTarget.position);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandIkWeight);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIkTarget.rotation);
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }

        if (righthadnIkTarget)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandIkWeight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, righthadnIkTarget.position);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandIkWeight);
            anim.SetIKRotation(AvatarIKGoal.RightHand, righthadnIkRotation.rotation);
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        }

        if (leftElbowTarget)
        {
            anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftHandIkWeight);
            anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowTarget.position);
        }
        else
        {
            anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
        }
        if (rightElbowTarget)
        {
            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightHandIkWeight);
            anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowTarget.position);
        }
        else
        {
            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
        }
    }
}
