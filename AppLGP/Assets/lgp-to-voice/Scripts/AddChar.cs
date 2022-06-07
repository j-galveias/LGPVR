using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddChar : MonoBehaviour
{
    public TMP_Text text;
    private string _char;
    // Start is called before the first frame update
    void Start()
    {
        _char = this.GetComponentInChildren<TMP_Text>().text;
    }

    public void Pressed()
    {
        text.text += _char;
    }

    public void DeleteChar()
    {
        if(text.text.Length > 0)
        {
            text.text = text.text.Substring(0, text.text.Length - 1);
        }
    }
}
