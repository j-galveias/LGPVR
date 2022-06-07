using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorManager : MonoBehaviour
{
    public ReflectionProbe probe;

    void Update()
    {
        probe.RenderProbe();
    }
}
