using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Novo Gesto", menuName = "Gesto", order = 1)]
public class Gesto : ScriptableObject
{
    public string gestureName;
    public List<Vector3> wristRot;
    public List<Vector3> positionsPerFinger; // Relative to hand
    public List<Vector3> positionsPerBone; // Relative to hand
    public List<float> distanceBetweenFingertipsAndWrist; // Relative to hand base
    public List<float> angleBetweenAdjacentFingertips;
    public List<float> distanceBetweenAdjacentFingertips;
    public UnityEvent onRecognized;
    public TMP_Text text;

    [HideInInspector]
    public float time = 0.0f;

    public Gesto(string name, List<Vector3> rot, List<Vector3> positions, List<Vector3> pob, List<float> dbfaw, List<float> abaf, List<float> dbaf, UnityEvent onRecognized, TMP_Text text)
    {
        gestureName = name;
        wristRot = rot;
        positionsPerFinger = positions;
        positionsPerBone = pob;
        distanceBetweenFingertipsAndWrist = dbfaw;
        angleBetweenAdjacentFingertips = abaf;
        distanceBetweenAdjacentFingertips = dbaf;
        this.text = text;
        this.onRecognized = onRecognized;
    }

    public Gesto(string name, List<Vector3> rot, List<Vector3> positions, List<Vector3> pob, List<float> dbfaw, List<float> abaf, List<float> dbaf, TMP_Text text)
    {
        gestureName = name;
        wristRot = rot;
        positionsPerFinger = positions;
        positionsPerBone = pob;
        distanceBetweenFingertipsAndWrist = dbfaw;
        angleBetweenAdjacentFingertips = abaf;
        distanceBetweenAdjacentFingertips = dbaf;
        this.text = text;
        onRecognized = new UnityEvent();
        onRecognized.AddListener(onRec);
    }

    public void onRec()
    {
        text.text = "Found " + gestureName;
    }
}