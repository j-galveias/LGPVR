using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VerifyHandTracking : MonoBehaviour
{
    public Transform RotPoint;
    public Transform CenterEye;

    public Image Limage;
    public Image Rimage;
    public Text Ltext;
    public Text Rtext;
    public OVRHand Lhand;
    public OVRHand Rhand;
    public OVRSkeleton Lskeleton;
    public OVRSkeleton Rskeleton;
    public GameObject Lwrist;
    public GameObject Rwrist;

    public GameObject LThumb_tip;
    public GameObject Lindex_tip;
    public GameObject Lmiddl_tip;
    public GameObject Lringf_tip;
    public GameObject Lpinky_tip;
    public GameObject RThumb_tip;
    public GameObject Rindex_tip;
    public GameObject Rmiddl_tip;
    public GameObject Rringf_tip;
    public GameObject Rpinky_tip;

    public GameObject LThumb_distal;
    public GameObject Lindex_distal;
    public GameObject Lmiddl_distal;
    public GameObject Lringf_distal;
    public GameObject Lpinky_distal;
    public GameObject RThumb_distal;
    public GameObject Rindex_distal;
    public GameObject Rmiddl_distal;
    public GameObject Rringf_distal;
    public GameObject Rpinky_distal;

    public GameObject Lindex_inter;
    public GameObject Lmiddl_inter;
    public GameObject Lringf_inter;
    public GameObject Lpinky_inter;
    public GameObject Rindex_inter;
    public GameObject Rmiddl_inter;
    public GameObject Rringf_inter;
    public GameObject Rpinky_inter;

    public GameObject LThumb_prox;
    public GameObject Lindex_prox;
    public GameObject Lmiddl_prox;
    public GameObject Lringf_prox;
    public GameObject Lpinky_prox;
    public GameObject RThumb_prox;
    public GameObject Rindex_prox;
    public GameObject Rmiddl_prox;
    public GameObject Rringf_prox;
    public GameObject Rpinky_prox;

    public GameObject LThumb_meta;
    public GameObject Lpinky_meta;
    public GameObject RThumb_meta;
    public GameObject Rpinky_meta;

    public GameObject LThumb_trap;
    public GameObject RThumb_trap;


    // Update is called once per frame
    void Update()
    {
        //Limage.color = Lhand.IsTracked ? Color.green : Color.red;
        Rimage.color = Rhand.IsTracked ? Color.green : Color.red;

        RotPoint.position = CenterEye.position;
        RotPoint.eulerAngles = new Vector3(0, CenterEye.eulerAngles.y, 0);
            
        //Ltext.text = Lhand.HandConfidence.ToString();
        //Rtext.text = Rhand.HandConfidence.ToString();

        /*var inicio = Rskeleton.GetCurrentStartBoneId() + 2;
        for (int i = inicio; i < 19; i++)
        {
            if(2<=i<=5)
            {
                LThumb.transform.GetChild(i).GetComponent<TMP_Text>().text = Lskeleton.Bones.ge;
            }
        }*/
        //Debug.Log(Lskeleton.Bones[5].Transform.localPosition);

        if(Lskeleton.Bones.Count > 0 && Rskeleton.Bones.Count > 0)
        {
            /***
             *      Wrists
             */
            #region
            Lwrist.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Start].Transform.position;
            Lwrist.transform.rotation = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Start].Transform.rotation;
            Rwrist.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Start].Transform.position;
            Rwrist.transform.rotation = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Start].Transform.rotation;

            //Debug.Log(Vector3.SignedAngle(Vector3.up, Rwrist.transform.up, Vector3.forward));
            #endregion
            /***
             *      Fingertips
             */
            #region
            LThumb_tip.transform.position = Lskeleton.Bones[(int) OVRPlugin.BoneId.Hand_ThumbTip].Transform.position;
            Lindex_tip.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform.position;
            Lmiddl_tip.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_MiddleTip].Transform.position;
            Lringf_tip.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_RingTip].Transform.position;
            Lpinky_tip.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_PinkyTip].Transform.position;
            RThumb_tip.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_ThumbTip].Transform.position;
            Rindex_tip.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip].Transform.position;
            Rmiddl_tip.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_MiddleTip].Transform.position;
            Rringf_tip.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_RingTip].Transform.position;
            Rpinky_tip.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_PinkyTip].Transform.position;
            #endregion
            /***
             *      Distal
             */
            #region
            LThumb_distal.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Thumb3].Transform.position;
            Lindex_distal.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Index3].Transform.position;
            Lmiddl_distal.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Middle3].Transform.position;
            Lringf_distal.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Ring3].Transform.position;
            Lpinky_distal.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Pinky3].Transform.position;
            RThumb_distal.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Thumb3].Transform.position;
            Rindex_distal.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Index3].Transform.position;
            Rmiddl_distal.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Middle3].Transform.position;
            Rringf_distal.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Ring3].Transform.position;
            Rpinky_distal.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Pinky3].Transform.position;
            #endregion
            /***
             *      Intermediate
             */
            #region
            Lindex_inter.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Index2].Transform.position;
            Lmiddl_inter.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Middle2].Transform.position;
            Lringf_inter.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Ring2].Transform.position;
            Lpinky_inter.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Pinky2].Transform.position;
            Rindex_inter.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Index2].Transform.position;
            Rmiddl_inter.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Middle2].Transform.position;
            Rringf_inter.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Ring2].Transform.position;
            Rpinky_inter.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Pinky2].Transform.position;
            #endregion
            /***
             *      Proximal
             */
            #region
            LThumb_prox.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Thumb2].Transform.position;
            Lindex_prox.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Index1].Transform.position;
            Lmiddl_prox.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Middle1].Transform.position;
            Lringf_prox.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Ring1].Transform.position;
            Lpinky_prox.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Pinky1].Transform.position;
            RThumb_prox.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Thumb2].Transform.position;
            Rindex_prox.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Index1].Transform.position;
            Rmiddl_prox.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Middle1].Transform.position;
            Rringf_prox.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Ring1].Transform.position;
            Rpinky_prox.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Pinky1].Transform.position;
            #endregion
            /***
             *      Metacarpal
             */
            #region
            LThumb_meta.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Thumb1].Transform.position;
            Lpinky_meta.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Pinky0].Transform.position;
            RThumb_meta.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Thumb1].Transform.position;
            Rpinky_meta.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Pinky0].Transform.position;
            #endregion
            /***
             *      Trapezium
             */
            #region
            LThumb_trap.transform.position = Lskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Thumb0].Transform.position;
            RThumb_trap.transform.position = Rskeleton.Bones[(int)OVRPlugin.BoneId.Hand_Thumb0].Transform.position;
            #endregion
        }
    }
}
