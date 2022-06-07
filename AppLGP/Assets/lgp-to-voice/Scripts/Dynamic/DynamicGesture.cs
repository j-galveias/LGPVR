using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DynamicGesture
{
    public string gestureName;
    public Gesture gesture;
    public List<Vector3> linePoints;
    public List<Vector3> fur;
    public List<Vector3> spherePoints;
    public TMP_Text text;
    public int currentPoint = 0;
    public OVRSkeleton skel;

    [HideInInspector]
    public float time = 0.0f;

    public DynamicGesture(string name, Gesture ges, List<Vector3> linePositions, List<Vector3> spherePos, List<Vector3> fur, TMP_Text text, OVRSkeleton ovrSkel)
    {
        gestureName = name;
        gesture = ges;
        linePoints = linePositions;
        this.fur = fur;
        spherePoints = spherePos;
        this.text = text;
        this.skel = ovrSkel;
    }
}
