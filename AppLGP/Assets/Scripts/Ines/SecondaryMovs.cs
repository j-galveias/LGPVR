using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryMovs : MonoBehaviour
{
    protected Animator animator;
    public bool rotationActive = false;
    float state = 0;
    float elapsedTime = 0;
    float timeReaction = 2f;

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
            // Transform hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            // Debug.Log(hand.position);
            // animator.GetBoneTransform(HumanBodyBones.Head);
            // animator.SetBoneLocalRotation(HumanBodyBones.Head, Quaternion.Euler(hand.position));

            // Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            // spine.eulerAngles = hand.position;
            // animator.SetBoneLocalRotation(HumanBodyBones.Head, Quaternion.Euler(hand.position));

            // Move headdd!!
            // if (animator.GetBool("Animating") && !animator.GetBool("ExpFacial")) {
            //     Transform hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            //     Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
            //     if (hand.position.y>=2.3) {
            //         if (state < 0.2f)
            //         {
            //             elapsedTime += Time.deltaTime;
            //             state = Mathf.Lerp(0, 0.2f, elapsedTime * timeReaction);
            //             Debug.Log(state);
            //         }
            //         else
            //         {
            //             state = 0.2f;
            //             elapsedTime = 0;
            //         }
            //         animator.SetLookAtWeight(state);
            //         animator.SetLookAtPosition(new Vector3(hand.position.x, hand.position.y, hand.position.z));
            //     }
            //     // }
            //     else {
            //         if (state > 0f)
            //         {
            //             elapsedTime += Time.deltaTime;
            //             state = Mathf.Lerp(0, 0.2f, elapsedTime * timeReaction);
            //             state = 0.2f - state;
            //         }
            //         else
            //         {
            //             state = 0;
            //             elapsedTime = 0;
            //         }
            //         animator.SetLookAtWeight(state);
            //         animator.SetLookAtPosition(new Vector3(hand.position.x,  hand.position.y, hand.position.z));
            //     }
            // }
        }
    }
}