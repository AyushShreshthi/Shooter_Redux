using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupBehaviour : MonoBehaviour
{
    public ItemBase itemToPickup;
    WeaponManager wm;
    Text Uitext;
    bool initItem;

    WeaponItem wpToPickup;
    AmmoItem amItemToPick;

    private void Start()
    {
        Uitext = CrosshairManager.GetInstance().pikItemsText;
        wm = GetComponent<WeaponManager>();
        Uitext.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckItemType();
        ActualPickup();
    }
    void WeaponActualPickup()
    {
        if (wpToPickup != null)
        {
            WeaponReferencebase targetWeapon = wm.ReturnWeaponWithID(wpToPickup.weaponId);

            if (targetWeapon != null)
            {
                wm.AvailableWeapons.Add(targetWeapon);

                if (wm.AvailableWeapons.Count >= wm.maxWeapons + 1)
                {
                    WeaponReferencebase prevWeapon = wm.ReturnCurrentWeapon();
                    wm.AvailableWeapons.Remove(prevWeapon);
                    wm.SwitchWeaponWithTargetWeapon(targetWeapon);

                    if (prevWeapon.pickablePrefab != null)
                    {
                        Instantiate(prevWeapon.pickablePrefab,
                            (transform.position + transform.forward * 2) + Vector3.up,
                            Quaternion.identity);
                    }
                }
            }
            Destroy(wpToPickup.gameObject);
            wpToPickup = null;
            itemToPickup = null;
        }
    }
    void AmmoItemActualPickup()
    {
        if (amItemToPick != null)
        {
            WeaponReferencebase targetWeapon = wm.ReturnWeaponWithID(amItemToPick.weaponId);

            if (targetWeapon != null)
            {
                if (targetWeapon.carryingAmmo < targetWeapon.maxAmmo)
                {
                    targetWeapon.carryingAmmo += amItemToPick.ammoAmount;

                    if (targetWeapon.carryingAmmo > targetWeapon.maxAmmo)
                    {
                        targetWeapon.carryingAmmo = targetWeapon.maxAmmo;
                    }

                    GetComponent<HandleShooting>().carryingAmmo = targetWeapon.carryingAmmo;

                    Destroy(amItemToPick.gameObject);
                    amItemToPick = null;
                    itemToPickup = null;
                }
            }
        }
    }
    private void ActualPickup()
    {
        if (Input.GetKey(KeyCode.X))
        {
            WeaponActualPickup();
            AmmoItemActualPickup();
        }
    }

    private void CheckItemType()
    {
        if (itemToPickup != null)
        {
            if (!initItem)
            {
                Uitext.gameObject.SetActive(true);

                switch (itemToPickup.itemType)
                {
                    case ItemBase.ItemType.weapon:
                        WeaponItemPicup();
                        break;

                    case ItemBase.ItemType.ammo:
                        AmmoItemPickup();
                        break;

                    default:
                        break;
                }
                initItem = true;
            }
        }
        else
        {
            if (initItem)
            {
                initItem = false;
                wpToPickup = null;
                amItemToPick = null;
                Uitext.gameObject.SetActive(false);
            }
        }
    }

    private void AmmoItemPickup()
    {
        amItemToPick = itemToPickup.GetComponent<AmmoItem>();

        WeaponReferencebase forWp = wm.ReturnWeaponWithID(amItemToPick.weaponId);

        if (wm.AvailableWeapons.Contains(forWp))
        {
            if (forWp.carryingAmmo < forWp.maxAmmo)
            {
                Uitext.text = "Press X to Pick Up Ammo For " + amItemToPick.weaponId;
            }
            else
            {
                Uitext.text = "Ammo for " + amItemToPick.weaponId + " is Full";

            }
        }
        else
        {
            Uitext.text = "Can't Pickup ammo for " + amItemToPick.weaponId;
        }
    }

    private void WeaponItemPicup()
    {
        wpToPickup = itemToPickup.GetComponent<WeaponItem>();
        string targetId = wpToPickup.weaponId;

        if (wm.AvailableWeapons.Count < wm.maxWeapons)
        {
            Uitext.text = "Press X to Pick Up " + targetId;
        }
        else
        {
            Uitext.text = "Press X to Switch " + wm.ReturnCurrentWeapon().waeponID + " with " + targetId;
        }
    }
}
