using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimatedSignMini
{
    public Dictionary<string, string> linguisticTags { get; set; } = new Dictionary<string, string>();
    public string version { get; set; } = "0";
    public string globalSpeed { get; set; } = "1";
    public string globalRepetitions { get; set; } = "1";

    //All these keys are the time in seconds
    public List<JsonTimeStringPair> facialExpressions { get; set; } = new List<JsonTimeStringPair>();
    public List<JsonTimeStringPair> leftHandPoses { get; set; } = new List<JsonTimeStringPair>();
    public List<JsonTimeStringPair> rightHandPoses { get; set; } = new List<JsonTimeStringPair>();
    public List<JsonTimeVector3> leftHandPositions { get; set; } = new List<JsonTimeVector3>();
    public List<JsonTimeVector3> rightHandPositions { get; set; } = new List<JsonTimeVector3>();


    public float GetVersion()
    {
        return float.Parse(version);
    }

    public float GetGlobalSpeed()
    {
        return float.Parse(globalSpeed);
    }

    public int GetGlobalRepetitions()
    {
        return int.Parse(globalRepetitions);
    }

    public HashSet<string> GetFacialExpressionSet()
    {
        HashSet<string> set = new HashSet<string>();

        foreach (JsonTimeStringPair keyFrame in facialExpressions)
        {
            set.Add(keyFrame.GetValue());
        }

        return set;
    }

    public HashSet<string> GetHandPosesSet()
    {
        HashSet<string> set = new HashSet<string>();

        foreach (JsonTimeStringPair keyFrame in leftHandPoses)
        {
            set.Add(keyFrame.GetValue());
        }

        foreach (JsonTimeStringPair keyFrame in rightHandPoses)
        {
            set.Add(keyFrame.GetValue());
        }

        return set;
    }
}
