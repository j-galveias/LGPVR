using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DynamicGestureContainer : MonoBehaviour
{
    public DynamicGesture dynamicGesture;
    public GameObject initialPos;
    public List<Vector3> sphereList;
    public DynamicGestureRecognizer dgr;
    public Vector3 prevPos = Vector3.zero;
    public float timeToDie = 3.0f;
    public float timeLeft = 3.0f;

    [HideInInspector]
    public float time = 0.0f;

    private void Awake()
    {
        sphereList = new List<Vector3>();
        initialPos = new GameObject();
        initialPos.name = "initialPos";
    }

    void Start()
    {


    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;

        if(timeLeft <= 0)
        {
            Destroy(initialPos);
            dgr.possibleGestures.Remove(this.gameObject);
            Destroy(this.gameObject);
        }

        if (dynamicGesture != null && dynamicGesture.currentPoint == 0)
        {
            /*initialPos.transform.position = this.transform.position;
            initialPos.transform.forward = dgr.ancora.transform.forward;
            initialPos.transform.up = dgr.ancora.transform.up;
            initialPos.transform.right = dgr.ancora.transform.right;*/
            //initialPos.transform.rotation *= Quaternion.Euler(0, 180, 0);
            //initialPos.transform.rotation = this.transform.rotation;
            /*initialPos.transform.forward = this.transform.forward;
            initialPos.transform.up = this.transform.up;
            initialPos.transform.right = this.transform.right;*/

            dynamicGesture.currentPoint++;

            if(dynamicGesture.currentPoint < sphereList.Count)
            {
                this.transform.position = sphereList[dynamicGesture.currentPoint];
                timeLeft = timeToDie;
            }

        }
        if (dynamicGesture != null && dynamicGesture.currentPoint > 0)
        {
            //float dist1 = Math.Abs(Vector3.Distance(this.transform.position, dynamicGesture.skel.Bones[(int)OVRSkeleton.BoneId.Hand_Middle1].Transform.position));
            float dist1 = Math.Abs(Vector3.Distance(this.transform.position.normalized, dynamicGesture.skel.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position.normalized));
            Debug.Log(dist1);

            if(dist1 < 0.04f)
            {
                dynamicGesture.currentPoint++;
                if (dynamicGesture.currentPoint >= dynamicGesture.spherePoints.Count)
                {
                    if (dgr.message.text.Equals(""))
                    {
                        dgr.lastMessage = "";
                    }

                    string gestName;
                    if (dgr.testMode)
                    {
                        gestName = dynamicGesture.gesture.gestureName;
                    }
                    else
                    {
                        string[] split = dynamicGesture.gesture.gestureName.Split('-');
                        gestName = split[1];
                    }
                    dynamicGesture.text.text = dgr.lastMessage + gestName;
                    dgr.message.text = dgr.lastMessage + "<#ff4538>" + gestName + "</color>";
                    dgr.GestureCD();
                    Destroy(initialPos);
                    dgr.possibleGestures.Remove(this.gameObject);
                    Destroy(this.gameObject);
                    /*foreach (GameObject g in dgr.possibleGestures.Keys)
                    {
                        Destroy(g);
                    }
                    dgr.possibleGestures.Clear();
                    Destroy(this.gameObject);*/
                }
                else
                {
                    this.transform.position = sphereList[dynamicGesture.currentPoint];
                    timeLeft = timeToDie;
                }
            }

            
        }
    }

    private void OnDestroy()
    {
        //Destroy(initialPos);
        DestroyImmediate(initialPos);
    }

    private float DistCalculator(Vector3 v1, Vector3 v2) {
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2) + Mathf.Pow(v1.z - v2.z, 2));
    }
}