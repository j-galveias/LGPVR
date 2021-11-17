using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

/// <summary>	
/// </summary>
public class InitializeHandButtons : MonoBehaviour
{
    public GameObject buttonModel;

    void Start()
    {
        buttonModel.SetActive(false);
    }

    public Button AddButton(string name, bool hideText, string imagePath)
    {
        GameObject newButtonObj = Instantiate(buttonModel);
        newButtonObj.SetActive(true);
        Button newButton = newButtonObj.GetComponentInChildren<Button>();
        Text newButtonText = newButton.transform.parent.GetComponentInChildren<Text>();

        newButton.transform.name = name;
        newButtonObj.name = name;
        newButtonText.text = hideText ? "" : name;
        SetThumbnail(newButton.transform.GetChild(0).GetComponent<Image>(), imagePath);

        newButtonObj.transform.SetParent(transform);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());

        return newButton;
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
