using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTimeStringPair : List<string>
{
    private const int TIME_INDEX = 0;
    private const int VALUE_INDEX = 1;

    public JsonTimeStringPair() : base() { }

    public JsonTimeStringPair(float timeStamp, string value)
    {
        Add("" + timeStamp);
        Add(value);
    }

    public void SetTime(float newTime)
    {
        this[TIME_INDEX] = "" + newTime;
    }

    public void SetValue(string newValue)
    {
        this[VALUE_INDEX] = newValue;
    }

    public float GetTime()
    {
        return float.Parse(this[TIME_INDEX]);
    }

    public string GetValue()
    {
        return this[VALUE_INDEX];
    }
}
