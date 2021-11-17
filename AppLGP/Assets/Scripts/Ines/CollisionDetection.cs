using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour 
 {
    protected Animator animator;
    public bool isCollided;
    ContactPoint[] collisionPoints;

    float state = 0;
    float elapsedTime = 0;
    float timeReaction = 2f;

    Vector3 difference = new Vector3();
    Vector3 defaultPos = new Vector3();

    public bool IsCollided
    {
        get {return isCollided; }
    }

    void Start() {
        animator = GetComponentInParent<Animator>();
        state = 0;
        defaultPos = animator.GetBoneTransform(HumanBodyBones.RightHand).position;
    }
    
    void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.tag == "RHand")
        {
            isCollided = true;
            // Debug.Log("collisionnnnnnnnnnnnnnn"); //collision.contacts[0]
            collisionPoints = collision.contacts;
        }
    }

    void OnCollisionStay(Collision collision) 
    {
        if (collision.gameObject.tag == "RHand")
        {
            isCollided = true;
            // Debug.Log("collisionnnnnnnnnnnnnnn"); //collision.contacts[0]
            collisionPoints = collision.contacts;
        }
    }

    public void OnAnimatorIK() {
        if (animator.GetBool("Animating")) {
            if (isCollided){
                foreach (ContactPoint contact in collisionPoints)
                {
                    // Debug.Log(contact.point);
                    // Debug.Log(contact.normal);
                    // Debug.Log(contact.separation);
                    if (state < 0.03f)
                    {
                        elapsedTime += Time.deltaTime;
                        state = Mathf.Lerp(0, 0.03f, elapsedTime * timeReaction);
                    }
                    else
                    {
                        state = 0.03f;
                        elapsedTime = 0;
                    }
                    // rightHandPosition.position = Vector3.Lerp(rightHandPosition.position, rightHandPosition.position+difference, state);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, state);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, contact.point - contact.normal);
                }
            } else {
                if (state > 0f)
                {
                    elapsedTime += Time.deltaTime;
                    state = Mathf.Lerp(0, 0.03f, elapsedTime * timeReaction);
                    state = 0.03f - state;

                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, state);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, defaultPos); 
                }
                else
                {
                    state = 0;
                    elapsedTime = 0;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, defaultPos);
                }
            }
        } else {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPosition(AvatarIKGoal.RightHand, defaultPos);            
        }
    }
    
    public void ResetCollision()
    {
        isCollided = false;
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        animator.SetIKPosition(AvatarIKGoal.RightHand, defaultPos); 
    }

}