using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public ItemType itemType;

    public enum ItemType
    {
        weapon,
        ammo,
        health,
        etc
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<ItemPickupBehaviour>())
        {
            other.transform.GetComponent<ItemPickupBehaviour>().itemToPickup = this;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<ItemPickupBehaviour>())
        {
            other.transform.GetComponent<ItemPickupBehaviour>().itemToPickup = null;
        }
    }
}
