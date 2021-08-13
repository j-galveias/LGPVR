using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSpine : MonoBehaviour
{
    protected Animator animator;
    public bool rotationActive = false;
    float state = 0;
    float elapsedTime = 0;
    float timeReaction = 2f;
    float startPosition = 0f;

    void Start()
    {
        // Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        animator = GetComponent<Animator>();
        state = 0;
        // startPosition = spine.position.x;

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
            if (animator.GetBool("Animating") && !animator.GetBool("ExpFacial0")) {
                Transform RightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                Transform LeftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
                Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);

                // Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);

                // spine.transform.Rotate(hand.position);
                // Quaternion angle = Quaternion.FromToRotation(spine.position, hand.position);
                // // Quaternion rotation = Quaternion.Euler(angle, angle, 0);
                // Debug.Log("angle");
                // Debug.Log(angle);

                // animator.SetBoneLocalRotation(HumanBodyBones.Spine, angle);

                // spine.localRotation = Quaternion.LookRotation(hand.rotation, spine.up);

                Vector3 handsMiddle = (RightHand.position + LeftHand.position)/2;

                Vector3 fromToPosition = handsMiddle - spine.position; //+ para rodar o lado

                // Debug.Log("handdddd");
                // Debug.Log(handsMiddle);

                //  if (LeftHand.position.x>=-18.6) {
                //     if (state2 <0.2f)
                //     {
                //         elapsedTime2 += Time.deltaTime;
                //         state2 = Mathf.Lerp(0,0.2f, elapsedTime2 * timeReaction);
                //         Debug.Log(state2);
                //     }
                //     else
                //     {
                //         state2 =0.2f;
                //         elapsedTime2 = 0;
                //     }
                //     animator.SetLookAtWeight(state2, 0.2f, 0f);
                //     animator.SetLookAtPosition(fromToPosition2);
                // }

                Debug.Log("righthandpos: " + RightHand.position.y);


                if (RightHand.position.x>=-18.6 && RightHand.position.y<=2.2) {
                    if (state <0.3f)
                    {
                        elapsedTime += Time.deltaTime;
                        state = Mathf.Lerp(0,0.3f, elapsedTime * timeReaction);
                        Debug.Log(state);
                    }
                    else
                    {
                        state =0.3f;
                        elapsedTime = 0;
                    }
                    animator.SetLookAtWeight(state, 0.3f, 0f);
                    // Debug.Log("LeftHand.position");
                    // Debug.Log(LeftHand.position);
                    animator.SetLookAtPosition(fromToPosition);
                }
                else {
                    if (state > 0f)
                    {
                        elapsedTime += Time.deltaTime;
                        state = Mathf.Lerp(0,0.3f, elapsedTime * timeReaction);
                        state = 0.3f - state;
                    }
                    else
                    {
                        state = 0;
                        elapsedTime = 0;
                    }
                    animator.SetLookAtWeight(state, 0.3f, 0f);
                    animator.SetLookAtPosition(fromToPosition);
                }
            }
        }
    }
}