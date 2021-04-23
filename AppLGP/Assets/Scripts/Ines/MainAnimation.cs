using System;
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

    bool hasExprFaciais = false;
    string[] split = null;

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

        var getClips = Resources.LoadAll("Animations/Signs/");

        foreach(var clip in getClips)
        {
            if (clip is AnimationClip){
                AnimationClip animClip =  (AnimationClip) clip;
                var animation = (animClip.name.ToUpper()).Replace("GESTO_", "");
                animationClips.Add(removeAccents(animation), animClip);
                Debug.Log(removeAccents(animation));
                // var animation = (animClip.name.ToUpper()).Replace("GESTO_", "");
                // char[] values = animation.ToCharArray();
                // foreach (char letter in values)
                // {
                //     // Get the integral value of the character.
                //     int value = Convert.ToInt32(letter);
                //     // Convert the integer value to a hexadecimal value in string form.
                //     Debug.Log($"Hexadecimal value of {letter} is {value:X}");
                // }
            }
        }

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
        if(www.isNetworkError || www.isHttpError) {
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
        animator.SetBool("Idle", animator.GetCurrentAnimatorStateInfo(0).IsName("idle_gestos"));
        // basicIK.last = !(basicIK.last && (animator.GetCurrentAnimatorStateInfo(0).IsName("State0") || animator.GetCurrentAnimatorStateInfo(0).IsName("State1")));
        // Debug.Log(basicIK.blocked);
    }

    void OnCollisionEnter(Collision collision) { 
        Debug.Log("collisionnn");
        Debug.Log(collision.collider.tag);
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

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        json = JsonSerializer.Deserialize<TranslatorInfo>(serverMessage);
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

        Animating();
        // animationEvent = new AnimationEvent();
        // animationEvent.time = 0.8f;
        // animationEvent.functionName = "Animating";
        // animatorOverrideController["_tempAnim"].AddEvent(animationEvent);
        // events.Add(animatorOverrideController["_tempAnim"]);
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
        var glosa = json.glosas[0];
        hasExprFaciais = (json.exprFaciais.Count != 0);
        // if (glosa.Contains("-"))
        //     glosa = glosa.Replace("-", " ");
        Debug.Log(hasExprFaciais);

        if (text.text == "") text.text += glosa;
        else text.text += " " + glosa;
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);

         if (hasExprFaciais){
                // Debug.Log("expeeee");
                animatorOverrideController["_tempExpFacial"] = expFacClips[json.exprFaciais.First().Value];
                animator.runtimeAnimatorController = animatorOverrideController;
                split = (json.exprFaciais.First().Key).Split('-');
                if (glosasIndex == (Int32.Parse(split[0]))){
                    Debug.Log("negativa");
                    // StartCoroutine(Int()); // animate blendshape for interrogativa
                   animator.SetBool("ExpFacial", true); // negativa headshake && int blendshapes
                }
                if ("negativa" == json.exprFaciais.First().Value && glosasIndex == Int32.Parse(split[1])) animator.SetBool("ExpFacial", false);
                if ("negativa" == json.exprFaciais.First().Value) animator.SetBool("Loop", true);
         }

        glosa = removeAccents(glosa);
        if (animationClips.ContainsKey(glosa)) {
            if (hasExprFaciais){
                if ("interrogativa" == json.exprFaciais.First().Value && glosasIndex == (Int32.Parse(split[1])-1)){
                    StartCoroutine(Interrogativa(animationClips[glosa].length)); // play animations for facial expressions
                }
            }
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
        if (basicIK.first) basicIK.difference = new Vector3(0.001f,-0.0001f,0f)/characters.Count;
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

        if (hasExprFaciais){
            if ("interrogativa" == json.exprFaciais.First().Value && glosasIndex == (Int32.Parse(split[1])-1) && characters.Count == 0){
                StartCoroutine(Interrogativa(fingerSpellClips[character].length)); // play animations for facial expressions
            }
        }

        animationEvent = new AnimationEvent();
        animationEvent.time = fingerSpellClips[character].length;
        // Debug.Log("timeee");
        // Debug.Log(fingerSpellClips[character].length);
        if (fingerSpellClips.ContainsKey(glosa) && json.glosas.Count != 0|| characters.Count == 0 && json.glosas.Count != 0) {
            json.glosas.RemoveAt(0);
            glosasIndex += 1;
            animationEvent.functionName = "Animating";
            // basicIK.last = true;
        }
        Debug.Log(json.glosas.Count);
        if (json.glosas.Count == 0){
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
        return Regex.Replace(word.Normalize(NormalizationForm.FormD), @"[^A-Za-z 0-9 \.,\?'""!@#\$%\^&\*\(\)-_=\+;:<>\/\\\|\}\{\[\]`~]*", string.Empty).Trim();    

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
}
