using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TranslatorInfo
{
    public List<string> glosas { get; set; } = new List<string>();
    public List<string> fonemas { get; set; } = new List<string>();
    public Dictionary<string, string> exprFaciais { get; set; } = new Dictionary<string,string>();
}
