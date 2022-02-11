using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public int maxWeapons = 2;
    public List<WeaponReferencebase> AvailableWeapons = new List<WeaponReferencebase>();

    public int weaponIndex;
    public List<WeaponReferencebase> Weapons = new List<WeaponReferencebase>();
    WeaponReferencebase currentWeapon;
    IKhandler ikHandler;
    HandleShooting handleShooting;
    CharacterAudioManager audioManager;

    private void Start()
    {
        ikHandler = GetComponent<IKhandler>();
        handleShooting = GetComponent<HandleShooting>();
        audioManager = GetComponent<CharacterAudioManager>();

        AvailableWeapons.Add(Weapons[4]);
        weaponIndex = 0;

        CloseAllWeapons();
        SwitchWeapon(weaponIndex);
    }

    private void SwitchWeapon(int desiredIndex)
    {
        if (desiredIndex > AvailableWeapons.Count - 1)
        {
            desiredIndex = 0;
            weaponIndex = 0;
        }

        if (currentWeapon != null)
        {
            currentWeapon.weaponModel.SetActive(false);
            currentWeapon.ikHolder.SetActive(false);

        }

        WeaponReferencebase newWeapon = AvailableWeapons[desiredIndex];

        ikHandler.righthadnIkTarget = newWeapon.rightHandTarget;
        ikHandler.righthadnIkRotation = newWeapon.rightHandRotation;
        ikHandler.leftHandIkTarget = newWeapon.leftHandTarget;

        if (newWeapon.lookTarget)
        {
            ikHandler.overrideLookTarget = newWeapon.lookTarget;
        }
        else
        {
            ikHandler.overrideLookTarget = null;
        }

        if (newWeapon.modelAnimator)
        {
            handleShooting.modelAnim = newWeapon.modelAnimator;
        }
        else
        {
            handleShooting.modelAnim = null;
        }
        if (newWeapon.leftElbowTarget) { ikHandler.leftElbowTarget = newWeapon.leftElbowTarget; }
        else { ikHandler.leftElbowTarget = null; }

        if (newWeapon.rightElbowTarget) { ikHandler.rightElbowTarget = newWeapon.rightElbowTarget; }
        else { ikHandler.rightElbowTarget = null; }

        ikHandler.LHIK_dis_notAiming = newWeapon.dis_LHIK_notAiming;

        if (newWeapon.dis_LHIK_notAiming)
            ikHandler.leftHandIkWeight = 0;

        handleShooting.fireRate = newWeapon.weaponStats.fireRate;
        handleShooting.weaponAnim = newWeapon.ikHolder.GetComponent<Animator>();
        handleShooting.bulletSpawnPoint = newWeapon.bulletSpawner;
        handleShooting.curBullets = newWeapon.weaponStats.curBullets;
        handleShooting.magazineBullets = newWeapon.weaponStats.maxBullets;
        handleShooting.caseSpawn = newWeapon.casingSpawner;
        handleShooting.muzzle = newWeapon.muzzle;

        audioManager.gunSounds.clip = newWeapon.weaponStats.shootSound;

        handleShooting.carryingAmmo = newWeapon.carryingAmmo;

        weaponIndex = desiredIndex;
        newWeapon.weaponModel.SetActive(true);
        newWeapon.ikHolder.SetActive(true);

        currentWeapon = newWeapon;
    }

    void CloseAllWeapons()
    {
        for(int i = 0; i < Weapons.Count; i++)
        {
            ParticleSystem[] muzzles = Weapons[i].weaponModel.GetComponentsInChildren<ParticleSystem>();
            Weapons[i].muzzle = muzzles;

            Weapons[i].weaponModel.SetActive(false);
            Weapons[i].ikHolder.SetActive(false);
        }
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            SwitchWeapon(weaponIndex);

            if (weaponIndex < AvailableWeapons.Count - 1)
            {
                weaponIndex++;
            }
            else
            {
                weaponIndex = 0;
            }
        }
    }

    public WeaponReferencebase ReturnWeaponWithID(string weaponID)
    {
        WeaponReferencebase retVal = null;

        for(int i = 0; i < Weapons.Count; i++)
        {
            if (string.Equals(Weapons[i].waeponID, weaponID))
            {
                retVal = Weapons[i];
                break;
            }
        }
        return retVal;
    }
    public WeaponReferencebase ReturnCurrentWeapon()
    {
        return currentWeapon;
    }

    public void SwitchWeaponWithTargetWeapon(WeaponReferencebase targetWeapon)
    {
        
        if (currentWeapon != null)
        {
            currentWeapon.weaponModel.SetActive(false);
            currentWeapon.ikHolder.SetActive(false);

        }

        WeaponReferencebase newWeapon = targetWeapon;

        ikHandler.righthadnIkTarget = newWeapon.rightHandTarget;
        ikHandler.righthadnIkRotation = newWeapon.rightHandRotation;
        ikHandler.leftHandIkTarget = newWeapon.leftHandTarget;

        if (newWeapon.lookTarget)
        {
            ikHandler.overrideLookTarget = newWeapon.lookTarget;
        }
        else
        {
            ikHandler.overrideLookTarget = null;
        }

        if (newWeapon.modelAnimator)
        {
            handleShooting.modelAnim = newWeapon.modelAnimator;
        }
        else
        {
            handleShooting.modelAnim = null;
        }
        if (newWeapon.leftElbowTarget) { ikHandler.leftElbowTarget = newWeapon.leftElbowTarget; }
        else { ikHandler.leftElbowTarget = null; }

        if (newWeapon.rightElbowTarget) { ikHandler.rightElbowTarget = newWeapon.rightElbowTarget; }
        else { ikHandler.rightElbowTarget = null; }

        ikHandler.LHIK_dis_notAiming = newWeapon.dis_LHIK_notAiming;

        if (newWeapon.dis_LHIK_notAiming)
            ikHandler.leftHandIkWeight = 0;

        handleShooting.fireRate = newWeapon.weaponStats.fireRate;
        handleShooting.weaponAnim = newWeapon.ikHolder.GetComponent<Animator>();
        handleShooting.bulletSpawnPoint = newWeapon.bulletSpawner;
        handleShooting.curBullets = newWeapon.weaponStats.curBullets;
        handleShooting.magazineBullets = newWeapon.weaponStats.maxBullets;
        handleShooting.caseSpawn = newWeapon.casingSpawner;
        handleShooting.muzzle = newWeapon.muzzle;

        audioManager.gunSounds.clip = newWeapon.weaponStats.shootSound;

        handleShooting.carryingAmmo = newWeapon.carryingAmmo;

        //weaponIndex = desiredIndex;
        newWeapon.weaponModel.SetActive(true);
        newWeapon.ikHolder.SetActive(true);

        currentWeapon = newWeapon;
    }
}

[System.Serializable]
public class WeaponReferencebase
{
    public string waeponID;
    public GameObject weaponModel;
    public Animator modelAnimator;
    public GameObject ikHolder;
    public Transform rightHandTarget;
    public Transform rightHandRotation;
    public Transform leftHandTarget;
    public Transform lookTarget;
    public ParticleSystem[] muzzle;
    public Transform bulletSpawner;
    public Transform casingSpawner;
    public WeaponStats weaponStats;
    public Transform rightElbowTarget;
    public Transform leftElbowTarget;

    public bool dis_LHIK_notAiming;

    public int carryingAmmo = 30;
    public int maxAmmo = 30;
    public GameObject pickablePrefab;

}
[System.Serializable]
public class WeaponStats
{
    public int curBullets;
    public int maxBullets;
    public float fireRate;
    public AudioClip shootSound;
}
