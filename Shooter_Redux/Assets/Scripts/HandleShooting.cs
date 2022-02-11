using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleShooting : MonoBehaviour
{
    StateManager states;
    [HideInInspector]
    public Animator weaponAnim;
    [HideInInspector]
    public Animator modelAnim;
    [HideInInspector]
    public float fireRate;
    float timer;
    [HideInInspector]
    public Transform bulletSpawnPoint;
    [HideInInspector]
    public GameObject smokeParticle;
    [HideInInspector]
    public ParticleSystem[] muzzle;

    public GameObject casingPrefab;   // we can add diff. or diff. weapon
    [HideInInspector]
    public Transform caseSpawn;

    WeaponManager weaponManager;

    public int magazineBullets = 0;
    public int curBullets = 30;
    public int carryingAmmo;

    private void Start()
    {
        states = GetComponent<StateManager>();
        weaponManager = GetComponent<WeaponManager>();

    }
    bool shoot;
    bool dontShoot;
    bool emptyGun;

    private void Update()
    {
        shoot = states.shoot;

        if (modelAnim != null)
        {
            modelAnim.SetBool("Shoot", false);

            if (curBullets > 0)
            {
                modelAnim.SetBool("Empty", false);
            }
            else
            {
                modelAnim.SetBool("Empty", true);
            }
        }
        if (shoot)
        {
            if (timer <= 0)
            {
                if (modelAnim != null)
                {
                    modelAnim.SetBool("Shoot", false);

                }
                weaponAnim.SetBool("Shoot", false);

                if (curBullets > 0)
                {
                    emptyGun = false;
                    states.audioManager.PlayGunSound();

                    if (modelAnim != null)
                    {
                        modelAnim.SetBool("Shoot", true);
                    }

                    weaponAnim.SetBool("Shoot", true);

                    GameObject go = Instantiate(casingPrefab, caseSpawn.position, caseSpawn.rotation);
                    Rigidbody rig = go.GetComponent<Rigidbody>();
                    rig.AddForce(transform.right.normalized * 2 + Vector3.up * 1.3f, ForceMode.Impulse);
                    rig.AddRelativeTorque(go.transform.right * 1.5f, ForceMode.Impulse);

                    states.actualShooting = true;

                    for(int i = 0; i < muzzle.Length; i++)
                    {
                        muzzle[i].Emit(1);
                        muzzle[i].Play();
                    }

                    RaycastShoot();

                    curBullets--;
                }
                else
                {
                    
                    if (emptyGun)
                    {
                        if (carryingAmmo > 0)
                        {
                            states.handleAnim.StartReload();

                            int targetBullets = 0;
                            if (magazineBullets < carryingAmmo)
                            {
                                targetBullets = magazineBullets;
                            }
                            else
                            {
                                targetBullets = carryingAmmo;
                            }

                            carryingAmmo -= targetBullets;

                            curBullets = targetBullets;

                            states.weaponManager.ReturnCurrentWeapon().weaponStats.curBullets = curBullets;
                            states.weaponManager.ReturnCurrentWeapon().carryingAmmo = carryingAmmo;

                        }
                        else
                        {
                            states.audioManager.PlayEffect("empty_gun");
                        }
                    }
                    else
                    {
                        states.audioManager.PlayEffect("empty_gun");
                        emptyGun = true;
                    }
                }
                timer = fireRate;
            }
            else
            {
                states.actualShooting = false;

                weaponAnim.SetBool("Shoot", true);
                timer -= Time.deltaTime;
            }
        }
        else
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            else
                timer = 0;

            weaponAnim.SetBool("Shoot", false);

            states.actualShooting = false;
        }
    }

    private void RaycastShoot()
    {
        Vector3 direction = states.lookHitPosition - bulletSpawnPoint.position;
        RaycastHit hit;

        if(Physics.Raycast(bulletSpawnPoint.position,direction,out hit, 100, states.layerMask))
        {
            GameObject go = Instantiate(smokeParticle, hit.point, Quaternion.identity) as GameObject;
            go.transform.LookAt(bulletSpawnPoint.position);


        }

    }
}
