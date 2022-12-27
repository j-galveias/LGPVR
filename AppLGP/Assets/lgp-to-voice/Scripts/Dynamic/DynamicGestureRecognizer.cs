using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class DynamicGestureRecognizer : MonoBehaviour
{
    [Header("Behaviour")]
    [SerializeField] public float smallTreshold;
    [SerializeField] public float bigTreshold;
    [SerializeField] private List<DynamicGesture> savedGestures = new List<DynamicGesture>();
    [SerializeField] private float threshold = 0.05f;
    [SerializeField] private float delay = 0.2f;
    [SerializeField] private UnityEvent onNothingDetected = default;

    [Header("Objects")]
    [SerializeField] private GameObject head = default;
    [SerializeField] private GameObject hand = default;
    [SerializeField] private OVRSkeleton skel = default;
    [SerializeField] private GameObject[] fingers = default;

    [Header("Debugging")]
    [SerializeField] private DynamicGesture gestureDetected = default;
    [SerializeField] private List<DynamicGesture> gesturesDetected = default;
    [SerializeField] public List<TMP_Text> distWrist;
    [SerializeField] public List<TMP_Text> fingerAngles;
    [SerializeField] public List<TMP_Text> distFingers;
    [SerializeField] public List<TMP_Text> distWristRes;
    [SerializeField] public List<TMP_Text> fingerAnglesRes;
    [SerializeField] public List<TMP_Text> distFingersRes;
    [SerializeField] public List<TMP_Text> rotRes;
    [SerializeField] public List<TMP_Text> rots;
    [SerializeField] public TMP_Text totalText;
    [SerializeField] public TMP_Text miniText;
    [SerializeField] public TMP_Text text;
    [SerializeField] public TMP_Text gestureName;
    [SerializeField] public TMP_Text Thumbtext;
    [SerializeField] public bool testMode;

    private List<DynamicGesture> gestures;
    private Dictionary<string, int> counts;
    private Gesture initGesture;
    private bool recDynamic = false;
    private List<DynamicGesture> currentGestures;
    public GameObject gesturePrefab;
    public GameObject debugGesturePrefab;
    public Draw draw;
    public Dictionary<GameObject, string> possibleGestures;
    public GameObject ancora;
    public TMP_Text message;
    public string lastMessage;
    public TMP_Text lastReconGest;

    public TMP_Text timerText;
    public float timer;
    public bool isGestureCD;
    public Slider slider;
    public Image fill;

    public float timeRemain;

    private DynamicGesture _previousGestureDetected = null;

    private void OnEnable()
    {
        if (message.text.Equals(""))
        {
            lastMessage = "";
        }
        else
        {
            lastMessage = message.text + " ";
        }
    }

    public void UpdateLastMessage()
    {
        lastMessage = message.text;
    }

    private void OnDisable()
    {
        if (!message.text.Equals(""))
        {
            message.text = lastMessage;   
        }
    }
    public void AddLastGest()
    {
        if (!lastReconGest.text.Equals(""))
        {
            message.text = lastReconGest.text + " ";
            lastMessage = message.text;

            foreach (GameObject g in possibleGestures.Keys)
            {
                Destroy(g.GetComponent<DynamicGestureContainer>());
                Destroy(g);
            }
            possibleGestures.Clear();
        }
    }

    private void Start()
    {
        onNothingDetected.Invoke();
        LoadGestures();
        gestures = new List<DynamicGesture>();
        currentGestures = new List<DynamicGesture>();
        counts = new Dictionary<string, int>();
        possibleGestures = new Dictionary<GameObject, string>();
    }

    private void Update()
    {
        if (isGestureCD && slider != null)
        {
            timeRemain -= Time.deltaTime;
            
            if(timeRemain <= 0)
            {
                isGestureCD = false;
               
            }
        }
        if (!skel.IsDataHighConfidence)
        {
            return;
        }
        if (Input.GetKeyDown("space"))
        {
            DynamicGestureClick();
        }
        if (Input.GetKeyDown("d"))
        {
            foreach(var gest in savedGestures)
            {
                if (gest.gestureName.Equals("cv1-Preciso"))
                {
                    foreach(var s in gest.spherePoints)
                    {
                        Instantiate(gesturePrefab, s, new Quaternion(0, 0, 0, 1));
                    }
                }
            }
        }
       
        gesturesDetected = Recognize();
        if (gesturesDetected != null && !isGestureCD)
        {
            foreach (var gesture in gesturesDetected)
            {
                if (!possibleGestures.Values.Contains(gesture.gestureName))
                {
                    GameObject gest = Instantiate(gesturePrefab, skel.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.TransformPoint(gesture.spherePoints[gesture.currentPoint]), new Quaternion(0, 0, 0, 1));
                    gest.transform.name = "gestTransf";

                    

                    DynamicGestureContainer dg = gest.AddComponent<DynamicGestureContainer>();
                    if (dg.initialPos != null)
                    {
                        dg.initialPos.transform.position = skel.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position;
                        dg.initialPos.transform.forward = ancora.transform.forward;
                        dg.initialPos.transform.up = ancora.transform.up;
                        dg.initialPos.transform.right = ancora.transform.right;
                    }
                    possibleGestures.Add(gest, gesture.gestureName);
                    dg.dgr = this;
                    var serialized = JsonUtility.ToJson(gesture);
                    dg.dynamicGesture = JsonUtility.FromJson<DynamicGesture>(serialized);
                    dg.dynamicGesture.skel = skel;
                    dg.dynamicGesture.text = lastReconGest;

                    foreach (var i in gesture.spherePoints)
                    {
                        
                        dg.sphereList.Add(dg.initialPos.transform.TransformPoint(i));

                    }
                }
            }
        }
    }

    IEnumerator StartWarning()
    {
        var t = timer;
        while(t > 0)
        {
            t -= Time.deltaTime;
            timerText.text = ((int) t).ToString();

            yield return null;
        }
        timerText.text = "Começar";
        StartSaving();
    }

    public void GestureCD()
    {
        if(slider != null)
        {
            isGestureCD = true;
            timeRemain = timer;
        }
    }

    public void DynamicGestureClick() {
        if (!draw.recording)
        {
            StartCoroutine("StartWarning");
        }
        else
        {
            EndSaving();
            timerText.text = "";
        }
    }

    public void StartSaving()
    {
        draw.ResetDraw();
        List<Vector3> positionsTips = new List<Vector3>();

        for (int i = (int)OVRPlugin.BoneId.Hand_ThumbTip; i < (int)OVRPlugin.BoneId.Hand_End; i++)
        {
            positionsTips.Add(skel.Bones[0].Transform.InverseTransformPoint(skel.Bones[i].Transform.position));
        }

        List<Vector3> positions = new List<Vector3>();

        for (int i = (int)OVRPlugin.BoneId.Hand_Thumb0; i < (int)OVRPlugin.BoneId.Hand_End; i++)
        {
            positions.Add(skel.Bones[0].Transform.InverseTransformPoint(skel.Bones[i].Transform.position));
        }

        List<float> dbfaw = getWristDistances(positions);
        List<float> abaf = getAngle(positionsTips);
        List<float> dbaf = getAdjacentDistances(positionsTips);

        List<Vector3> rots = new List<Vector3>();
        rots.Add(hand.transform.up);
        rots.Add(hand.transform.right);
        rots.Add(hand.transform.forward);
        List<Vector3> pob = new List<Vector3>();
        for (int i = 2; i < (int)OVRPlugin.BoneId.Hand_PinkyTip; i++)
        {
            pob.Add(hand.transform.InverseTransformPoint(skel.Bones[i].Transform.position));
        }
        initGesture = new Gesture(gestureName.text == "" ? "New Gesture" : gestureName.text, rots, positions, pob, dbfaw, abaf, dbaf, new List<float>(), new List<Vector3>(), new List<Vector3>(), text) ;

        draw.AddNewLineRenderer();
        draw.recording = true;
    }

    public void EndSaving()
    {
        DynamicGesture dynamicGesture = new DynamicGesture(gestureName.text == "" ? "New Gesture" : gestureName.text, initGesture, draw.linePos, draw.spherePos, draw.fur, text, skel);
        string dir = Application.persistentDataPath + "/GestosDinamicos/";

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string json = JsonUtility.ToJson(dynamicGesture);

        File.WriteAllText(dir + dynamicGesture.gestureName, json);

        savedGestures.Add(dynamicGesture);
        draw.recording = false;
    }

    public void SaveAsGesture()
    {
        string dir = Application.persistentDataPath + "/GestosDinamicos/";

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        List<Vector3> positionsTips = new List<Vector3>();

        for (int i = (int)OVRPlugin.BoneId.Hand_ThumbTip; i < (int)OVRPlugin.BoneId.Hand_End; i++)
        {
            positionsTips.Add(skel.Bones[0].Transform.InverseTransformPoint(skel.Bones[i].Transform.position));
        }

        List<Vector3> positions = new List<Vector3>();

        for (int i = (int)OVRPlugin.BoneId.Hand_Thumb0; i < (int)OVRPlugin.BoneId.Hand_End; i++)
        {
            positions.Add(skel.Bones[0].Transform.InverseTransformPoint(skel.Bones[i].Transform.position));
        }

        List<float> dbfaw = getWristDistances(positions);
        List<float> abaf = getAngle(positionsTips);
        List<float> dbaf = getAdjacentDistances(positionsTips);

        List<Vector3> rots = new List<Vector3>();
        rots.Add(hand.transform.up);
        rots.Add(hand.transform.right);
        rots.Add(hand.transform.forward);
        List<Vector3> pob = new List<Vector3>();
        for (int i = 2; i < (int)OVRPlugin.BoneId.Hand_PinkyTip; i++)
        {
            pob.Add(hand.transform.InverseTransformPoint(skel.Bones[i].Transform.position));
        }
        Gesture novoGesto = new Gesture(gestureName.text == "" ? "New Gesture" : gestureName.text, rots, positions, pob, dbfaw, abaf, dbaf, new List<float>(), new List<Vector3>(), new List<Vector3>(), text);

        string json = JsonUtility.ToJson(novoGesto);

        File.WriteAllText(dir + novoGesto.gestureName, json);
    }

    private List<float> getAngle(List<Vector3> positions)
    {
        List<float> angles = new List<float>();
        for (int i = 0; i < positions.Count - 1; i++)
        {
            
            float angle = Mathf.Acos((positions[i].x * positions[i + 1].x + positions[i].y * positions[i + 1].y + positions[i].z * positions[i + 1].z) / 
                (Mathf.Sqrt(Mathf.Pow(positions[i].x, 2) + Mathf.Pow(positions[i].y, 2) + Mathf.Pow(positions[i].z, 2)) * 
                Mathf.Sqrt(Mathf.Pow(positions[i + 1].x, 2) + Mathf.Pow(positions[i + 1].y, 2) + Mathf.Pow(positions[i + 1].z, 2))));

            angles.Add(angle);
        }

        return angles;
    }

    private List<float> getAdjacentDistances(List<Vector3> positions)
    {
        List<float> dists = new List<float>();
        for (int i = 0; i < positions.Count - 1; i++)
        {
            float dist = Vector3.Distance(positions[i], positions[i + 1]);

            dists.Add(dist);
        }

        return dists;
    }

    private List<float> getWristDistances(List<Vector3> positions)
    {
        List<float> dists = new List<float>();
        for (int i = 0; i < positions.Count; i++)
        {
            float dist = positions[i].magnitude * 1000;

            dists.Add(dist);
        }

        return dists;
    }

    private List<DynamicGesture> Recognize()
    {
        bool discardGesture = false;
        float minSumDistances = Mathf.Infinity;
        DynamicGesture bestCandidate = null;
        Dictionary<DynamicGesture, float> bestCandidates = new Dictionary<DynamicGesture, float>();

        // For each gesture
        for (int g = 0; g < savedGestures.Count; g++)
        {
            // If the number of fingers does not match, it returns an error
            /*if (fingers.Length != savedGestures[g].positionsPerFinger.Count)
                throw new Exception("Different number of tracked fingers");*/

            float sumDistances = 0f;

            List<Vector3> positionsTips = new List<Vector3>();

            for (int i = (int)OVRPlugin.BoneId.Hand_ThumbTip; i < (int)OVRPlugin.BoneId.Hand_End; i++)
            {
                positionsTips.Add(skel.Bones[0].Transform.InverseTransformPoint(skel.Bones[i].Transform.position));
            }
            List<Vector3> positions = new List<Vector3>();

            for (int i = (int)OVRPlugin.BoneId.Hand_Thumb0; i < (int)OVRPlugin.BoneId.Hand_End; i++)
            {
                positions.Add(skel.Bones[0].Transform.InverseTransformPoint(skel.Bones[i].Transform.position));
            }

            List<float> dbfaw = getWristDistances(positions);
            List<float> dbaf = getAdjacentDistances(positionsTips);

            float total = 0;

            
            var textCount = 0;
            float thumbRes = 0;
            float fingerRes = 0;

            for (int i = 0; i < dbfaw.Count; i++)
            {
                var res = Math.Abs(savedGestures[g].gesture.distanceBetweenFingertipsAndWrist[i] - dbfaw[i]);
                if(i < 4)
                {
                    thumbRes += res;
                }
                else
                {
                    fingerRes += res;
                }
                
                
                
            }

            total += thumbRes  + fingerRes ;

            if (g == savedGestures.Count - 1)
            {
                
            }

            

            for (int i = 0; i < dbaf.Count; i++)
            {
                
                var res = Math.Abs(savedGestures[g].gesture.distanceBetweenAdjacentFingertips[i] - dbaf[i]);
                total += res;
               
            }
            
            if (savedGestures[g].gestureName.Equals("NG-Bom dia"))
            {
                Debug.Log("Toall = " + total);
            }
            if (total < smallTreshold)
            {
                var dif = Mathf.Abs(Vector3.Angle(savedGestures[g].gesture.wristRot[0], head.transform.InverseTransformDirection(hand.transform.up)));
                if (dif > 20)
                {
                    discardGesture = true;
                    savedGestures[g].time = 0.0f;
                }
                total += dif;
                

                dif = Mathf.Abs(Vector3.Angle(savedGestures[g].gesture.wristRot[1], head.transform.InverseTransformDirection(hand.transform.right)));
                if (dif > 20)
                {
                    discardGesture = true;
                    savedGestures[g].time = 0.0f;
                }
                total += dif;
                

                dif = Mathf.Abs(Vector3.Angle(savedGestures[g].gesture.wristRot[2], head.transform.InverseTransformDirection(hand.transform.forward)));
                if (dif > 20)
                {
                    discardGesture = true;
                    savedGestures[g].time = 0.0f;
                }
                total += dif;
                
            }
            else
            {
                discardGesture = true;
                savedGestures[g].time = 0.0f;
            }
            
            if (total > bigTreshold )
            {
                discardGesture = true;
                savedGestures[g].time = 0.0f;
            }
            else if(!discardGesture) {
                sumDistances = total;
            }

            // If we have to discard the gesture, we skip it
            if (discardGesture)
            {
                discardGesture = false;
                continue;
            }

           

            bestCandidates.Add(savedGestures[g], sumDistances);
        }

       
        bestCandidates.OrderBy(key => key.Value);
        List<DynamicGesture> bestOfBest = new List<DynamicGesture>();
        string best = "";
        for (int i = 0; i < (bestCandidates.Count < 3 ? bestCandidates.Count : 3); i++)
        {
            bestOfBest.Add(bestCandidates.ElementAt(i).Key);
            best += bestCandidates.ElementAt(i).Key.gestureName + " - ";
        }
        if (!best.Equals(""))
        {
            Debug.Log(best);
        }

        // If we've found something, we'll return it
        // If we haven't found anything, we return it anyway (newly created object)
        return bestOfBest;
    }

    void CheckGestures() { 
        //currentGestures
        foreach(var ges in currentGestures)
        {
            if(Math.Abs(Vector3.Distance(skel.Bones[(int)OVRSkeleton.BoneId.Hand_Middle1].Transform.TransformPoint(ges.spherePoints[ges.currentPoint]), skel.Bones[(int)OVRSkeleton.BoneId.Hand_Middle1].Transform.position)) > 2f)
            {
                currentGestures.Remove(ges);
            }
        }
    }

    private void LoadGestures() 
    {
        string path = Application.persistentDataPath + "/GestosDinamicos/";

        if (Directory.Exists(path))
        {
            foreach(var file in Directory.GetFiles(path))
            {
                string json = File.ReadAllText(file);
                DynamicGesture gesture = JsonUtility.FromJson<DynamicGesture>(json);

                savedGestures.Add(gesture);
            }
        }
        else
        {
            Directory.CreateDirectory(path);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DynamicGestureRecognizer))]
public class CustomInspectorDynamicGestureRecognizer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DynamicGestureRecognizer dynamicGestureRecognizer = (DynamicGestureRecognizer)target;
        if (!GUILayout.Button("Save current gesture")) return;
        dynamicGestureRecognizer.SaveAsGesture();
    }
}
#endif