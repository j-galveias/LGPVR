using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Text;
using UnityEditor;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Runtime.InteropServices;
using UnityEditor.Search;

[System.Serializable]
public class Manager : MonoBehaviour
{
    const string PATH_FINGERSPELL = "/Fingerspelling/";
    const string PATH_SIGNS = "/Signs/";

    Dictionary<string, AnimationClip> animations = new Dictionary<string, AnimationClip>();
    Dictionary<string, TextAsset> jsonFiles = new Dictionary<string, TextAsset>();
    List<AnimationClip> events = new List<AnimationClip>();

    List<Button> Buttons = new List<Button>();

    public InputField search;

    int bundles = 0;
    bool loadedBundles = false;

    public GameObject buttonModel;
    public GameObject canvas;
    public Animator animator;
    private int signLayer;
    private string currentSign = "idle";
    private bool idle = true;
    private Coroutine idleCoroutine;
    private int repeat = 1;
    private bool doRepeat = false;

    private Dictionary<string, bool> hasPause = new Dictionary<string, bool>();
    private Dictionary<string, AnimatedSignData> animData = new Dictionary<string, AnimatedSignData>();

    //private Dictionary<string, Motion> animations = new Dictionary<string, Motion>();

    void Start()
    {
        // signLayer = animator.GetLayerIndex("signLayer");
        animator.transform.localPosition += new Vector3(0,(float)0.9,0);

        // InitializeButtons(Application.dataPath + "/AssetBundles/fingerspelling", true);
        // InitializeButtons(Application.dataPath + "/AssetBundles/signs");

        // LoadAssetBundle(@"C:\Users\nesla\Desktop\AssetBundles\signs");
        // LoadAssetBundle(@"C:\Users\nesla\Desktop\AssetBundles\fingerspelling");


        // AnimationClip[] animationsArray = Resources.LoadAll<AnimationClip>("Animations" + PATH_SIGNS);
        // TextAsset[] jsonArray = Resources.LoadAll<TextAsset>("Animations" + PATH_SIGNS);

        // StartCoroutine(Blink(2f, 6f));

        // Debug.Log(Application.persistentDataPath);
        // File.WriteAllText(Application.persistentDataPath + "/" + "ola.anim", "olaaa");

        // Serialize();
        StartCoroutine(GetBundle());

        //Descomentar para usar o FuzzyMatch
        //search.onValueChanged.AddListener(delegate {SortButtons();});
    }

    IEnumerator GetBundle() {
        // var assetbundles = Directory.EnumerateFiles(Application.streamingAssetsPath + "/Bundle/").Where(s => !s.Contains(".")).Select(Path.GetFileName);
        // Debug.Log(Application.streamingAssetsPath + "/Bundle/");
        // foreach(var assetbundle in assetbundles) {
        //     if (assetbundle != "Bundle"){
        //         Debug.Log(assetbundle);
        //         GetAssets(assetbundle.ToString(), assetbundle == assetbundles.Last());
        //     } 
        // }
        UnityWebRequest www = new UnityWebRequest("https://web.tecnico.ulisboa.pt/ist186436/tese/gestuario/StreamingAssets/Bundle/"); //Application.streamingAssetsPath + "/Bundle/"
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
            MatchCollection matches = regex.Matches(www.downloadHandler.text);
            // bundles = matches.Count;
            List<string> finalLinks = new List<string>();
            foreach (Match match in matches) {
                Debug.Log(match.Groups["name"].Value);
                if (match.Groups["name"].Value != "Description" && match.Groups["name"].Value != "Parent Directory" && !match.Groups["name"].Value.Contains("Bundle") && !match.Groups["name"].Value.Contains("meta")  && !match.Groups["name"].Value.Contains("manifest")){
                    finalLinks.Add(match.Groups["name"].Value);
                    Debug.Log(bundles);
                    Debug.Log(matches.Count);
                    StartCoroutine(GetAssets(match.Groups["name"].Value, match.Groups["name"].Value==matches[matches.Count-1].Groups["name"].Value.Split('.')[0]));
                }
            }
        }
    }

    IEnumerator GetAssets(string asset, bool last) {
        // Debug.Log(Path.Combine(Application.streamingAssetsPath + "/Bundle/", asset));
        // var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath + "/Bundle/", asset));
        // if (myLoadedAssetBundle == null)
        // {
        //     Debug.Log("Failed to load AssetBundle!");
        //     return;
        // }

        // AnimationClip[] animationsArray = myLoadedAssetBundle.LoadAllAssets<AnimationClip>();
        // foreach (var clip in animationsArray) {
        //     string name = clip.name.ToLower().StartsWith("gesto_") ? clip.name.Substring("gesto_".Length) : clip.name;
        //     animations.Add(name.ToLower(), clip as AnimationClip);
        // }
        // TextAsset[] jsonArray = myLoadedAssetBundle.LoadAllAssets<TextAsset>();
        // foreach (var clip in jsonArray) {
        //     string name = clip.name.ToLower().StartsWith("gesto_") ? clip.name.Substring("gesto_".Length) : clip.name;
        //     jsonFiles.Add(name.ToLower(), clip as TextAsset);
        // }
        // foreach(var anim in animations) Debug.Log(anim);
        // if(last) InitializeButtons();

        // myLoadedAssetBundle.Unload(false);


        // Debug.Log(last);
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("https://web.tecnico.ulisboa.pt/ist186436/tese/gestuario/StreamingAssets/Bundle/" + asset);
        yield return www.SendWebRequest();
 
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

            AnimationClip[] animationsArray = bundle.LoadAllAssets<AnimationClip>();
            foreach (var clip in animationsArray) {
                string name = clip.name.ToLower().StartsWith("gesto_") ? clip.name.Substring("gesto_".Length) : clip.name;
                animations.Add(name.ToLower(), clip as AnimationClip);
            }
            TextAsset[] jsonArray = bundle.LoadAllAssets<TextAsset>();
            foreach (var clip in jsonArray) {
                string name = clip.name.ToLower().StartsWith("gesto_") ? clip.name.Substring("gesto_".Length) : clip.name;
                jsonFiles.Add(name.ToLower(), clip as TextAsset);
            }
            foreach(var anim in animations) Debug.Log(anim);
            if(last) InitializeButtons();
        }
    }

    void Serialize() {
        AnimationClip[] animationsArray = Resources.LoadAll<AnimationClip>("Animations/Signs/");
        // var fileStream = new FileStream(Path.Combine(Application.streamingAssetsPath, animationsArray[0].name+".dat"), FileMode.Create);
        // // System.IO.File.WriteAllText(Path.Combine(Application.streamingAssetsPath, link), www.downloadHandler.text);
        // // Construct a BinaryFormatter and use it to serialize the data to the stream.
        // var formatter = new BinaryFormatter();
        // Instantiate(animationsArray[0]);
        // formatter.Serialize(fileStream, animationsArray[0]);
        // fileStream.Close();

        // AssetDatabase.Refresh();
        // animationsArray[0].toJSON();
    }

    private void InitializeButtons(bool pause=false)
    {
        buttonModel.SetActive(true);

        // LoadAssetBundle(path);

        // AnimationClip[] animationsArray = Resources.LoadAll<AnimationClip>(path);
        // TextAsset[] jsonArray = Resources.LoadAll<TextAsset>(path);
        // animations = new Dictionary<string, AnimationClip>();
        // Dictionary<string, TextAsset> jsonFiles = new Dictionary<string, TextAsset>();

        // foreach (AnimationClip animFile in animationsArray){
        //     Debug.Log(animFile);
        //     animations.Add(animFile.name, animFile as AnimationClip);
        // }

        // foreach (TextAsset json in jsonArray)
        //     jsonFiles.Add(json.name, json);


        List<string> list = animations.Keys.ToList();
        foreach(var a in list) Debug.Log("a: " + a);
        list.Sort();



        /*AnimatorController controller = (AnimatorController)animator.runtimeAnimatorController;
        AnimatorControllerLayer layer = controller.layers[signLayer];

        foreach (var childState in layer.stateMachine.states)
        {
            layer.stateMachine.RemoveState(childState.state);
        }

        controller.AddMotion(emptyAnim, signLayer);*/


        foreach (string key in list)
        {
            Button newButton = Instantiate(buttonModel).GetComponent<Button>();

            string animName = key;
            // string shortName = animName.ToLower().StartsWith("gesto_") ? animName.Substring("gesto_".Length) : animName;

            newButton.name = animName;
            hasPause[animName] = pause;

            newButton.transform.GetChild(0).GetComponent<Text>().text = animName;
            newButton.transform.SetParent(buttonModel.transform.parent);

            animations[key].wrapMode = WrapMode.Once;

            newButton.GetComponent<Button>().onClick.AddListener( () =>
            {
                // canvas.SetActive(false);
                // animator.transform.localPosition += new Vector3(0.1f,0,0);
                StartCoroutine(PlaySign(newButton.name));
            });

            Buttons.Add(newButton);


            // AnimatedSignData data = new AnimatedSignData();

            // try
            // {
            //     //TextAsset json = Resources.Load<TextAsset>(path + "/" + animName + ".json");
            //     //string jsonString = File.ReadAllText("Assets/Resources/"+path +"/"+animName+".json");
            //     //data = JsonConvert.DeserializeObject<AnimatedSignData>(jsonString);
            //     var jsonString = Regex.Replace(jsonFiles[key].text, ":\\s*([-1234567890\\.E]+)", ":\"$1\"");
            //     jsonString = Regex.Replace(jsonString, ":\\s*(true|false)", ":\"$1\"");
            //     data = JsonSerializer.Deserialize<AnimatedSignData>(jsonString);
            // }
            // catch (System.Exception e)
            // {
            //     Debug.Log("ERROR: Couldn't open " + animName + ".json");
            //     Debug.Log("Exception " + e.GetType() + " --- " + e.Message);
            // }

            // animData[animName] = data;

            //controller.AddMotion(animations[key] as AnimationClip, signLayer);
        }

        buttonModel.SetActive(false);
        loadedBundles = true;
    }

    void Update() {
        // if (loadedBundles) InitializeButtons();
        // Debug.Log(animations.Keys.Count());
        
        // if (search.text != null) {
        if(loadedBundles) {
            foreach (Button b in Buttons){
                b.interactable = currentSign == "idle";
                if (!String.IsNullOrEmpty(search.text))
                    b.gameObject.SetActive(removeAccents(b.name.ToLower()).IndexOf(removeAccents(search.text.ToLower()).Trim() ) >= 0);
                else
                    b.gameObject.SetActive(true);
            }
        }
    }

    string removeAccents(string word) {
        return Regex.Replace(word.Normalize(NormalizationForm.FormD), @"[^A-Za-z 0-9 \.,\?'""!@#\$%\^&\*\(\)-_=\+;:<>\/\\\|\}\{\[\]`~]*", string.Empty).Trim();
    }

    private void SortButtons()
    {
        int FuzzySearchCompare(Button b1, Button b2)
        {
            long score1 = 0;
            long score2 = 0;
            string pattern = removeAccents(search.text.ToLower()).Trim();
            string name1 = removeAccents(b1.GetComponentInChildren<Text>().text.ToLower());
            string name2 = removeAccents(b2.GetComponentInChildren<Text>().text.ToLower());

            //FuzzySearch.FuzzyMatch(pattern, name1, ref score1, new List<int>());
            //FuzzySearch.FuzzyMatch(pattern, name2, ref score2, new List<int>());

            if (score1 > score2)
                return -1;

            if (score1 == score2)
                return 0;

            return 1;
        }


        Comparison<Button> compareFunction;

        if (String.IsNullOrEmpty(search.text)) {
            compareFunction = (b1, b2) => b1.GetComponentInChildren<Text>().text.CompareTo(b2.GetComponentInChildren<Text>().text);
        }
        else
        {
            compareFunction = FuzzySearchCompare;
        }

        Buttons.Sort(compareFunction);

        for (int i = 0; i < Buttons.Count; i++)
        {
            Button button = Buttons[i];
            button.transform.SetSiblingIndex(i);
        }
    }

    // private IEnumerator Blink(float waitTimeMin, float waitTimeMax)
    // {
    //     while (true)
    //     {
    //         float time = Random.Range(waitTimeMin, waitTimeMax);
    //         yield return new WaitForSeconds(time);
    //         animator.Play("blink");
    //     }
    // }
    
    public IEnumerator PlaySign(string sign)
    {
        // animator.SetFloat("speedParam", animData[sign].globalSpeed);
        // repeat = animData[sign].globalRepetitions;
        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animatorOverrideController["_signAnim0"] = animations[sign];
        animator.runtimeAnimatorController = animatorOverrideController;
        animator.CrossFadeInFixedTime("Sign", 0.5f, 1, 0.5f);
        currentSign = sign;
        // idle = false;
        doRepeat = false;

        float waitTime = hasPause[currentSign] ? 0.1f : 0f;
        yield return new WaitForSeconds(animations[sign].length + waitTime);
        currentSign = "idle";
        animator.CrossFadeInFixedTime("Idle", 0.5f, 1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        currentSign = "idle";
        // animator.transform.localPosition -= new Vector3(0.1f,0,0);
        // canvas.SetActive(true);

    }

    public void Idle() {
        animator.CrossFadeInFixedTime("Idle", 0.5f, 1, 0.5f);
        canvas.SetActive(true);
    }

    // public void Update()
    // {
    //     AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(1);
    //     Debug.Log(state.IsName("Idle"));
    //     // if (!state.IsName("Sign"))
    //     canvas.SetActive(state.IsName("Idle"));
    // }

    // public int GetSignLayer()
    // {
    //     return signLayer;
    // }

    // public void Update()
    // {
    //     AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(1);

    //     /*if (state.normalizedTime > 0.99f && !idle)
    //         animator.SetFloat("speedParam", 0.005f);
    //     else
    //       animator.SetFloat("speedParam", 1f);*/

    //     if (!state.IsName("signEmpty") && !idle)
    //     {

    //         if (state.normalizedTime < 0.5f)
    //         {
    //             doRepeat = true;
    //         }

    //         if (state.normalizedTime > 0.999f && doRepeat)
    //         {
    //             Debug.Log(repeat);
    //             repeat--;

    //             if (repeat > 0)
    //             {
    //                 animator.CrossFadeInFixedTime(currentSign, 0.3f, 0, 0.9f);
    //                 doRepeat = false;
    //             }
    //             else
    //             {
    //                 float waitTime = hasPause.ContainsKey(currentSign) && hasPause[currentSign] ? 1f : 0f;
    //                 idleCoroutine = StartCoroutine(BecomeIdle(waitTime));
    //                 idle = true;
    //             }
    //         }
    //     }
    // }

    // private IEnumerator BecomeIdle(float waitTime)
    // {
    //     yield return new WaitForSeconds(waitTime);
    //     animator.CrossFadeInFixedTime("signEmpty", 0.5f, 2, 0f);
    // }
}