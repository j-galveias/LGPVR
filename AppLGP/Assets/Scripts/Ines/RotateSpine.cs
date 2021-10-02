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
    public Transform spine;
    // Transform RightHand;
    Quaternion rot = new Quaternion(0,0,0,0);

    Quaternion lastAimRotation;

    // Transform LeftHand;

    void Start()
    {
        // Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        animator = GetComponent<Animator>();
        state = 0;
        // spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        // RightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        // LeftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
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
            if (animator.GetBool("Animating")) {
                Transform LeftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
                // Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
                Transform RightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

                // rot = Quaternion.FromToRotation(spine.position, RightHand.position + LeftHand.position);

                // spine.transform.rotation = rot * spine.transform.rotation;

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

                // Debug.Log("z position: " + RightHand.position.z);

                // Quaternion rot = Quaternion.FromToRotation(spine.position, RightHand.position);

                // Debug.Log("angles: " + Quaternion.Slerp(lastAimRotation, RightHand.rotation, Time.deltaTime * 1f));

                // spine.rotation = Quaternion.Slerp(lastAimRotation, RightHand.rotation, Time.deltaTime * 1f);
                // lastAimRotation = spine.rotation;

                // transform.LookAt(RightHand.position, Vector3.up);

                // animator.bodyRotation = RightHand.rotation;


                if (RightHand.position.x>=-18.6 && RightHand.position.z >= -1.80) {
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


                    // Quaternion rot = Quaternion.FromToRotation(spine.position, RightHand.position);
                    // float angle = Quaternion.Angle(spine.rotation, RightHand.rotation);

                    // transform.Rotate(Vector3.down, -angle*state);

                    // spine.rotation = rot * spine.rotation;´

                    // spine.rotation = Quaternion.Slerp(lastAimRotation, RightHand.rotation, Time.deltaTime * 1f);
                    // lastAimRotation = spine.rotation;
                    
                    animator.SetLookAtWeight(state, 0.3f, 0f);
                    // // Debug.Log("LeftHand.position");
                    // // Debug.Log(LeftHand.position);
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
                    // animator.SetLookAtPosition(spine.position);
                }
            }
        }
    }
}