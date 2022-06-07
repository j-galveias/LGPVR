using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public enum HandToTrack
{
    Left,
    Right
}

public class Draw : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToTrackMovement;

    [SerializeField, Range(0, 1.0f)]
    private float minDistanceBeforeNewPoint = 0.2f;

    public Vector3 prevPointDistance = Vector3.zero;

    [SerializeField]
    private float minFingerPinchDownStrength = 0.5f;

    [SerializeField, Range(0, 1.0f)]
    private float lineDefaultWidth = 0.010f;

    public int positionCount = 0;

    private List<LineRenderer> lines = new List<LineRenderer>();

    public LineRenderer currentLineRender;

    [SerializeField]
    private Color defaultColor = Color.yellow;

    [SerializeField]
    private GameObject editorObjectToTrackMovement;

    [SerializeField]
    private bool allowEditorControls = true;

    [SerializeField]
    private Material defaultLineMaterial;

    private bool IsPinchingReleased = false;

    #region Oculus Types

    public  OVRHand ovrHand;

    public OVRSkeleton ovrSkeleton;

    private OVRBone boneToTrack;
    #endregion

    public GameObject ball;

    public Vector3 prevBallDistance = Vector3.zero;

    public bool recording = false;

    public List<Vector3> spherePos = new List<Vector3>();
    public List<Vector3> fur = new List<Vector3>();
    public List<Vector3> linePos = new List<Vector3>();

    public GameObject firstPoint;
    public GameObject ancora;

    public int spaceCount = 0;

    void Awake()
    {
        firstPoint = new GameObject();
#if UNITY_EDITOR

        // if we allow editor controls use the editor object to track movement because oculus
        // blocks the movement of LeftControllerAnchor and RightControllerAnchor
        if (allowEditorControls)
        {
            objectToTrackMovement = editorObjectToTrackMovement != null ? editorObjectToTrackMovement : objectToTrackMovement;
        }

#endif
        // get initial bone to track
        /*boneToTrack = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Middle1)
                .SingleOrDefault();*/
        boneToTrack = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_IndexTip)
                .SingleOrDefault();

        // add initial line rerderer
        AddNewLineRenderer();
    }

    public void AddNewLineRenderer()
    {
        positionCount = 0;

        GameObject go = new GameObject($"LineRenderer_{lines.Count}");
        go.transform.parent = objectToTrackMovement.transform.parent;
        go.transform.position = objectToTrackMovement.transform.position;

        LineRenderer goLineRenderer = go.AddComponent<LineRenderer>();
        goLineRenderer.startWidth = lineDefaultWidth;
        goLineRenderer.endWidth = lineDefaultWidth;
        goLineRenderer.useWorldSpace = true;
        goLineRenderer.material = defaultLineMaterial;
        goLineRenderer.positionCount = 1;
        goLineRenderer.numCapVertices = 5;

        currentLineRender = goLineRenderer;

        lines.Add(goLineRenderer);
    }

    void Update()
    {
        if (!ovrSkeleton.IsDataHighConfidence)
        {
            return;
        }
        if (boneToTrack == null)
        {
            /*boneToTrack = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Middle1)
                .SingleOrDefault();*/
            boneToTrack = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_IndexTip)
                .SingleOrDefault();

            objectToTrackMovement = boneToTrack.Transform.gameObject;
        }
        if (recording)
        {
            CheckPinchState();
        }
    }

    public void ResetDraw()
    {
        positionCount = 0;
        spherePos.Clear();
        fur.Clear();
        linePos.Clear();
        prevPointDistance = Vector3.zero;
        prevBallDistance = Vector3.zero;
    }

    private void CheckPinchState()
    {
        bool isIndexFingerPinching = ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

        float indexFingerPinchStrength = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        if (ovrHand.GetFingerConfidence(OVRHand.HandFinger.Index) != OVRHand.TrackingConfidence.High)
            return;

        // finger pinch down
        //if (isIndexFingerPinching && indexFingerPinchStrength >= minFingerPinchDownStrength)
        //{
            UpdateLine();
        //    IsPinchingReleased = true;
            return;
        //}

        // finger pinch up
       /* if (IsPinchingReleased)
        {
            AddNewLineRenderer();
            IsPinchingReleased = false;
        }*/
    }

    void UpdateLine()
    {
        if (prevPointDistance == null)
        {
            prevPointDistance = objectToTrackMovement.transform.position;
        }

        if (prevPointDistance != null && Mathf.Abs(Vector3.Distance(prevPointDistance, objectToTrackMovement.transform.position)) >= minDistanceBeforeNewPoint)
        {
            prevPointDistance = objectToTrackMovement.transform.position;
            if (positionCount == 0)
            {
                firstPoint.name = "drawfirstPoint";
                firstPoint.transform.position = objectToTrackMovement.transform.position;
                firstPoint.transform.forward = ancora.transform.forward;
                firstPoint.transform.up = ancora.transform.up;
                firstPoint.transform.right = ancora.transform.right;
                 /*firstPoint.transform.rotation = objectToTrackMovement.transform.rotation;
                 firstPoint.transform.forward = objectToTrackMovement.transform.forward;
                 firstPoint.transform.up = objectToTrackMovement.transform.up;
                 firstPoint.transform.right = objectToTrackMovement.transform.right;*/
                 //firstPoint.transform.eulerAngles = objectToTrackMovement.transform.eulerAngles;
                 fur.Add(objectToTrackMovement.transform.eulerAngles);
                //fur.Add(objectToTrackMovement.transform.up);
                //fur.Add(objectToTrackMovement.transform.right);

            }
            AddPoint(prevPointDistance);
            spaceCount++;
            //transform.
            //linePos.Add(ovrSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle1].Transform.InverseTransformPoint(prevPointDistance));
            linePos.Add(ovrSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.InverseTransformPoint(prevPointDistance));
            //if (prevBallDistance != null && Mathf.Abs(Vector3.Distance(prevBallDistance, objectToTrackMovement.transform.position)) >= 0.5f)
            if (spherePos.Count == 0 || (spaceCount >= 20))
            {
                spaceCount = 0;
                Debug.Log(Vector3.Distance(prevBallDistance, objectToTrackMovement.transform.position));
                prevBallDistance = objectToTrackMovement.transform.position;
                Instantiate(ball, firstPoint.transform.TransformPoint(firstPoint.transform.InverseTransformPoint(prevPointDistance)), new Quaternion(0, 0, 0, 1));
                spherePos.Add(firstPoint.transform.InverseTransformPoint(prevPointDistance));
            }
        }
    }

    void AddPoint(Vector3 position)
    {
        currentLineRender.SetPosition(positionCount, position);
        positionCount++;
        currentLineRender.positionCount = positionCount + 1;
        currentLineRender.SetPosition(positionCount, position);
    }

    public void UpdateLineWidth(float newValue)
    {
        currentLineRender.startWidth = newValue;
        currentLineRender.endWidth = newValue;
        lineDefaultWidth = newValue;
    }

    public void UpdateLineColor(Color color)
    {
       

        currentLineRender.material.color = color;

        currentLineRender.material.EnableKeyword("_EMISSION");
        currentLineRender.material.SetColor("_EmissionColor", color);
        defaultColor = color;
        defaultLineMaterial.color = color;

        
    }

    public void UpdateLineMinDistance(float newValue)
    {
        minDistanceBeforeNewPoint = newValue;
    }
}


