using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class ImageManager
{
    private static HashSet<string> folderLoaded = new HashSet<string>();
    private static Dictionary<string, Sprite> normalizedNames = new Dictionary<string, Sprite>();

    private static void LoadSprites(string path)
    {
        path = Path.GetDirectoryName(path).Normalize(NLP.NORMALIZATION);
        path = path.Replace("\\", "/");

        if (!IsFolderLoaded(path))
        {
            Sprite[] images = Resources.LoadAll<Sprite>(path);

            foreach (Sprite image in images)
            {
                normalizedNames[path + "/" + image.name.Normalize(NLP.NORMALIZATION)] = image;
            }

            folderLoaded.Add(path);
        }
    }

    public static Sprite GetSprite(string resourcePath)
    {
        LoadSprites(resourcePath);

        if (normalizedNames.ContainsKey(resourcePath))
            return normalizedNames[resourcePath];
        else
            return null;
    }

    public static bool SpriteExists(string resourcePath)
    {
        LoadSprites(resourcePath);
        return normalizedNames.ContainsKey(resourcePath);
    }

    private static bool IsFolderLoaded(string path)
    {
        return folderLoaded.Contains(path);
    }
}
