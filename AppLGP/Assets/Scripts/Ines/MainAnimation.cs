﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
// using UnityEditor.Animations;

// [System.Serializable]
// public class AnimationEvent : UnityEvent<List<string>> { }

public class MainAnimation : MonoBehaviour 
{   
    // Start is called before the first frame update
    float clipTime;
    Animation animation;
    AnimationClip animanClip;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    Animator animator;
    AnimatorStateInfo[] layerInfo;
    AnimatorOverrideController animatorOverrideController;
    Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();
    Dictionary<string, int> animationUtils = new Dictionary<string, int>();
    Dictionary<string, AnimationClip> fingerSpellClips = new Dictionary<string, AnimationClip>();
    Dictionary<string, AnimationClip> expFacClips = new Dictionary<string, AnimationClip>();
    // Dictionary<string, float> intBlendShapes = new Dictionary<string, float>() {
    //     {"F019", 26f},
    //     {"LEyeBMad", 100f},
    //     {"LEyeLidUp", 100f},
    //     {"LEyeMad", 60f},
    //     {"LEyeSad", 100f},
    //     {"REyeBMad", 100f},
    //     {"REyeLidUp", 100f},
    //     {"REyeMad", 60f},
    //     {"REyeSad", 100f},
    // };
    List<AnimationClip> events = new List<AnimationClip>();
    AnimationEvent animationEvent;
    int glosasIndex, estados;
    TranslatorInfo json;
    public GameObject replay_button;
    public InputField sentence;
    public Text frase_pensar;
    public Text text;
    public Button button;
	public GameObject character;
    public GameObject head;
    public Toggle toggle;
    BasicIK basicIK;
    Transform bone;
    List<bool> repetition = new List<bool>();
    Vector3 defaultPosition;
    AhoCorasick.Trie trie;
    List<string> glosas = new List<string>();

    bool hasExprFaciais = false;
    string[] split = null;

    CollisionDetection collisionDetection;

        //Muscles
    public enum AllMuscles : int
    {
        LEFT_SHOULDER_DOWN_UP = 37,
        LEFT_SHOULDER_FRONT_BACK = 38,
        RIGHT_SHOULDER_DOWN_UP = 46,
        RIGHT_SHOULDER_FRONT_BACK = 47,
    }

    public void Start() {
        // Component[] components = character.GetComponentsInChildren<Component>(true);
        // foreach(var component in components) {
        //     Debug.Log(component);
        // }

        skinnedMeshRenderer = head.GetComponent<SkinnedMeshRenderer>();
        
        // int blendshapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("LEyeMad");
        // Debug.Log(blendshapeIndex);
        // skinnedMeshRenderer.SetBlendShapeWeight(blendshapeIndex, 100f);

        // Debug.Log(skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("mClose"));
        // int blendshapeIndex = FindBlendshapeIndex(skinnedMeshRenderer.sharedMesh, "blendShape.mClose");
        // // skinnedMesh = skinnedMeshRenderer.sharedMesh;
        // // foreach (var mesh in skinnedMeshRenderer) Debug.Log(mesh);
        // Debug.Log(skinnedMesh.blendShapeCount);
        // skinnedMeshRenderer.SetBlendShapeWeight(blendshapeIndex, 0f);
        // Debug.Log(skinnedMeshRenderer.GetBlendShapeWeight(14));

        animator = character.GetComponent<Animator>();
        character.transform.localPosition += new Vector3(0,(float)0.9,0);
        basicIK = gameObject.GetComponent<BasicIK>();
        collisionDetection = gameObject.GetComponentInChildren<CollisionDetection>();
        // get bone
        // Transform rightShoulder = animator.GetBoneTransform((HumanBodyBones)HumanTrait.BoneFromMuscle((int) AllMuscles.RIGHT_SHOULDER_DOWN_UP));

        // text.fontSize = Camera.main.pixelHeight / 30;
        // frase_pensar.fontSize = Camera.main.pixelHeight / 30;
        
        // var inputText = sentence.GetComponent<Text>();
        // sentence.placeholder.GetComponent<Text>().fontSize = (Camera.main.pixelHeight / 38);
        // sentence.textComponent.fontSize = (Camera.main.pixelHeight / 38);
        // // sentence.GetComponent<RectTransform>().sizeDelta = new Vector2(sentence.placeholder.GetComponent<Text>().preferredWidth+35, sentence.placeholder.GetComponent<Text>().preferredHeight+35);
        // button.GetComponentInChildren<Text>().fontSize =  Camera.main.pixelHeight / 38;
       
        // frase_pensar.transform.position = new Vector3((Camera.main.pixelWidth/4) - (frase_pensar.preferredWidth/2), Camera.main.pixelHeight-(frase_pensar.preferredHeight*5), frase_pensar.transform.position.z);
        // replay_button.transform.position = new Vector3(replay_button.transform.position.x, Camera.main.pixelHeight/7, replay_button.transform.position.z);
        // text.transform.position = new Vector3((Camera.main.pixelWidth/2) - (text.preferredWidth/2), Camera.main.pixelHeight-(text.preferredHeight*3), text.transform.position.z);
        // sentence.transform.position = new Vector3(sentence.transform.position.x, Camera.main.pixelHeight/7, sentence.transform.position.z);
        // button.transform.position = new Vector3(button.transform.position.x, (Camera.main.pixelHeight/7)-80, button.transform.position.z);

        // string text = "hello and welcome to this beautiful world!";
        
        // trie.Add("hello");
        // trie.Add("world");
        // trie.Build();

        // string[] matches = trie.Find(text).ToArray();

        // Debug.Log(2 == matches.Length);
        // Debug.Log("hello" == matches[0]);
        // Debug.Log("world" == matches[1]);

        trie = new AhoCorasick.Trie();
        var getClips = Resources.LoadAll("Animations/Signs/");

        foreach(var clip in getClips)
        {
            if (clip is AnimationClip){
                AnimationClip animClip =  (AnimationClip) clip;
                var animation = removeAccents(animClip.name.ToUpper().Replace("GESTO_", ""));
                animationClips.Add(animation, animClip);
                Debug.Log(animation);

                trie.Add(animation);
                int util = animation.Contains(' ') ? animation.Split(' ').Length : 1;
                Debug.Log(util);
                animationUtils.Add(animation, util);
            }
        }
        trie.Build();

        // StartCoroutine(GetSigns());

        var getFingerSpelling = Resources.LoadAll("Animations/Fingerspelling/");

        foreach(var clip in getFingerSpelling)
        {
            if (clip is AnimationClip){
                AnimationClip animClip =  (AnimationClip) clip;
                fingerSpellClips.Add((animClip.name.ToUpper()).Replace("GESTO_", ""), animClip);
            }
        }

        var getExprs = Resources.LoadAll("Animations/FacialExpr/Sintaticas/");

        foreach(var clip in getExprs)
        {
            if (clip is AnimationClip){
                AnimationClip animClip =  (AnimationClip) clip;
                expFacClips.Add(animClip.name, animClip);
            }
        }

        defaultPosition = basicIK.rightHandPosition.position;
    }

    string authenticate(string username, string password)
    {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }


    IEnumerator GetSigns() {
        UnityWebRequest www = new UnityWebRequest("https://github.com/ineslacerda/Cluedo/master/Cluedo/README.md");
        // string authorization = authenticate("ineslacerda", "3896Menina123ines");
        // www.SetRequestHeader("AUTHORIZATION", authorization);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            foreach (var result in results)
                Debug.Log(result);
            // foreach (var sing in results) {
            //     AnimationClip animClip =  (AnimationClip) sign;
            //     var animation = (animClip.name.ToUpper()).Replace("GESTO_", "");
            // }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Stop") && animator.GetCurrentAnimatorStateInfo(0).IsName("idle_gestos")) Reset();
        animator.SetBool("Blink", !animator.GetBool("Pensar") && !animator.GetBool("ExpFacial") && !animator.GetBool("olhos_franzidos"));
        // basicIK.last = !(basicIK.last && (animator.GetCurrentAnimatorStateInfo(0).IsName("State0") || animator.GetCurrentAnimatorStateInfo(0).IsName("State1")));
        // Debug.Log(basicIK.blocked);
    }

    public void Animate(string serverMessage) {
        text.gameObject.SetActive(toggle.isOn);
        text.text = "";
        glosasIndex = 0;
        estados = 0;
        hasExprFaciais = false;
        split = null;
        animator.SetBool("Pensar", false);
        animator.SetLayerWeight(animator.GetLayerIndex ("idle_pensar"), 0);
        animator.SetLayerWeight(animator.GetLayerIndex ("idle"), 1);
        glosas = new List<string>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        json = JsonSerializer.Deserialize<TranslatorInfo>(serverMessage);

        // Debug.Log("transitionnn");
        // Debug.Log(animator.GetAnimatorTransitionInfo(0).duration);
        
        // animator.SetBool("Pensar", false);

        // AnimationClip new_clip = UnityEngine.Object.Instantiate(clip);
        // new_clip.name = "asd";

        // Debug.Log("layers");
        // Debug.Log(animator.layerCount);

        // AnimatorControllerLayer layer =  new AnimatorControllerLayer();
        // layer.defaultWeight = 1;
        // layer.blendingMode = AnimatorLayerBlendingMode.Additive;
        // layer.name = "worldlayer";
        // layer.stateMachine = new AnimatorStateMachine();
        // animatorController.AddLayer(layer);

        // Debug.Log("layers");
        // Debug.Log(animator.layerCount);

        AhoCorasick(json.glosas);

        Animating();
        // animationEvent = new AnimationEvent();
        // animationEvent.time = 0.8f;
        // animationEvent.functionName = "Animating";
        // animatorOverrideController["_tempAnim"].AddEvent(animationEvent);
        // events.Add(animatorOverrideController["_tempAnim"]);
    }

    void AhoCorasick(List<string> json_glosas) {

        // string[] animations = string.Join("",  json.glosas).split(' ');
        List<string> newLista = new List<string>();
        foreach (var glosa in json_glosas) newLista.Add(removeAccents(glosa));

        List<string> matches = trie.Find(string.Join(" ", newLista)).ToList();

        List<string> finalLista = new List<string>();

        List<string> matchesRemoveDupli = matches.Distinct().ToList();

        foreach(string glosa in newLista) {
            matches = new List<string>();
            foreach(string match in matchesRemoveDupli) {
                if (match.Contains(' ')){
                    string[] split = match.Split(' ');
                    if(split.Contains(glosa)) matches.Add(match);
                }
                else if(match.Contains(glosa)) {
                    matches.Add(match);
                }
            }
            if(matches.Count==0) // gesto não existe
                glosas.Add(glosa);
            else if(matches.Count==1) // só existe um match com o algoritmo
                glosas.Add(matches[0]);
            else { // tem que escolher o gesto com base nas utilities
                var values = matches.Where(k => animationUtils.ContainsKey(k)).Select(k => animationUtils[k]);
                Dictionary<string, int> matchUtils = matches.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
                var key = matchUtils.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                glosas.Add(key);
            }
        }

        Debug.Log("finallll matchesss");

        glosas = glosas.Distinct().ToList();

        foreach(var final in glosas) Debug.Log(final);
        Debug.Log("DONEEEE");
    }

    void Animating() {
        basicIK.ikActive = false;
        basicIK.first = true;
        // basicIK.last = false;
        basicIK.rightHandPosition.position = defaultPosition;
        repetition = new List<bool>();
        if (events.Count != 0){
            events[0].events = new AnimationEvent[0];
            events.RemoveAt(0);
        }
        var glosa = glosas[0];
        hasExprFaciais = (json.exprFaciais.Count != 0);
        // if (glosa.Contains("-"))
        //     glosa = glosa.Replace("-", " ");
        Debug.Log(hasExprFaciais);

        if (text.text == "") text.text += glosa;
        else text.text += " " + glosa;
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);

         if (hasExprFaciais){
            foreach (var expression in json.exprFaciais.Keys){
                split = expression.Split('-');
                if(Int32.Parse(split[0]) == glosasIndex){
                    if (json.exprFaciais[expression].Contains("interrogativa") || json.exprFaciais[expression].Contains("negativa")) {
                        animatorOverrideController["_tempExpFacial"] = expFacClips[json.exprFaciais[expression]];
                        animator.runtimeAnimatorController = animatorOverrideController;
                        animator.SetBool("ExpFacial", true);
                        animator.SetBool("Loop", json.exprFaciais[expression].Contains("negativa"));
                    }
                    else animator.SetBool("olhos_franzidos", true);
                }
                if(Int32.Parse(split[1]) == glosasIndex) {
                    if (json.exprFaciais[expression].Contains("interrogativa") || json.exprFaciais[expression].Contains("negativa"))
                        animator.SetBool("ExpFacial", false);
                    else animator.SetBool("olhos_franzidos", false);
                }
            }
         }

        glosa = removeAccents(glosa);
        if (animationClips.ContainsKey(glosa)) {
            // if (hasExprFaciais){
            //     if ("interrogativa" == json.exprFaciais.First().Value && glosasIndex == (Int32.Parse(split[1])-1)){
            //         StartCoroutine(Interrogativa(animationClips[glosa].length)); // play animations for facial expressions
            //     }
            // }
            // animator.speed = 1.2f;
            animatorOverrideController["_signAnim" + estados%2] = animationClips[glosa];
            animator.runtimeAnimatorController = animatorOverrideController;
            glosas.RemoveAt(0);
            animator.SetBool("Animating", true);

            animationEvent = new AnimationEvent();
            animationEvent.time = animationClips[glosa].length;

            if (glosas.Count == 0) animationEvent.functionName = "StopAnim";
            else animationEvent.functionName = "Animating";

            animationClips[glosa].AddEvent(animationEvent);
            events.Add(animationClips[glosa]);
            bool flag = false;
            if (estados % 2 == 0) flag = true;
            animator.SetBool("Animating3", flag);
            animator.SetBool("Animating2", !flag);

            glosasIndex += 1;
            estados += 1;
        }
        else Datilologia(glosa);
    }

    private IEnumerator Interrogativa(float time)
    {
        // yield return new WaitForSeconds(time);
        // animator.SetBool("ExpFacial", false);
        animator.SetBool("IntCabeca", true);
        yield return new WaitForSeconds(time+0.5f);
        animator.SetBool("IntCabeca", false);
        animator.SetBool("ExpFacial", false);
    }

    void Datilologia(string glosa) {
        if (events.Count != 0){
            events[0].events = new AnimationEvent[0];
            events.RemoveAt(0);
        }
        
        // if (glosa == "") Animating();
        Debug.Log("datilologiaaa");
        List<string> characters = new List<string>();
        string asciiStr = removeAccents(glosa);    
        Debug.Log(asciiStr);
        characters.AddRange(asciiStr.Select(c => c.ToString()));

        // Debug.Log(int.TryParse(glosa, out int n));
        bool repeat = (characters.Count > 1) ? (characters[0]==characters[1]) : false;
        // if(basicIK.ikActive) basicIK.rightHandPosition.transform.position += new Vector3(0.05f, -0.003f, 0);
        if(basicIK.ikActive) {
            basicIK.first = false;
        }
        if (basicIK.first) basicIK.difference = (new Vector3(0.001f,-0.0001f,0f)/characters.Count);
        basicIK.ikActive = int.TryParse(glosa, out int n) ? (Int32.Parse(glosa) >= 20|| characters.Count >= 3): repeat;

        if (repetition.Count == 2) repetition.RemoveAt(0);
        repetition.Add(repeat);
        Debug.Log(repeat);

        // animator.speed = basicIK.ikActive ? 0.8f : 1f;

        var character = (fingerSpellClips.ContainsKey(glosa)) ? glosa : characters[0];
        
        animatorOverrideController["_signAnim" + estados%2] = fingerSpellClips[character];
        animator.runtimeAnimatorController = animatorOverrideController;

        animator.SetBool("Animating", true);

        characters.RemoveAt(0);

        animationEvent = new AnimationEvent();
        animationEvent.time = fingerSpellClips[character].length;
        // Debug.Log("timeee");
        // Debug.Log(fingerSpellClips[character].length);
        if (fingerSpellClips.ContainsKey(glosa) && glosas.Count != 0|| characters.Count == 0 && glosas.Count != 0) {
            glosas.RemoveAt(0);
            glosasIndex += 1;
            animationEvent.functionName = "Animating";
            // basicIK.last = true;
        }
        Debug.Log(glosas.Count);
        if (glosas.Count == 0){
            // repetition = new List<bool>();
            animationEvent.functionName = "StopAnim";
            // basicIK.last = true;
        }
        else if (characters.Count != 0) {
            animationEvent.stringParameter = string.Join("", characters);
            animationEvent.functionName = "Datilologia"; 
            basicIK.last = false;
        }

        Debug.Log("event addeddd");
        fingerSpellClips[character].AddEvent(animationEvent);
        events.Add(fingerSpellClips[character]);
        
        // animator.SetBool("Repeat", repeat);
        if (repetition.Count == 2 && repetition[0]){
            // StartCoroutine(Wait());
            animator.SetBool("Animating3", false);
            animator.SetBool("Animating2", false);
            animator.SetBool("Repeat", true);
        }
        else {
            bool flag = false;
            if (estados % 2 == 0) flag = true;
            animator.SetBool("Animating3", flag);
            animator.SetBool("Animating2", !flag);
            if(!repetition[0]){
                animator.SetBool("Repeat", false);
                estados += 1;
            }else {
                animator.SetBool("Repeat", true);
            }
        }
    }

    string removeAccents(string word) {
        word = Regex.Replace(word.Normalize(NormalizationForm.FormD), @"[^A-Za-z 0-9 \.,\?'""!@#\$%\^&\*\(\)-_=\+;:<>\/\\\|\}\{\[\]`~]*", string.Empty).Trim();
        return word.Replace("-", " ").Replace("_", " ").Replace(" DE ", " ");
    }

     private IEnumerator Wait()
     {
         print("Begin wait() " + Time.time);
         yield return new WaitForSeconds(3f);

    }

    void StopAnim() {
        basicIK.ikActive = false;
        basicIK.last = true;
        Debug.Log("stopp");
        animator.SetBool("Stop", true);
        animator.SetBool("ExpFacial", false);
        animator.SetBool("olhos_franzidos", false);
        // basicIK.ikActive = false;
        basicIK.first = false;
    }

    void Reset() {
        sentence.gameObject.SetActive(true);
        button.gameObject.SetActive(true);
        replay_button.gameObject.SetActive(true);
        toggle.gameObject.SetActive(true);
        text.text = "";
        animator.ResetTrigger("Animating");
        animator.ResetTrigger("Animating2");
        animator.ResetTrigger("Animating3");
        animator.ResetTrigger("Stop");
        animator.ResetTrigger("ExpFacial");
        basicIK.rightHandPosition.position = defaultPosition;

        // for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
        // {
        //     if (i != skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("mClose"))
        //         skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
        // }
    }  

    void OnAnimatorIK() {
        if(collisionDetection.isCollided) collisionDetection.OnAnimatorIK();
    }
}
