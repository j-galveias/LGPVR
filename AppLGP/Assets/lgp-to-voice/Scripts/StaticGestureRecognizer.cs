using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Gesture
{
    public string gestureName;
    public List<Vector3> wristRot;
    public List<Vector3> positionsPerFinger; // Relative to hand
    public List<Vector3> positionsPerBone; // Relative to hand
    public List<float> distanceBetweenFingertipsAndWrist; // Relative to hand base
    public List<float> angleBetweenAdjacentFingertips;
    public List<float> distanceBetweenAdjacentFingertips;
    public List<float> distanceBetweenFingertipsAndThumbFingertip;
    public List<Vector3> distalAng;
    public List<Vector3> interAng; 
    public UnityEvent onRecognized;
    public TMP_Text text;

    [HideInInspector]
    public float time = 0.0f;

    public Gesture(string name, List<Vector3> rot, List<Vector3> positions, List<Vector3> pob,List<float> dbfaw, List<float> abaf, List<float> dbaf, List<float> tb, UnityEvent onRecognized, TMP_Text text)
    {
        gestureName = name;
        wristRot = rot;
        positionsPerFinger = positions;
        positionsPerBone = pob;
        distanceBetweenFingertipsAndWrist = dbfaw;
        angleBetweenAdjacentFingertips = abaf;
        distanceBetweenAdjacentFingertips = dbaf;
        distanceBetweenFingertipsAndThumbFingertip = tb;
        this.text = text;
        this.onRecognized = onRecognized;
    }

    public Gesture(string name, List<Vector3> rot, List<Vector3> positions, List<Vector3> pob,List<float> dbfaw, List<float> abaf, List<float> dbaf, List<float> tb, List<Vector3> dist, List<Vector3> inter, TMP_Text text)
    {
        gestureName = name;
        wristRot = rot;
        positionsPerFinger = positions;
        positionsPerBone = pob;
        distanceBetweenFingertipsAndWrist = dbfaw;
        angleBetweenAdjacentFingertips = abaf;
        distanceBetweenAdjacentFingertips = dbaf;
        distanceBetweenFingertipsAndThumbFingertip = tb;
        distalAng = dist;
        interAng = inter;
        this.text = text;
        onRecognized = new UnityEvent();
        onRecognized.AddListener(onRec);
    }

    public void onRec()
    {
        text.text = "Found " + gestureName;
    }
}

[DisallowMultipleComponent]
public class StaticGestureRecognizer : MonoBehaviour
{
    public bool testMode;
    [Header("Recognition Timer Settings")]
    public float timeForRec;
    public float recCooldown;

    [Header("Behaviour")]
    [SerializeField] public float smallTreshold;
    [SerializeField] public float bigTreshold;
    [SerializeField] private List<Gesture> savedGestures = new List<Gesture>();
    [SerializeField] private float threshold = 0.05f;
    [SerializeField] private float delay = 0.2f;
    [SerializeField] private UnityEvent onNothingDetected = default;

    [Header("Objects")]
    [SerializeField] private GameObject head = default;
    [SerializeField] private GameObject hand = default;
    [SerializeField] private OVRSkeleton skel = default;
    [SerializeField] private GameObject[] fingers = default;

    [Header("Debugging")]
    [SerializeField] private Gesture gestureDetected = default;
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
    [SerializeField] public Gesto[] gestos;
    private List<Gesture> gestures;
    private Dictionary<string, int> counts;
    public TMP_Text message;
    public string lastLetter = "";
    public float timeRemaining;
    public float cooldownRemaining;

    public TMP_Text timer;
    public TMP_Text currentDetect;
    public Slider slider;
    public Image fill;  

    public Animator anim;

    private Gesture _previousGestureDetected = null;

    private void Start()
    {
        onNothingDetected.Invoke();
        LoadGestures();
        gestures = new List<Gesture>();
        counts = new Dictionary<string, int>();
        timeRemaining = timeForRec;
    }

    private void Update()
    {

        if(!skel.IsDataHighConfidence && testMode){
            return;
        }
        if (Input.GetKeyDown("space"))
        {
            SaveAsGesture();
        }
        gestureDetected = Recognize();


        if (timeRemaining > 0 && cooldownRemaining <= 0)
        {

            if (gestureDetected != null)
            {
                text.text = gestureDetected.gestureName;
                gestures.Add(gestureDetected);
                if (counts.ContainsKey(gestureDetected.gestureName))
                {
                    counts[gestureDetected.gestureName] += 1;
                }
                else
                {
                    counts.Add(gestureDetected.gestureName, 1);
                }
                
                timeRemaining -= Time.deltaTime;
                slider.value = (timeForRec - timeRemaining) / timeForRec;
                fill.fillAmount = slider.value;
                timer.text = timeRemaining.ToString();
            }
            else if(gestureDetected == null && counts.Count > 0)
            {
                timeRemaining -= Time.deltaTime;
                slider.value = (timeForRec - timeRemaining) / timeForRec;
                fill.fillAmount = slider.value;
                timer.text = timeRemaining.ToString();
            }
            else
            {
                onNothingDetected.Invoke();
                slider.value = 0;
                fill.fillAmount = 0;
                timeRemaining = timeForRec;
                timer.text = timeRemaining.ToString();
            }
        }
        else if (cooldownRemaining > 0) 
        {
            cooldownRemaining -= Time.deltaTime;
        }
        else
        {
            var highest = 0;
            var letter = "";

            // get most recognized letter
            foreach (var item in counts)
            {
                if (highest < item.Value)
                {
                    letter = item.Key;
                    highest = item.Value;
                }
            }

            foreach (var item in gestures)
            {
                if (item.gestureName == letter)
                {
                    text.text = letter;
                    
                    if (anim != null)
                    {
                        anim.Play("Light");
                    }
                    if (testMode)
                    {
                        message.text += letter;
                    }
                    else
                    {
                        string[] split = letter.Split('-');
                        message.text += split[1];
                    }
                    timeRemaining = timeForRec;
                    timer.text = timeRemaining.ToString();
                    slider.value = 0;
                    fill.fillAmount = 0;
                    gestures.Clear();
                    counts.Clear();
                    cooldownRemaining = recCooldown;
                    return;
                }
            }
        }

    }

    public void SaveAsGesture()
    {
        string dir = Application.persistentDataPath + "/Gestos/";

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
        List<float> tb = getThumbDistances(positionsTips);
        List<Vector3> distalAngles = new List<Vector3>();
        List<Vector3> interAngles = new List<Vector3>();

        distalAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Thumb3].Transform.localEulerAngles);
        distalAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Index3].Transform.localEulerAngles);
        distalAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Middle3].Transform.localEulerAngles);
        distalAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Ring3].Transform.localEulerAngles);
        distalAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Pinky3].Transform.localEulerAngles);

        interAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Index2].Transform.localEulerAngles);
        interAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Middle2].Transform.localEulerAngles);
        interAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Ring2].Transform.localEulerAngles);
        interAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Pinky2].Transform.localEulerAngles);

        List<Vector3> rots = new List<Vector3>();
        rots.Add(hand.transform.up);
        rots.Add(hand.transform.right);
        rots.Add(hand.transform.forward);
        List<Vector3> pob = new List<Vector3>();
        for (int i = 2; i < (int)OVRPlugin.BoneId.Hand_PinkyTip; i++)
        {
            pob.Add(hand.transform.InverseTransformPoint(skel.Bones[i].Transform.position));
        }
        Gesture novoGesto = new Gesture(gestureName.text == "" ? "New Gesture" : gestureName.text, rots, positions, pob, dbfaw, abaf, dbaf, tb, distalAngles, interAngles,text);
        savedGestures.Add(novoGesto);

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

    private float FingerAngleDiff(List<Vector3> savedDist, List<Vector3> savedInter, List<Vector3> currDist, List<Vector3> currInter)
    {
        float diffInter = 0;
        for (int i = 0; i < savedInter.Count; i++)
        {
            diffInter = CompareZAngles(savedInter[i],
                                             currInter[i]);
        }

        float diffDistal = 0;
        for (int i = 0; i < savedDist.Count; i++)
        {
            diffDistal = CompareZAngles(savedDist[i],
                                             currDist[i]);
        }

        return (diffInter * 1f + diffDistal * 0.75f) / 2;
    }

    private float CompareZAngles(Vector3 v1, Vector3 v2)
    {
        return Mathf.Abs(Mathf.DeltaAngle(v1.z, v2.z));
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

    private List<float> getThumbDistances(List<Vector3> positions)
    {
        List<float> dists = new List<float>();
        for (int i = 1; i < positions.Count; i++)
        {
            float dist = Vector3.Distance(positions[0], positions[i]);

            dists.Add(dist);
        }

        return dists;
    }

    private Gesture Recognize()
    {
        bool discardGesture = false;
        float minSumDistances = Mathf.Infinity;
        Gesture bestCandidate = null;

        List<Vector3> positionsTips = new List<Vector3>();

        // List with only the fingertips
        for (int i = (int)OVRPlugin.BoneId.Hand_ThumbTip; i < (int)OVRPlugin.BoneId.Hand_End; i++)
        {
            positionsTips.Add(skel.Bones[0].Transform.InverseTransformPoint(skel.Bones[i].Transform.position));
        }
        List<Vector3> positions = new List<Vector3>();

        // List with all the hand bones and joints
        for (int i = (int)OVRPlugin.BoneId.Hand_Thumb0; i < (int)OVRPlugin.BoneId.Hand_End; i++)
        {
            positions.Add(skel.Bones[0].Transform.InverseTransformPoint(skel.Bones[i].Transform.position));
        }

        

        List<float> dbfaw = getWristDistances(positions);
        List<float> dbaf = getAdjacentDistances(positionsTips);
        List<float> tb = getThumbDistances(positionsTips);

        List<Vector3> distalAngles = new List<Vector3>();
        List<Vector3> interAngles = new List<Vector3>();

        distalAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Thumb3].Transform.localEulerAngles);
        distalAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Index3].Transform.localEulerAngles);
        distalAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Middle3].Transform.localEulerAngles);
        distalAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Ring3].Transform.localEulerAngles);
        distalAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Pinky3].Transform.localEulerAngles);

        interAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Index2].Transform.localEulerAngles);
        interAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Middle2].Transform.localEulerAngles);
        interAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Ring2].Transform.localEulerAngles);
        interAngles.Add(skel.Bones[(int)OVRPlugin.BoneId.Hand_Pinky2].Transform.localEulerAngles);
        // For each gesture
        for (int g = 0; g < savedGestures.Count; g++)
        {

            float sumDistances = 0f;


            float total = 0;
            float fingerTotal = 0;
            float rotsTotal = 0;

            
            var textCount = 0;
            float thumbRes = 0;
            float fingerRes = 0;

            for (int i = 0; i < dbfaw.Count; i++)
            {
                var res = Math.Abs(savedGestures[g].distanceBetweenFingertipsAndWrist[i] - dbfaw[i]);
                if(i < 4)
                {
                    thumbRes += res;
                }
                else
                {
                    fingerRes += res;
                }
                //distWristRes[0].text +="\n" + res.ToString();
                if(i == 2 && g == savedGestures.Count - 1)
                {
                    //Debug.Log("thumb dist prox= " + res);
                }
                if(i >= dbfaw.Count - 5)
                {
                    //distWristRes[textCount].text = res.ToString();
                    //distWrist[textCount].text = savedGestures[g].distanceBetweenFingertipsAndWrist[i].ToString() + "   <-->   " + dbfaw[i].ToString();
                    textCount++;
                }
                /*if (res > 6)
                {
                    discardGesture = true;
                    savedGestures[g].time = 0.0f;
                    break;
                }*/
            }

            total += thumbRes * 0.5f + fingerRes * 0.5f;


            for (int i = 0; i < dbaf.Count; i++)
            {
                var res = Math.Abs(savedGestures[g].distanceBetweenAdjacentFingertips[i] - dbaf[i]);
                total += res;
            }
            if (savedGestures[g].distanceBetweenFingertipsAndThumbFingertip == null || savedGestures[g].distanceBetweenFingertipsAndThumbFingertip.Count == 0) continue;

            for (int i = 0; i < tb.Count; i++)
            {
                
                var res = Math.Abs(savedGestures[g].distanceBetweenFingertipsAndThumbFingertip[i] - tb[i]);
                total += res;
                
                if (res > 15)
                {
                    discardGesture = true;
                    savedGestures[g].time = 0.0f;
                    break;
                }
            }

            if (savedGestures[g].distalAng == null || savedGestures[g].distalAng.Count == 0) continue;

            var fingerAngleDifs = FingerAngleDiff(savedGestures[g].distalAng, savedGestures[g].interAng, distalAngles, interAngles);
            total += fingerAngleDifs;

            if (g == savedGestures.Count - 1)
            {
                Thumbtext.text = thumbRes.ToString();
            }
            fingerTotal = total;
            miniText.text = total.ToString();
            total = 0;
            if ( fingerTotal < smallTreshold)
            {
                var dif = Mathf.Abs(Vector3.Angle(savedGestures[g].wristRot[0], head.transform.InverseTransformDirection(hand.transform.up)));
                if (dif > 20)
                {
                    discardGesture = true;
                    savedGestures[g].time = 0.0f;
                }
                total += dif;
                

                dif = Mathf.Abs(Vector3.Angle(savedGestures[g].wristRot[1], head.transform.InverseTransformDirection(hand.transform.right)));
                if (dif > 20)
                {
                    discardGesture = true;
                    savedGestures[g].time = 0.0f;
                }
                total += dif;
                

                dif = Mathf.Abs(Vector3.Angle(savedGestures[g].wristRot[2], head.transform.InverseTransformDirection(hand.transform.forward)));
                if (dif > 20)
                {
                    discardGesture = true;
                    savedGestures[g].time = 0.0f;
                }
                total += dif;
                rotsTotal = total;
                
            }
            else
            {
                discardGesture = true;
                savedGestures[g].time = 0.0f;
            }

            total = fingerTotal * 1.0f + rotsTotal * 1.0f;
            totalText.text = total.ToString();
            if (rotsTotal > bigTreshold )
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

            // If it is valid and the sum of its distances is less than the existing record, it is replaced because it is a better candidate 
            if (sumDistances < minSumDistances)
            {
                if (bestCandidate != null)
                    bestCandidate.time = 0.0f;

                minSumDistances = sumDistances;
                bestCandidate = savedGestures[g];
            }
        }

        if (bestCandidate != null)
        {
            bestCandidate.time += Time.deltaTime;

            if (bestCandidate.time < delay)
                bestCandidate = null;
        }

        // If we've found something, we'll return it
        // If we haven't found anything, we return it anyway (newly created object)
        return bestCandidate;
    }

    private void LoadGestures() 
    {
        string path = Application.persistentDataPath + "/Gestos/";

        if (Directory.Exists(path))
        {
            foreach(var file in Directory.GetFiles(path))
            {
                string json = File.ReadAllText(file);
                Gesture gesture = JsonUtility.FromJson<Gesture>(json);

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
[CustomEditor(typeof(StaticGestureRecognizer))]
public class CustomInspectorGestureRecognizer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        StaticGestureRecognizer gestureRecognizer = (StaticGestureRecognizer)target;
        if (!GUILayout.Button("Save current gesture")) return;
        gestureRecognizer.SaveAsGesture();
    }
}
#endif