using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    public bool lookIK = true;
    public bool leftHandIK = true;
    public bool rightHandIK = true;
    public bool leftFootIK = true;
    public bool rightFootIK = true;

    public Transform lookObj;
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftFoot;
    public Transform rightFoot;

    private Manager manager;
    private Animator animator;

    //private float weightFactor = 0.5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //float weight = weightFactor * controller.layers[manager.GetSignLayer()].defaultWeight;
        float weight = 1;

        if  (lookIK)
        {
            animator.SetLookAtWeight(weight);
            animator.SetLookAtPosition(lookObj.position);
        }

        if (leftHandIK)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
        }

        if (rightHandIK)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
        }

        if (leftFootIK)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weight);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFoot.position);
        }

        if (rightFootIK)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, weight);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot.position);
        }
    }
}
