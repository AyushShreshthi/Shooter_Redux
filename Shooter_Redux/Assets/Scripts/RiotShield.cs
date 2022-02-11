using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiotShield : MonoBehaviour
{
    Animator anim;
    StateManager states;

    public Transform leftShoulder;
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        states = GetComponentInParent<StateManager>();
    }
    private void OnEnable()
    {
        if (states == null)
            states = GetComponentInParent<StateManager>();

        states.dontRun = true;
    }
    private void OnDisable()
    {
        states.dontRun = false;
    }
    private void Update()
    {
        if (leftShoulder == null)
            leftShoulder = transform.root.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftShoulder);


        transform.position = leftShoulder.position;

        anim.SetBool("Aim", states.aiming);
    }
}
