using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameobject : MonoBehaviour
{
    public GameObject targetObj;
    private void OnEnable()
    {
        targetObj.SetActive(true);
    }
    private void OnDisable()
    {
        targetObj.SetActive(false);
    }
}
