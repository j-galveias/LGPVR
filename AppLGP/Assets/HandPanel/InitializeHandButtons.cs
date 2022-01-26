using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System.Text;
using System.Collections.Generic;

/// <summary>	
/// </summary>
public class InitializeHandButtons : MonoBehaviour
{
    public GameObject buttonModelPlayAnim;
    public GameObject buttonModelSelect;
    public GameObject buttonModelClose;

    void Start()
    {
        buttonModelPlayAnim.SetActive(false);
        buttonModelSelect.SetActive(false);
        buttonModelClose.SetActive(false);
    }

    public Button AddButton(MenuButton button)
    {
        GameObject buttonModel = null;

        if (button is MenuButtonSelect)
        {
            buttonModel = buttonModelSelect;
        }

        if (button is MenuButtonPlayAnimation)
        {
            buttonModel = buttonModelPlayAnim;
        }

        string name = button.GetName();
        bool hideText = button.GetHideText();
        string imagePath = button.GetImagePath();

        GameObject newButtonObj = Instantiate(buttonModel);
        button.obj = newButtonObj;
        newButtonObj.SetActive(true);
        Button buttonComponent = newButtonObj.GetComponentInChildren<Button>();
        Text newButtonText = buttonComponent.transform.parent.GetComponentInChildren<Text>();

        buttonComponent.transform.name = name;
        newButtonObj.name = name;
        newButtonText.text = hideText ? "" : name;
        SetThumbnail(buttonComponent.transform.GetChild(0).GetComponent<Image>(), imagePath);

        newButtonObj.transform.SetParent(transform);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());

        return buttonComponent;
    }


    private void SetThumbnail(Image image, string path)
    {
        path = path.Normalize(NLP.NORMALIZATION);
        Sprite texture = ImageManager.GetSprite(path);

        if (texture != null)
            image.sprite = texture;
        else
        {
            Debug.Log("No image found for sign " + Path.GetFileName(path));
        }
    }
}
