using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuButton
{
    public GameObject obj;

    protected MenuList list;
    string name;
    bool hideText;
    string imagePath;

    public MenuButton(MenuList list, string name, bool hideText, string imagePath)
    {
        this.list = list;
        this.name = name.Normalize(NLP.NORMALIZATION);
        this.hideText = hideText;
        this.imagePath = imagePath.Normalize(NLP.NORMALIZATION);
    }

    public string GetName()
    {
        return name;
    }

    public bool GetHideText()
    {
        return hideText;
    }

    public string GetImagePath()
    {
        return imagePath;
    }

    protected Button GetButtonComponent()
    {
        return obj.GetComponentInChildren<Button>();
    }

    public abstract void AddListener();
}
