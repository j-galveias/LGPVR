using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class JsonTimeVector3
{
    public string time { get; set; } = "0";
    public string x { get; set; } = "0";
    public string y { get; set; } = "0";
    public string z { get; set; } = "0";

    public JsonTimeVector3() { }

    public JsonTimeVector3(float time, float x, float y, float z) : this(time, new Vector3(x, y, z)) {}

    public JsonTimeVector3(float time, Vector3 pos)
    {
        this.time = "" + time;
        this.x = "" + pos.x;
        this.y = "" + pos.y;
        this.z = "" + pos.z;
    }

    public float GetTime()
    {
        return float.Parse(time);
    }

    public Vector3 GetPos()
    {
        return new Vector3(GetX(), GetY(), GetZ());
    }

    public float GetX()
    {
        return float.Parse(x, CultureInfo.InvariantCulture);
    }

    public float GetY()
    {
        return float.Parse(y, CultureInfo.InvariantCulture);
    }

    public float GetZ()
    {
        return float.Parse(z, CultureInfo.InvariantCulture);
    }
}
