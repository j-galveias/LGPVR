using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonPlayAnimation : MenuButton
{
    private string animationName;

    public MenuButtonPlayAnimation(MenuList list, string name, bool hideText, string imagePath, string animationName) : base(list, name, hideText, imagePath)
    {
        this.animationName = animationName;
    }
    public override void AddListener() { }
}
