using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GestosCompostos
{
   public Dictionary<string, List<string>> gestos_compostos { get; set; } = new Dictionary<string, List<string>>();
}
