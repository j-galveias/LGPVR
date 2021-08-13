using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSignKey: IComparable
{
    public string property { get; set; } = "null";
    public int index { get; set; } = -1;

    //Time should really really really be private!
    public float time { get; set; } = -1; //If time is not found, set it to index (in seconds)
    public Dictionary<string, int> ints { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, float> floats { get; set; } = new Dictionary<string, float>();
    public Dictionary<string, string> strings { get; set; } = new Dictionary<string, string>();

    public float inTangent { get; set; } = 0;
    public float inWeight { get; set; } = 1/3;
    public float outTangent { get; set; } = 0;
    public float outWeight { get; set; } = 1/3;
    public WeightedMode weightedMode { get; set; } = WeightedMode.None;

    private const string NAME = "name";
    private const string FACE_NAME = "faceName";
    private const string HASH = "hash";
    private const string DURATION = "duration";
    private const string INTERPOLATION = "interpolation";
    private const string LEFT_HAND_POS_X = "leftHandPositionX";
    private const string LEFT_HAND_POS_Y = "leftHandPositionY";
    private const string LEFT_HAND_POS_Z = "leftHandPositionZ";
    private const string RIGHT_HAND_POS_X = "rightHandPositionX";
    private const string RIGHT_HAND_POS_Y = "rightHandPositionY";
    private const string RIGHT_HAND_POS_Z = "rightHandPositionZ";
    public const string MUSCLE = "muscle";
    private const string LOCAL_POSITION = ".m_LocalPosition"; //Unity strings given by EditorCurveBinding.propertyName
    private const string LOCAL_ANGLES = ".localEulerAnglesRaw";
    private const string LOCAL_SCALE = ".m_LocalScale";
    private const string BLENDSHAPE = ".blendShape";

    public AnimatedSignKey()
    {
    }

    public AnimatedSignKey(string _property, int _index, float _time)
    {
        property = _property;
        index = _index;
        time = _time;
    }

    public AnimatedSignKey(string _property, int _index, Keyframe keyframe)
    {
        property = _property;
        index = _index;
        time = keyframe.time;
        inTangent = keyframe.inTangent;
        inWeight = keyframe.inWeight;
        outTangent = keyframe.outTangent;
        outWeight = keyframe.outWeight;
        weightedMode = keyframe.weightedMode;
    }

    // private void SetHandPos(Vector3 pos, bool left)
    // {
    //     floats[left ? LEFT_HAND_POS_X : RIGHT_HAND_POS_X] = pos.x;
    //     floats[left ? LEFT_HAND_POS_Y : RIGHT_HAND_POS_Y] = pos.y;
    //     floats[left ? LEFT_HAND_POS_Z : RIGHT_HAND_POS_Z] = pos.z;
    // }

    // public void SetLeftHandPos(Vector3 pos)
    // {
    //     SetHandPos(pos, true);
    // }

    // public void SetRightHandPos(Vector3 pos)
    // {
    //     SetHandPos(pos, false);
    // }

    // public Vector3 GetLeftHandPos()
    // {
    //     if (floats.ContainsKey(LEFT_HAND_POS_X) && floats.ContainsKey(LEFT_HAND_POS_Y) && floats.ContainsKey(LEFT_HAND_POS_Z))
    //         return new Vector3(floats[LEFT_HAND_POS_X], floats[LEFT_HAND_POS_Y], floats[LEFT_HAND_POS_Z]);
    //     else
    //         return new Vector3(-0.2132077f, 0, 0);
    // }

    // public Vector3 GetRightHandPos()
    // {
    //     if (floats.ContainsKey(RIGHT_HAND_POS_X) && floats.ContainsKey(RIGHT_HAND_POS_Y) && floats.ContainsKey(RIGHT_HAND_POS_Z))
    //         return new Vector3(floats[RIGHT_HAND_POS_X], floats[RIGHT_HAND_POS_Y], floats[RIGHT_HAND_POS_Z]);
    //     else
    //         return new Vector3(-0.2132077f, 0, 0);
    // }

    // public void SetInterpolationDefault()
    // {
    //     strings[INTERPOLATION] = AnimatedSign.INTERPOLATION_DEFAULT;
    // }

    // public void SetInterpolationSmooth()
    // {
    //     strings[INTERPOLATION] = AnimatedSign.INTERPOLATION_SMOOTH;
    // }

    // public string GetInterpolation()
    // {
    //     if (strings.ContainsKey(INTERPOLATION))
    //         return strings[INTERPOLATION];
    //     else
    //         return AnimatedSign.INTERPOLATION_DEFAULT;
    // }

    // public void SetHandName(string name)
    // {
    //     strings[NAME] = name;
    // }

    // public string GetHandName()
    // {
    //     if (strings.ContainsKey(NAME))
    //         return strings[NAME];
    //     else
    //         return AnimatedSign.HAND_DEFAULT;
    // }

    // public void SetHandHash(int hash)
    // {
    //     ints[HASH] = hash;
    // }

    // public int GetHandHash()
    // {
    //     if (ints.ContainsKey(HASH))
    //         return ints[HASH];
    //     else
    //         return 0;
    // }

    // public void SetFaceName(string faceName)
    // {
    //     strings[FACE_NAME] = faceName;
    // }

    // public string GetFaceName()
    // {
    //     if (strings.ContainsKey(FACE_NAME))
    //         return strings[FACE_NAME];
    //     else
    //         return AnimatedSign.FACE_DEFAULT;
    // }

    // public void SetPropertyValue(string pathProperty, float value)
    // {
    //     floats[pathProperty] = value;
    // }

    // public void SetDuration(float value)
    // {
    //     floats[DURATION] = value;
    // }

    // public float GetDuration()
    // {
    //     if (floats.ContainsKey(DURATION))
    //         return floats[DURATION];
    //     else
    //         return 0;
    // }


    // public void SetMuscle(int muscleIndex, float value)
    // {
    //     floats[MUSCLE+muscleIndex] = value;
    // }

    // public void SetMuscleArray(float[] muscles, int min=0, int max=-1)
    // {
    //     if (max < 0)
    //         max = muscles.Length;

    //     for (int i = min; i < max; i++)
    //     {
    //         SetMuscle(i, muscles[i]);
    //     }
    // }

    // public Dictionary<int, float> GetMuscleArray()
    // {
    //     Dictionary<int, float> muscles = new Dictionary<int, float>();

    //     foreach (string key in floats.Keys)
    //     {
    //         if (key.StartsWith(MUSCLE))
    //         {
    //             string muscle = key.Substring(MUSCLE.Length);
    //             int muscleIndex = int.Parse(muscle);

    //             if (muscleIndex >= 0 && muscleIndex < 95)
    //                 muscles[muscleIndex] = floats[key];
    //         }
    //     }

    //     return muscles;
    // }

    public int CompareTo(object obj)
    {
        AnimatedSignKey other = obj as AnimatedSignKey;

        if (other != null)
            return 0;
            // return this.time.CompareTo(other.time);
        else
            throw new ArgumentException("Object is not a AnimatedSignKey");
    }
}