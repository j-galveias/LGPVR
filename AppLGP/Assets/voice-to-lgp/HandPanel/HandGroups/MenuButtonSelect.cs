using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonSelect : MenuButton
{
    protected MenuList targetList;

    public MenuButtonSelect(MenuList list, string name, bool hideText, string imagePath, MenuList targetList) : base(list, name, hideText, imagePath)
    {
        this.targetList = targetList;
    }

    public void SelectList()
    {
        InitializeHandGroups.singleton.SelectList(targetList);
    }

    public override void AddListener()
    {
        Button button = GetButtonComponent();
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        var pointerUp = trigger.triggers[0]; //If this crashes, the Button Model needs to have a PointerUp event Type
        pointerUp.callback.AddListener((x) => SelectList());
        trigger.triggers.Add(pointerUp);
    }
}
