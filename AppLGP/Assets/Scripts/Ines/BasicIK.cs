using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicIK : MonoBehaviour
{
    protected Animator animator;

    public bool ikActive = false;
    public bool rotationActive = false;
    public bool positionActive = true;
    public Transform rightHandPosition = null;
    public Transform rightHandRotation = null;
    public Transform lookObj = null;

    float state = 0;
    float elapsedTime = 0;
    public float timeReaction = 0.05f;

    void Start()
    {
        animator = GetComponent<Animator>();
        state = 0;
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {

                // Set the look __target position__, if one has been assigned
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandPosition != null)
                {
                     if (state < 1.0f)
                     {
                         elapsedTime += Time.deltaTime;
                         state = Mathf.Lerp(0, 1, elapsedTime / timeReaction);
                     }
                     else
                     {
                         state = 1.0f;
                         elapsedTime = 0;
                     }
                    if (positionActive)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, state);
                        // animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition.position);
                    }
                }

                if (rightHandRotation != null)
                {
                    if (rotationActive)
                    {
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandRotation.rotation);
                    }
                }
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                if (state > 0f)
                 {
                     elapsedTime += Time.deltaTime;
                     state = Mathf.Lerp(0, 1, elapsedTime / timeReaction);
                     state = 1 - state;
 
                     animator.SetIKPositionWeight(AvatarIKGoal.RightHand, state);
                    //  animator.SetIKRotationWeight(AvatarIKGoal.RightHand, state);
 
                     animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition.position);
                    //  animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
 
                 }
                 else
                 {
                     state = 0;
                     elapsedTime = 0;
                     animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    //  animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    //  animator.SetLookAtWeight(0);
 
                 }
 
             }
                // animator.SetFloat("HandWeight", 0);
                // animator.SetIKPositionWeight(AvatarIKGoal.RightHand, animator.GetFloat("HandWeight"));
                // animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                // animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                // animator.SetLookAtWeight(0);
        }
    }
}
