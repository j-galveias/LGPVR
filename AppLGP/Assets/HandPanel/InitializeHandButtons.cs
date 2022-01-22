using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using UnityEngine.EventSystems;

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

        if (button is MenuButtonSelectAbstract)
        {
            buttonModel = buttonModelSelect;
        }

        if (button is MenuButtonPlayAnimation)
        {
            buttonModel = buttonModelPlayAnim;
        }

        if (button is MenuButtonClose)
        {
            buttonModel = buttonModelClose;
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
        Sprite texture = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        if (texture == null)
        {
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);

            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                AssetDatabase.WriteImportSettingsIfDirty(path);

                importer.SaveAndReimport();

                texture = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
        }

        if (texture != null)
            image.sprite = texture;
    }
}
