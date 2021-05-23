using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TranslatorInfo
{
    public List<string> glosas { get; set; } = new List<string>();
    public List<List<string>> fonemas { get; set; } = new List<List<string>>();
    public Dictionary<string, List<string>> exprFaciais { get; set; } = new Dictionary<string, List<string>>();
}
