﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text.Json;

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
    Dictionary<string, AnimationClip> fingerSpellClips = new Dictionary<string, AnimationClip>();
    Dictionary<string, AnimationClip> expFacClips = new Dictionary<string, AnimationClip>();
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

    public void Start() {
        // Component[] components = character.GetComponentsInChildren<Component>(true);
        // foreach(var component in components) {
        //     Debug.Log(component);
        // }
        // skinnedMeshRenderer = character.GetComponent<SkinnedMeshRenderer>();
        // skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        // Debug.Log(skinnedMesh.blendShapeCount);
        animator = character.GetComponent<Animator>();
        character.transform.localPosition += new Vector3(0,(float)0.9,0);
        text.fontSize = Camera.main.pixelHeight / 30;
        frase_pensar.fontSize = Camera.main.pixelHeight / 30;
        // var inputText = sentence.GetComponent<Text>();
        // sentence.placeholder.GetComponent<Text>().fontSize = (Camera.main.pixelHeight / 38);
        // sentence.textComponent.fontSize = (Camera.main.pixelHeight / 38);
        // // sentence.GetComponent<RectTransform>().sizeDelta = new Vector2(sentence.placeholder.GetComponent<Text>().preferredWidth+35, sentence.placeholder.GetComponent<Text>().preferredHeight+35);
        // button.GetComponentInChildren<Text>().fontSize =  Camera.main.pixelHeight / 38;
        frase_pensar.transform.position = new Vector3((Camera.main.pixelWidth/4) - (frase_pensar.preferredWidth/2), Camera.main.pixelHeight-(frase_pensar.preferredHeight*5), frase_pensar.transform.position.z);
        replay_button.transform.position = new Vector3(replay_button.transform.position.x, Camera.main.pixelHeight/7, replay_button.transform.position.z);
        text.transform.position = new Vector3((Camera.main.pixelWidth/2) - (text.preferredWidth/2), Camera.main.pixelHeight-(text.preferredHeight*3), text.transform.position.z);
        sentence.transform.position = new Vector3(sentence.transform.position.x, Camera.main.pixelHeight/7, sentence.transform.position.z);
        button.transform.position = new Vector3(button.transform.position.x, (Camera.main.pixelHeight/7)-80, button.transform.position.z);

        var getClips = Resources.LoadAll("Animations/Signs/");

        foreach(var clip in getClips)
        {
            if (clip is AnimationClip){
                AnimationClip animClip =  (AnimationClip) clip;
                animationClips.Add((animClip.name.ToUpper()).Replace("GESTO_", ""), animClip);
            }
        }

        var getFingerSpelling = Resources.LoadAll("Animations/Fingerspelling/");

        foreach(var clip in getFingerSpelling)
        {
            if (clip is AnimationClip){
                AnimationClip animClip =  (AnimationClip) clip;
                fingerSpellClips.Add((animClip.name.ToUpper()).Replace("GESTO_", ""), animClip);
                Debug.Log((animClip.name.ToUpper()).Replace("GESTO_", ""));
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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Animate(string serverMessage) {
        text.text = "";
        glosasIndex = 0;
        estados = 0;
        animator.SetBool("Pensar", false);
        animator.SetLayerWeight(animator.GetLayerIndex ("idle_pensar"), 0);
        animator.SetLayerWeight(animator.GetLayerIndex ("idle"), 1);

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        json = JsonSerializer.Deserialize<TranslatorInfo>(serverMessage);
        // animator.SetBool("Pensar", false);
        Animating();
        // animationEvent = new AnimationEvent();
        // animationEvent.time = 0.8f;
        // animationEvent.functionName = "Animating";
        // animatorOverrideController["_tempAnim"].AddEvent(animationEvent);
        // events.Add(animatorOverrideController["_tempAnim"]);
    }

    void Animating() {
        if (events.Count != 0){
            events[0].events = new AnimationEvent[0];
            events.RemoveAt(0);
        }
        var glosa = json.glosas[0];
        bool hasExprFaciais = (json.exprFaciais.Count != 0);
        if (glosa.Contains("-"))
            glosa = glosa.Replace("-", " ");

        if (hasExprFaciais){
            animatorOverrideController["_tempExpFacial"] = expFacClips[json.exprFaciais.First().Value];
            animator.runtimeAnimatorController = animatorOverrideController;
            string[] split = (json.exprFaciais.First().Key).Split('-');
            if (glosasIndex == Int32.Parse(split[0])) animator.SetBool("ExpFacial", true);
            else if (glosasIndex == Int32.Parse(split[1])) animator.SetBool("ExpFacial", false);
        }

        if (text.text == "") text.text += glosa;
        else text.text += " " + glosa;
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);

        if (animationClips.ContainsKey(glosa)) {
            animatorOverrideController["_signAnim" + estados%2] = animationClips[glosa];
            animator.runtimeAnimatorController = animatorOverrideController;
            json.glosas.RemoveAt(0);
            animator.SetBool("Animating", true);

            animationEvent = new AnimationEvent();
            animationEvent.time = animationClips[glosa].length;

            if (json.glosas.Count == 0) animationEvent.functionName = "StopAnim";
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

    void Datilologia(string glosa) {
        if (events.Count != 0){
            events[0].events = new AnimationEvent[0];
            events.RemoveAt(0);
        }
        // if (glosa == "") Animating();
        Debug.Log("datilologiaaa");
        List<string> characters = new List<string>();
        characters.AddRange(glosa.Select(c => c.ToString()));
        var character = characters[0];

        if (fingerSpellClips.ContainsKey(character)) {
            animatorOverrideController["_signAnim" + estados%2] = fingerSpellClips[character];
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        animator.SetBool("Animating", true);

        animationEvent = new AnimationEvent();
        animationEvent.time = fingerSpellClips[character].length;
        characters.RemoveAt(0);
        if (characters.Count == 0 && json.glosas.Count != 0) {
            json.glosas.RemoveAt(0);
            glosasIndex += 1;
            animationEvent.functionName = "Animating";
        }
        if (json.glosas.Count == 0) animationEvent.functionName = "StopAnim";
        else if (characters.Count != 0) {
            animationEvent.stringParameter = string.Join("", characters);
            animationEvent.functionName = "Datilologia"; 
        }
        fingerSpellClips[character].AddEvent(animationEvent);
        events.Add(fingerSpellClips[character]);
        bool flag = false;
        if (estados % 2 == 0) flag = true;
        animator.SetBool("Animating3", flag);
        animator.SetBool("Animating2", !flag);

        estados += 1;
    }

    void StopAnim() {
        Debug.Log("stopp");
        animator.SetBool("Stop", true);
        animator.ResetTrigger("ExpFacial");
        animationEvent = new AnimationEvent();
        animationEvent.time = 0.8f;
        animationEvent.functionName = "Reset";
        animatorOverrideController["_tempAnim"].AddEvent(animationEvent);
    }

    void Reset() {
        animatorOverrideController["_tempAnim"].events = new AnimationEvent[0];
        sentence.gameObject.SetActive(true);
        button.gameObject.SetActive(true);
        replay_button.gameObject.SetActive(true);
        text.text = "";
        animator.ResetTrigger("Animating");
        animator.ResetTrigger("Animating2");
        animator.ResetTrigger("Animating3");
        animator.ResetTrigger("Stop");
    }  
}
