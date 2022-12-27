using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Horas : MonoBehaviour
{
    public TMP_Text horas;

    // Update is called once per frame
    void Update()
    {
        var date1 = DateTime.Now;
        horas.text = date1.ToString("HH:mm:ss");
    }
}
