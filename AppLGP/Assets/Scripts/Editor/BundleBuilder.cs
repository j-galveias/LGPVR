using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class BundleBuilder : Editor
{

    // Start is called before the first frame update
    [MenuItem("Assets/ Build AssetBundles")]
    static void BuildAllAssetBundles() {

        // AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/Bundle/signs");
        
        // var assetBundleSigns = assetBundle.LoadAllAssets();

        // Debug.Log("BUNDLEEE");
        // foreach(var asset in assetBundleSigns){
        //     // var extension = asset is AnimationClip ? ".anim" : ".json";
        //     // AssetDatabase.CreateAsset(asset, "Assets/Resources/Animations/Try2/" + asset.name + extension);
        //     // AssetDatabase.SaveAssets();
        //     // var file = System.IO.File.ReadAllText(asset);
        //     // System.IO.File.WriteAllText(Application.dataPath + "/Resources/Try/afia_new.anim", file);
        //     Debug.Log(asset);
        // }

        // Debug.Log("NEWWW");
        var newSigns = Resources.LoadAll("Animations/Try/");
        foreach(var asset in newSigns) Debug.Log(asset);
        Debug.Log(Application.dataPath);

        Debug.Log(File.Exists(Application.dataPath + "/Resources/Animations/Try/ola.anim"));
        Debug.Log(File.Exists(Application.dataPath +"/Resources/Animations/Try/afia.json"));

        var anim = Resources.LoadAll<AnimationClip>("Animations/Try/");

        AssetBundleBuild[] buildMap = new AssetBundleBuild[anim.Length];

        // buildMap[0].assetBundleName = "signs2";
        int i = 0;
        foreach (var clip in anim) {
            string[] signsAssets;
            if (File.Exists(Application.dataPath + "/Resources/Animations/Try/"+clip.name+".json")) {
                signsAssets = new string[2];
                signsAssets[0] = "Assets/Resources/Animations/Try/" + clip.name + ".anim";
                signsAssets[1] = "Assets/Resources/Animations/Try/" + clip.name + ".json";
            }
            else {
                signsAssets = new string[1];
                signsAssets[0] = "Assets/Resources/Animations/Try/" + clip.name + ".anim";
            }
            buildMap[i].assetBundleName = clip.name;
            buildMap[i].assetNames = signsAssets;
            i++;
        }
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/Bundle/", buildMap, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);


        // // var fingerspellingArray = Resources.LoadAll("Animations/Fingerspelling/");

        // // buildMap[1].assetBundleName = "fingerspelling";
        // // string[] fingerspellingAssets = new string[fingerspellingArray.Length];
        // // i = 0;
        // // foreach (var clip in fingerspellingArray) {
        // //     var extension = clip is AnimationClip ? ".anim" : ".json";
        // //     fingerspellingAssets[i] = "Assets/Resources/Animations/Fingerspelling/" + clip.name + extension;
        // //     i++;
        // // }
        // // buildMap[1].assetNames = fingerspellingAssets;


    }
}
