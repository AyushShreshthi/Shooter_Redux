using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPosition : MonoBehaviour
{
    public BezierCurve curvePath;
    public bool blockPos1;
    public bool blockPos2;

    #region obsolete
    //public bool blockPos1;
    //public bool blockPos2;
    #endregion
    public float length;
    public CoverType coverType;
    public enum CoverType
    {
        full,
        half
    }
    private void Start()
    {
        curvePath = GetComponentInChildren<BezierCurve>();
        length = curvePath.length;
    }
}
