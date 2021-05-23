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
    public Vector3 difference = new Vector3();
    public Vector3 defaultPosition = new Vector3();
    public bool last = false;
    public bool first = false;
    public int characters;

    float state = 0;
    float elapsedTime = 0;
    float timeReaction = 2f;

    CollisionDetection collisionDetection;

    void Start()
    {
        animator = GetComponent<Animator>();
        state = 0;
        defaultPosition = rightHandPosition.position;
        collisionDetection = gameObject.GetComponentInChildren<CollisionDetection>();
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        bool collision = false;
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
                         state = Mathf.Lerp(0, 1, elapsedTime * timeReaction);
                     }
                     else
                     {
                        state = 1.0f;
                        elapsedTime = 0;
                        // if (!first) rightHandPosition.position = Vector3.Lerp(rightHandPosition.position, new Vector3(rightHandPosition.position.x + 0.0002f,rightHandPosition.position.y -0.0001f,rightHandPosition.position.z), state);
                        // rightHandPosition.position = Vector3.Lerp(rightHandPosition.position, new Vector3(rightHandPosition.position.x + 0.0005f,rightHandPosition.position.y +0.00003f,rightHandPosition.position.z), state);
                     }
                    if (positionActive)
                    {   
                        if (!first) rightHandPosition.position = Vector3.Lerp(rightHandPosition.position, rightHandPosition.position+difference, state);
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, state);
                        // animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        // rightHandPosition.position = new Vector3(rightHandPosition.position.x + 0.003f,rightHandPosition.position.y + 0.003f,rightHandPosition.position.z);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition.position);
                        // if (!first && !last) rightHandPosition.position = Vector3.Lerp(rightHandPosition.position, new Vector3(rightHandPosition.position.x + 0.0005f,rightHandPosition.position.y -0.0001f,rightHandPosition.position.z), state);
                        // rightHandPosition.position = Vector3.Lerp(rightHandPosition.position, new Vector3(rightHandPosition.position.x + 0.0003f,rightHandPosition.position.y -0.0001f,rightHandPosition.position.z), state);

                        // rightHandPosition.position = new Vector3(rightHandPosition.position.x +0.003f,rightHandPosition.position.y +0.0003f,rightHandPosition.position.z);
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
                    state = Mathf.Lerp(0, 1, elapsedTime * timeReaction);
                    state = 1 - state;

                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, state);
                    //  animator.SetIKRotationWeight(AvatarIKGoal.RightHand, state);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition.position);
                    //  animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                    collision = false;

                }
                else
                {
                    state = 0;
                    elapsedTime = 0;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                //  animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                //  animator.SetLookAtWeight(0);
                    collision = true;
                }
 
             }
                // animator.SetFloat("HandWeight", 0);
                // animator.SetIKPositionWeight(AvatarIKGoal.RightHand, animator.GetFloat("HandWeight"));
                // animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                // animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                // animator.SetLookAtWeight(0);

            // if(collisionDetection.isCollided && !ikActive && collision) collisionDetection.OnAnimatorIK();
        }
    }
}
