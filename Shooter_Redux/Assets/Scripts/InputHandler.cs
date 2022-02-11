using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [HideInInspector]
    public float horizontal;
    [HideInInspector]
    public float vertical;
    [HideInInspector]
    public float mouse1;
    [HideInInspector]
    public float mouse2;
    [HideInInspector]
    public float fire3;
    [HideInInspector]
    public float middleMouse;
    [HideInInspector]
    public float mouseX;
    [HideInInspector]
    public float mouseY;

    [HideInInspector]
    public FreeCameraLook camProperties;
    [HideInInspector]
    public Transform camPivot;
    [HideInInspector]
    public Transform camTrans;

    CrosshairManager crosshairManager;
    ShakeCamera shakeCam;
    StateManager states;

    public float normalFov = 60;
    public float aimingFov = 40;
    float targetFov;
    float curFov;
    public float cameraNormalZ = -2;
    public float cameraAimingZ = -0.86f;
    float targetZ;
    float actualZ;
    float curZ;
    LayerMask layerMask;

    public float shakeRecoil = 0.5f;
    public float shakeMovement = 0.3f;
    public float shakeMin = 0.1f;
    float targetShake;
    float curShake;

    public bool leftPivot;
    public bool changePivot;
    public bool crouch;

    public CameraValues cameraValues;
    [System.Serializable]
    public class CameraValues
    {
        public float coverCameraOffsetX = 0.2f;
        public float coverCameraOffsetZ = -0.2f;
        public float coverCameraOffsetY = 0;
        public Vector3 targetCameraOffset;
        public Vector3 startingPivotPos;

        public float coverLeftMaxAngle = 30;
        public float coverLeftMinAngle = -30;
        public float coverRightMaxAngle=30;
        public float coverRightMinAngle=-30;
    }

    public bool fpsMode;
    bool canSwitch;
    ControllerSwitcher conSwitcher;

    private void Start()
    {
        crosshairManager = CrosshairManager.GetInstance();
        camProperties = FreeCameraLook.GetInstance();
        camPivot = camProperties.transform.GetChild(0);
        camTrans = camPivot.GetChild(0);
        shakeCam = camPivot.GetComponentInChildren<ShakeCamera>();

        states = GetComponent<StateManager>();

        layerMask = ~(1 << gameObject.layer);
        states.layerMask = layerMask;

        conSwitcher = ControllerSwitcher.GetInstance();
        if (conSwitcher != null)
        {
            canSwitch = true;
        }

        cameraValues.startingPivotPos = camPivot.localPosition;
    }

    private void FixedUpdate()
    {
        HandleInput();
        UpdateStates();
        HandleShake();

        Ray ray = new Ray(camTrans.position, camTrans.forward);
        states.lookPosition = ray.GetPoint(20);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction);

        if(Physics.Raycast(ray.origin,ray.direction,out hit, 100, layerMask))
        {
            states.lookHitPosition = hit.point;
        }
        else
        {
            states.lookHitPosition = states.lookPosition;
        }
        if (!fpsMode)
        {
            CameraCollision(layerMask);

            curZ = Mathf.Lerp(curZ, actualZ, Time.deltaTime * 15);
            camTrans.localPosition = new Vector3(0, 0, curZ);

            cameraValues.targetCameraOffset = Vector3.zero;

            float pivotX = (!leftPivot) ? cameraValues.startingPivotPos.x : -cameraValues.startingPivotPos.x;
            float pivotY = (!states.crouching) ? cameraValues.startingPivotPos.y : cameraValues.startingPivotPos.y - 0.5f;

            if (states.inCover)
            {
                pivotX = cameraValues.startingPivotPos.x * states.coverDirection;

                camProperties.inCover = states.aiming;
                camProperties.coverDirection = states.coverDirection;
                camProperties.overrideTarget = states.aiming;

                if (states.aiming)
                {
                    Vector3 localPos = Vector3.zero;
                    if (states.crouchCover && !states.aimAtSides)
                    {
                        pivotY = cameraValues.startingPivotPos.y;

                        camProperties.coverAngleMin = -50;
                        camProperties.coverAngleMax = 50;
                    }
                    else
                    {
                        localPos = new Vector3(cameraValues.coverCameraOffsetX * states.coverDirection, 0,
                                            cameraValues.coverCameraOffsetZ);


                        camProperties.coverAngleMin = (states.coverDirection < 0) ? cameraValues.coverLeftMinAngle : cameraValues.coverRightMinAngle;
                        camProperties.coverAngleMax = (states.coverDirection < 0) ? cameraValues.coverLeftMaxAngle : cameraValues.coverRightMaxAngle;

                    }
                    Vector3 worldPos = transform.TransformPoint(localPos);

                    worldPos.y = transform.position.y;

                    cameraValues.targetCameraOffset = worldPos;
                }
                
            }
            Vector3 targetPivotPos = new Vector3(pivotX, pivotY, cameraValues.startingPivotPos.z);
            camPivot.localPosition = Vector3.Lerp(camPivot.localPosition, targetPivotPos, Time.deltaTime * 3);

            camProperties.newTargetPosition = Vector3.Lerp(camProperties.newTargetPosition,
                                                        cameraValues.targetCameraOffset, Time.deltaTime * 3);

        }
    }

    private void CameraCollision(LayerMask layerMask)
    {
        Vector3 origin = camPivot.TransformPoint(Vector3.zero);
        Vector3 direction = camTrans.TransformPoint(Vector3.zero) - camPivot.TransformPoint(Vector3.zero);

        RaycastHit hit;

        actualZ = targetZ;

        if(Physics.Raycast(origin,direction,out hit, Mathf.Abs(targetZ), layerMask))
        {
            float dis = Vector3.Distance(camPivot.position, hit.point);
            actualZ = -dis;
        }
    }

    private void HandleInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouse1 = Input.GetAxis("Fire1");
        mouse2 = Input.GetAxis("Fire2");
        middleMouse = Input.GetAxis("Mouse ScrollWheel");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        fire3 = Input.GetAxis("Fire3");
        crouch = Input.GetKeyDown(KeyCode.C);

        if (canSwitch)
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                Ray ray = new Ray(camTrans.position, camTrans.forward);
                Vector3 lookPos = ray.GetPoint(20);

                if (!fpsMode)
                {
                    conSwitcher.SwitchToFps(lookPos);
                    
                }
                else
                    conSwitcher.SwitchToTps(lookPos);

                fpsMode = ControllerSwitcher.GetInstance().fps;
            }
        }

        if (states.inCover)
        {
            changePivot = false;

            if (states.coverPos.coverType == CoverPosition.CoverType.half)
            {
                states.crouchCover = true;
            }
            else
            {
                states.crouchCover = false;
                states.aimAtSides = false;
            }

            if (states.aiming)
            {
                horizontal = 0;
            }
            else
            {
                if (states.coverPercentage > 0.99f)
                {
                    if (!states.coverPos.blockPos2)
                    {
                        states.canAim = true;
                        states.aimAtSides = true;
                    }

                    horizontal = Mathf.Clamp(horizontal, 0, 1);
                }
                else
                {
                    if (states.coverPercentage < 0.01f)
                    {
                        horizontal = Mathf.Clamp(horizontal, -1, 0);

                        if (!states.coverPos.blockPos1)
                        {
                            states.canAim = true;
                            states.aimAtSides = true;
                        }
                    }
                    else
                    {
                        //states.canAim = false;
                        states.aimAtSides = false;
                    }
                }
            }
        }
        else
        {
            states.canAim = false;
            states.crouchCover = false;

            if (Input.GetKeyDown(KeyCode.E))
            {
                changePivot = !changePivot;
            }

            leftPivot = changePivot;
        }
    }

    private void HandleShake()
    {
        if (states.actualShooting && states.handleShooting.curBullets > 0)
        {
            targetShake = shakeRecoil;
            camProperties.WiggleCrosshairAndCamera(0.2f);
            targetFov += 5;
        }
        else
        {
            if (states.vertical != 0)
            {
                targetShake = shakeMovement;
            }
            else
            {
                if (states.horizontal != 0)
                {
                    targetShake = shakeMovement;
                }
                else
                {
                    targetShake = shakeMin;
                }
            }
        }

        curShake = Mathf.Lerp(curShake, targetShake, Time.deltaTime * 10);
        shakeCam.positionShakeSpeed = curShake;

        curFov = Mathf.Lerp(curFov, targetFov, Time.deltaTime * 5);
        Camera.main.fieldOfView = curFov;
    }

    private void UpdateStates()
    {
        states.canRun = !states.aiming;

        if (!states.inCover)
        {
            if (!states.dontRun)
                states.walk = (fire3 > 0);
            else
                states.walk = true;
        }
        
        states.horizontal = horizontal;
        states.vertical = vertical;

        if (states.inCover)
        {
            if (states.crouchCover)
            {
                if (mouse2 > 0 )
                {
                    states.aiming = true;
                }
                else
                {
                    states.aiming = false;
                }
            }
            else
            {
                if (mouse2 > 0 && states.canAim)
                {
                    states.aiming = true;
                }
                else
                {
                    states.aiming = false;
                }
            }
        }
        else
        {
            states.aiming = states.onGround && (mouse2 > 0);
        }
        if (states.aiming)
        {
            targetZ = cameraAimingZ;
            targetFov = aimingFov;

            if (mouse1 > 0.5f && !states.reloading)
            {
                states.shoot = true;

            }
            else
            {
                states.shoot = false;
            }
        }
        else
        {
            states.shoot = false;
            targetZ = cameraNormalZ;
            targetFov = normalFov;
        }

        if (crouch)
        {
            states.crouching = !states.crouching;
        }

        if (states.crouchCover)
        {
            states.crouching = true;
        }
    }

}
