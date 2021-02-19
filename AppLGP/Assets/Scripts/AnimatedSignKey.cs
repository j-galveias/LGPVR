using System;
using System.Collections.Generic;

public class AnimatedSignKey: IComparable
{
    public string property { get; set; } = "null";
    public int index { get; set; } = -1;
    public float time { get; set; } = -1; //If time is not found, set it to index (in seconds)
    public Dictionary<string, int> ints { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, float> floats { get; set; } = new Dictionary<string, float>();
    public Dictionary<string, string> strings { get; set; } = new Dictionary<string, string>();

    private const string NAME = "name";
    private const string HASH = "hash";
    private const string MUSCLE = "muscle";
    private const string DURATION = "duration";

    public AnimatedSignKey()
    {
    }

    public AnimatedSignKey(string _property, int _index, float _time)
    {
        property = _property;
        index = _index;
        time = _time;
    }


    public void SetHandName(string name)
    {
        strings[NAME] = name;
    }

    public string GetHandName()
    {
        if (strings.ContainsKey(NAME))
            return strings[NAME];
        else
            return "-";
    }

    public void SetHandHash(int hash)
    {
        ints[HASH] = hash;
    }

    public int GetHandHash()
    {
        if (ints.ContainsKey(HASH))
            return ints[HASH];
        else
            return 0;
    }

    public void SetDuration(float value)
    {
        floats[DURATION] = value;
    }

    public float GetDuration()
    {
        if (floats.ContainsKey(DURATION))
            return floats[DURATION];
        else
            return 0;
    }


    public void SetMuscle(int muscleIndex, float value)
    {
        floats[MUSCLE+muscleIndex] = value;
    }

    public void SetMuscleArray(float[] muscles)
    {
        for (int i = 0; i < muscles.Length; i++)
        {
            SetMuscle(i, muscles[i]);
        }
    }

    public Dictionary<int, float> GetMuscleArray()
    {
        Dictionary<int, float> muscles = new Dictionary<int, float>();

        foreach (string key in floats.Keys)
        {
            if (key.StartsWith(MUSCLE))
            {
                string muscle = key.Substring(MUSCLE.Length);
                int muscleIndex = int.Parse(muscle);

                if (muscleIndex >= 0 && muscleIndex < 95)
                    muscles[muscleIndex] = floats[key];
            }
        }

        return muscles;
    }

    public int CompareTo(object obj)
    {
        AnimatedSignKey other = obj as AnimatedSignKey;

        if (other != null)
            return this.time.CompareTo(other.time);
        else
            throw new ArgumentException("Object is not a AnimatedSignKey");
    }
}