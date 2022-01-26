using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuList
{
    public static InitializeHandGroups handGroupManager;

    public GameObject obj = null;

    private string name;
    private MenuList parent;
    private List<MenuList> children = new List<MenuList>();
    private Dictionary<string, MenuButton> buttons = new Dictionary<string, MenuButton>();

    public MenuList(string listName)
    {
        name = listName.Normalize(NLP.NORMALIZATION);
    }

    public MenuList(string listName, MenuList parentList)
    {
        name = listName.Normalize(NLP.NORMALIZATION);
        parent = parentList;
        parentList.AddChild(this);
    }

    public MenuList GetParent()
    {
        return parent;
    }

    public void AddChild(MenuList child)
    {
        children.Add(child);
    }

    public MenuButton AddButton(MenuButton menuButton)
    {
        buttons[menuButton.GetName()] = menuButton;
        return buttons[menuButton.GetName()];
    }

    public MenuButton AddButtonSelect(string name, bool hideText, string imagePath, MenuList targetList)
    {
        return AddButton(new MenuButtonSelect(this, name, hideText, imagePath, targetList));
    }

    public MenuButton AddButtonPlayAnimation(string name, bool hideText, string imagePath, string animationName)
    {
        return AddButton(new MenuButtonPlayAnimation(this, name, hideText, imagePath, animationName));
    }

    public GameObject Instantiate()
    {
        InstantiateSelf();
        var initHandButtons = obj.GetComponent<InitializeHandButtons>();

        foreach (var entry in buttons)
        {
            MenuButton button = entry.Value;
            initHandButtons.AddButton(button);
            button.AddListener();
        }

        return obj;
    }

    private GameObject InstantiateSelf()
    {
        obj = Object.Instantiate(handGroupManager.modelHandList);
        obj.transform.SetParent(handGroupManager.modelHandList.transform.parent);
        obj.name = name;
        obj.SetActive(true);

        return obj;
    }
}
