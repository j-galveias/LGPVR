using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using TMPro;
using Microsoft.VisualBasic;

// [System.Serializable]
// public class AnimationEvent : UnityEvent<List<string>> { }

public class MainAnimation : MonoBehaviour
{
    public GameObject leftCanvas;
    public GameObject rightCanvas;
    public GameObject cylinder;
    public GameObject mirror;

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
    Dictionary<string, List<AnimationClip>> compostosList = new Dictionary<string, List<AnimationClip>>();
    Dictionary<string, bool> mouthingAnim = new Dictionary<string, bool>();
    string[] facialExprWithMouthing = new string[]{
        "Neutra", "Alegre", "F021", "F022", "F023", "F029", "Olhos fechados", "Olhos Semifechados",
        "Sobrancelhas Contraídas", "Triste", "Tristucha", "Zangar", "Zangar Movimento"
    };
    Dictionary<string, int> animationUtils = new Dictionary<string, int>();
    Dictionary<string, AnimationClip> fingerSpellClips = new Dictionary<string, AnimationClip>();
    Dictionary<string, AnimationClip> expFacClips = new Dictionary<string, AnimationClip>();
    Dictionary<string, List<Vector3>> rightHandPos = new Dictionary<string, List<Vector3>>();
    Dictionary<string, List<Vector3>> leftHandPos = new Dictionary<string, List<Vector3>>();
    Dictionary<string, List<AnimationClip>> gestos_compostos = new Dictionary<string, List<AnimationClip>>();

    // Dictionary<string, string> expFacAnimated = new Dictionary<string, string>();
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
    int glosasIndex, glosasIndexAux, estados;
    TranslatorInfo json;
    AnimatedSignMini jsonInfo;
    public GameObject replay_button;
    public TMP_Text sentence;
    public TMP_Text frase_pensar;
    public TMP_Text text;
    public Button button;
    public GameObject character;
    public GameObject head;
    public Toggle toggle;

    public Toggle toggle_mouthing;

    public Toggle toggle_hand;

    // public Client client;

    BasicIK basicIK;
    Transform bone;
    List<bool> repetition = new List<bool>();
    Vector3 defaultPosition;
    AhoCorasick.Trie trie;
    List<string> glosas = new List<string>();
    List<string> glosas_copy = new List<string>();

    bool hasExprFaciais = false;
    string[] split = null;

    bool mouthing_bool = false;

    //Muscles
    public enum AllMuscles : int
    {
        LEFT_SHOULDER_DOWN_UP = 37,
        LEFT_SHOULDER_FRONT_BACK = 38,
        RIGHT_SHOULDER_DOWN_UP = 46,
        RIGHT_SHOULDER_FRONT_BACK = 47,
    }

    public void Start()
    {
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
        character.transform.localPosition += new Vector3(0, (float)0.9, 0);
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

        foreach (var clip in getClips)
        {
            string animation = clip.name.Normalize(NLP.NORMALIZATION);

            if (!animationClips.ContainsKey(removeAccents(animation.ToUpper())))
            {
                if (clip is AnimationClip)
                {
                    AnimationClip animClip = (AnimationClip)clip;
                    //var animation = removeAccents(animClip.name.ToUpper().Replace("GESTO_", ""));
                    animation = NLP.RemoveAccents(NLP.StandardizeName(animation)).ToUpper();
                    animationClips.Add(animation, animClip);
                    // Debug.Log(animation);
                    // Debug.Log(animClip.name);

                    // animationClipInfo.Add("facial_exp", );

                    if (!animation.Contains("NAO") && !animation.Contains("CHOCAR"))
                    {
                        trie.Add(animation);
                        int util = animation.Contains(' ') ? animation.Split(' ').Length : 1;
                        // Debug.Log(util);
                        animationUtils.Add(animation, util);
                    }

                    // Debug.Log("nameeeeeee");
                    // Debug.Log(animClip.name);
                    // string content = System.IO.File.ReadAllText("Assets/Resources/Animations/Signs/" + animClip.name + ".json");
                    // JsonSerializer.Deserialize<AnimatedSignMini>(jsonString);

                    // foreach (var line in content.Split(new string[] { "atribute: " }, StringSplitOptions.None)) Debug.Log(line);
                }
                else if (animation.EndsWith(AnimatedSign.MINI_SUFFIX))
                {
                    // Debug.Log("JSONNNNN");
                    // Debug.Log(AnimatedSign.MINI_SUFFIX);
                    // Debug.Log("NAMEE");
                    animation = animation.RemoveSuffix(AnimatedSign.MINI_SUFFIX);
                    animation = NLP.RemoveAccents(NLP.StandardizeName(animation)).ToUpper();
                    // Debug.Log(animation);
                    if (!rightHandPos.ContainsKey(animation))
                    {
                        // string content = System.IO.File.ReadAllText(clip.name + ".json");
                        var jsonString = Regex.Replace(clip.ToString(), ":\\s*([-1234567890\\.E]+)", ":\"$1\"");
                        jsonString = Regex.Replace(jsonString, ":\\s*(true|false)", ":\"$1\"");
                        jsonInfo = JsonSerializer.Deserialize<AnimatedSignMini>(jsonString);

                        List<Vector3> hand_pos_right = new List<Vector3>();
                        List<Vector3> hand_pos_left = new List<Vector3>();

                        hand_pos_right.Add(jsonInfo.rightHandPositions[0].GetPos());
                        hand_pos_right.Add(jsonInfo.rightHandPositions[jsonInfo.rightHandPositions.Count - 1].GetPos());
                        hand_pos_left.Add(jsonInfo.leftHandPositions[0].GetPos());
                        hand_pos_left.Add(jsonInfo.leftHandPositions[jsonInfo.leftHandPositions.Count - 1].GetPos());

                        rightHandPos.Add(animation, hand_pos_right);
                        leftHandPos.Add(animation, hand_pos_left);

                        HashSet<string> facialExpressions = jsonInfo.GetFacialExpressionSet();

                        // Saves whether mouthing can be done or not --> only if facial expression is not in the lower part of the face
                        mouthingAnim.Add(animation, false);

                        foreach (string face in facialExpressions)
                        {
                            mouthingAnim[animation] = facialExprWithMouthing.Contains(face);
                        }
                    }
                }
            }
        }

        CompositeUtterances("Animations/Compostos/");

        trie.Build();

        // var filepath = Application.streamingAssetsPath + "/Positions.csv";
        // using (var writer = new StreamWriter(filepath)) {

        //     writer.WriteLine("SignName, FirstKeyframe, LastKeyframe");

        //     foreach (var righpos in rightHandPos.Keys) {
        //         Debug.Log("WRITEEE: " + rightHandPos[righpos][0]);
        //         if (rightHandPos[righpos][0] != new Vector3(0,0,0)){
        //             Debug.Log("YESSSS");
        //             writer.WriteLine(righpos + ","  + rightHandPos[righpos][0].ToString("f3") + "," + rightHandPos[righpos][1].ToString("f3"));
        //         }
        //     }
        // }

        // File.Copy(Application.dataPath + "/" + "Resources/Animations/Signs/afia.anim", Application.dataPath + "/" + "Resources/Animations/Signs/afia_new.anim");

        // CREATE ANIM FILEEEE!!!
        // var file = System.IO.File.ReadAllText(Application.dataPath + "/" + "Resources/Animations/Signs/afia.anim");

        // System.IO.File.WriteAllText(Application.dataPath + "/" + "Resources/Animations/Signs/afia_new.anim", file);

        //GESTOS COMPOSTOS
        // var gestos_compostos_file = Resources.Load("Animations/gestos_compostos");
        // Debug.Log(gestos_compostos_file);
        // var jsonFile = JsonSerializer.Deserialize<GestosCompostos>(gestos_compostos_file.ToString());

        // Debug.Log(jsonFile);

        // Debug.Log("JSONNN");
        // Debug.Log(jsonFile.gestos_compostos);

        // foreach(var gesto_composto in jsonFile.gestos_compostos) {
        //     Debug.Log("gestooo");
        //     Debug.Log(gesto_composto);
        //     Debug.Log(gesto_composto.Key);
        //     List<AnimationClip> animFiles = new List<AnimationClip>();
        //     foreach (var gesto in gesto_composto.Value) {
        //         Debug.Log(gesto);
        //         animFiles.Add(animationClips[gesto.ToUpper()]);
        //     }
        //     gestos_compostos.Add(gesto_composto.Key.ToUpper(), animFiles);
        // }



        // string[] folders = Directory.GetDirectories(Application.dataPath + "/" + "Resources/Animations/GestCompostos/");
        // foreach (var folder in folders) {
        //     Debug.Log("FOLDERRR " + folder);
        //     string[] folderName = folder.Split('/');
        //     var gesto_composto = removeAccents(folderName[folderName.Length - 1].ToUpper().Replace("GESTO_", ""));
        //     trie.Add(gesto_composto);
        //     int util = gesto_composto.Contains(' ') ? gesto_composto.Split(' ').Length : 1;
        //     // Debug.Log(util);
        //     animationUtils.Add(gesto_composto, util);
        //     List<AnimationClip> animFiles = new List<AnimationClip>();
        //     var files = Resources.LoadAll("Animations/GestCompostos/" + folderName[folderName.Length - 1] + "/");

        //     foreach (var clip in files){ 
        //         if (clip is AnimationClip){
        //             animFiles.Add((AnimationClip) clip);
        //         }
        //     }

        //     gestos_compostos.Add(gesto_composto, animFiles);
        // }

        // var getCompostos = Resources.LoadAll("Animations/GestCompostos/");

        // foreach(var folder in getCompostos) {
        //     Debug.Log("FOLDERSSS " + folder);
        // }

        // StartCoroutine(GetSigns());

        var getFingerSpelling = Resources.LoadAll("Animations/Fingerspelling/");

        foreach (var clip in getFingerSpelling)
        {
            string animation = clip.name.Normalize(NLP.NORMALIZATION);

            if (clip is AnimationClip)
            {
                AnimationClip animClip = (AnimationClip)clip;
                fingerSpellClips.Add((animation.ToUpper()).Replace("GESTO_", ""), animClip);
            }
            else if (clip.name.EndsWith(AnimatedSign.MINI_SUFFIX))
            {
                // Debug.Log("JSONNNNN");
                // Debug.Log(AnimatedSign.MINI_SUFFIX);
                // Debug.Log("NAMEE");
                animation = animation.RemoveSuffix(AnimatedSign.MINI_SUFFIX);
                animation = removeAccents(animation.ToUpper().Replace("GESTO_", ""));
                // Debug.Log(animation);
                if (!rightHandPos.ContainsKey(animation))
                {
                    // string content = System.IO.File.ReadAllText(clip.name + ".json");
                    var jsonString = Regex.Replace(clip.ToString(), ":\\s*([-1234567890\\.E]+)", ":\"$1\"");
                    jsonString = Regex.Replace(jsonString, ":\\s*(true|false)", ":\"$1\"");
                    jsonInfo = JsonSerializer.Deserialize<AnimatedSignMini>(jsonString);

                    List<Vector3> hand_pos_right = new List<Vector3>();
                    List<Vector3> hand_pos_left = new List<Vector3>();

                    hand_pos_right.Add(jsonInfo.rightHandPositions[0].GetPos());
                    hand_pos_right.Add(jsonInfo.rightHandPositions[jsonInfo.rightHandPositions.Count - 1].GetPos());
                    hand_pos_left.Add(jsonInfo.leftHandPositions[0].GetPos());
                    hand_pos_left.Add(jsonInfo.leftHandPositions[jsonInfo.leftHandPositions.Count - 1].GetPos());

                    rightHandPos.Add(animation, hand_pos_right);
                    leftHandPos.Add(animation, hand_pos_left);
                }
            }
        }

        var getExprs = Resources.LoadAll("Animations/FacialExpr/Sintaticas/");

        foreach (var clip in getExprs)
        {
            if (clip is AnimationClip)
            {
                AnimationClip animClip = (AnimationClip)clip;
                expFacClips.Add(animClip.name.Normalize(NLP.NORMALIZATION), animClip);
            }
        }

        defaultPosition = basicIK.rightHandPosition.position;
    }

    void CompositeUtterances(string path)
    {
        AnimationClip[] compostosSigns = Resources.LoadAll<AnimationClip>(path);

        Dictionary<string, AnimationClip> compostosSignsDict = new Dictionary<string, AnimationClip>();

        foreach (var clip in compostosSigns)
        {
            string animation = clip.name.Normalize(NLP.NORMALIZATION).ToUpper();

            if (!compostosSignsDict.ContainsKey(animation))
            {
                AnimationClip animClip = (AnimationClip)clip;
                compostosSignsDict.Add(animation, animClip);
            }
        }
        compostosList = ReadCompositList("Animations/Compostos", compostosSignsDict);
    }

    public Dictionary<string, List<AnimationClip>> ReadCompositList(string filePath, Dictionary<string, AnimationClip> compostosSignsDict)
    {
        Dictionary<string, List<AnimationClip>> result = new Dictionary<string, List<AnimationClip>>();
        TextAsset compostos = Resources.Load<TextAsset>(filePath);
        // string[] lines = File.ReadAllLines(filePath);

        string[] lines = Regex.Split(compostos.text, "\n");

        Debug.Log(lines);

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            // Debug.Log(parts[0]);
            // Debug.Log(parts[1]);
            if (parts.Length > 1)
            {
                string[] components = Regex.Replace(parts[1], @"\s+", "").Split('+');

                List<AnimationClip> clipsList = new List<AnimationClip>();

                foreach (string component in components)
                {

                    string componentNorm = component.Normalize(NLP.NORMALIZATION).ToUpper();

                    if (compostosSignsDict.ContainsKey(componentNorm))
                    {
                        clipsList.Add(compostosSignsDict[componentNorm]);
                    }
                    else if (animationClips.ContainsKey(componentNorm))
                    {
                        clipsList.Add(animationClips[componentNorm]);
                    }
                }

                result[parts[0].Normalize(NLP.NORMALIZATION).ToUpper()] = clipsList;
                // break;
            }
        }
        return result;
    }


    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Stop") && animator.GetCurrentAnimatorStateInfo(2).IsName("idle_gestos")) Reset();
        // animator.SetBool("Blink", animator.GetCurrentAnimatorStateInfo(6).IsName("idle") && animator.GetCurrentAnimatorStateInfo(4).IsName("idle"));
        // animator.SetBool("Blink", animator.GetCurrentAnimatorStateInfo(1).IsName("idle_gestos"));
        // basicIK.last = !(basicIK.last && (animator.GetCurrentAnimatorStateInfo(0).IsName("State0") || animator.GetCurrentAnimatorStateInfo(0).IsName("State1")));
        // Debug.Log(basicIK.blocked);
        // if (animator.GetBool("Pensar")) animator.CrossFadeInFixedTime("pensar", 0.5f, 6, 0.2f);

        basicIK.glosasIndex = glosasIndex;

        mouthing_bool = toggle_mouthing.isOn;

        animator.SetBool("Mirror", toggle_hand.isOn);
    }

    public void Animate(string serverMessage)
    {
        try
        {
            // text.gameObject.SetActive(false);
            text.text = "";
            glosasIndex = 0;
            glosasIndexAux = 0;
            estados = 0;
            // hasExprFaciais = false;
            split = null;
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

            animator.SetBool("Pensar", false);

            Animating();
        }
        catch (Exception e)
        {
            // Debug.Log(e.Message);
            // Debug.Log("ERROO222");
            StopAnim("ERRO: animate");
        }
        // animationEvent = new AnimationEvent();
        // animationEvent.time = 0.8f;
        // animationEvent.functionName = "Animating";
        // animatorOverrideController["_tempAnim"].AddEvent(animationEvent);
        // events.Add(animatorOverrideController["_tempAnim"]);
    }

    void AhoCorasick(List<string> json_glosas)
    {
        try
        {

            // List<string> newLista = new List<string>();
            // foreach (var glosa in json_glosas) newLista.Add(glosa);

            List<string> matches = trie.Find(removeAccents(string.Join(" ", json_glosas))).ToList();

            // List<string> finalLista = new List<string>();

            List<string> matchesRemoveDupli = matches.Distinct().ToList();

            // Debug.Log("AhoCorasickkk1111111111111");
            foreach (var final in matchesRemoveDupli) Debug.Log(final);

            // Debug.Log("AhoCorasickkk");

            foreach (string glosa in json_glosas)
            {
                // Debug.Log(glosa);
                matches = new List<string>();
                foreach (string match in matchesRemoveDupli)
                {
                    if (match.Contains(' '))
                    {
                        string[] split = match.Split(' ');
                        if (split.Contains(removeAccents(glosa))) matches.Add(match);
                    }
                    else if (match == removeAccents(glosa))
                    {
                        matches.Add(match);
                    }
                }
                if (matches.Count == 0) // gesto não existe
                    glosas.Add(glosa);
                // só existe um match com o algoritmo
                else if (matches.Count == 1)
                {
                    if (glosas.Count == 0 || glosas.Count > 0 && glosas[glosas.Count - 1] != matches[0]) // acrescenta se a anteriro não for igual
                        glosas.Add(matches[0]);
                }
                else
                { // tem que escolher o gesto com base nas utilities
                    var values = matches.Where(k => animationUtils.ContainsKey(k)).Select(k => animationUtils[k]); //get utils for each match
                    Dictionary<string, int> matchUtils = matches.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v); // dict with matches and their utils
                    var key = matchUtils.Aggregate((l, r) => l.Value > r.Value ? l : r).Key; //get match with the highest util
                    if (glosas.Count == 0 || glosas.Count > 0 && glosas[glosas.Count - 1] != key) glosas.Add(key);
                }
            }

            // Debug.Log("finallll matchesss");

            // glosas = glosas.Distinct().ToList();

            // foreach(var final in glosas) Debug.Log(final);
            // Debug.Log("DONEEEE");
            glosas_copy = glosas.ToList();
        }
        catch
        {
            // Debug.Log("ERROO222");
            StopAnim("ERRO: ahocorasick");
        }
    }

    void Animating()
    {
        try
        {
            basicIK.ikActive = false;
            basicIK.first = true;
            // basicIK.last = false;
            basicIK.rightHandPosition.position = defaultPosition;
            repetition = new List<bool>();

            if (events.Count != 0)
            {
                events[0].events = new AnimationEvent[0];
                events.RemoveAt(0);
            }

            var glosa = glosas[0];
            // Debug.Log(hasExprFaciais);

            text.gameObject.SetActive(toggle.isOn);
            if (text.text == "") text.text += glosa.ToUpper();
            else text.text += " " + glosa.ToUpper();
            //text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
            // Debug.Log("indiceeeeee");
            // Debug.Log(glosasIndex);


            // Debug.Log("lengthhhh glosa");
            // Debug.Log(glosa.Length);

            // Duration of the gesture animation
            // float duration = animationClips.ContainsKey(glosa) ? animationClips[glosa].length : (3f * json.fonemas[glosasIndex].Count);

            // // duration = (duration<=(1f* json.fonemas[glosasIndex].Count)) ? (1f * json.fonemas[glosasIndex].Count) : duration;

            bool animateMouthing = mouthingAnim.ContainsKey(removeAccents(glosa)) ? mouthingAnim[removeAccents(glosa)] : true;

            animator.SetBool("idle_viseme", !animateMouthing);
            if (compostosList.ContainsKey(removeAccents(glosa)))
            {
                glosas.RemoveAt(0);
                List<string> newGlosas = new List<string>();
                json.adv_cond.RemoveAt(glosasIndex);
                json.fonemas.RemoveAt(glosasIndex);
                json.gestos_compostos.RemoveAt(glosasIndex);
                json.glosas.RemoveAt(glosasIndex);
                json.pausas.RemoveAt(glosasIndex);
                for (int i = 0; i < compostosList[removeAccents(glosa)].Count; i++)
                {
                    glosas.Insert(i, compostosList[removeAccents(glosa)][i].name.ToUpper());

                    json.adv_cond.Insert(glosasIndex + i, "false");
                    json.fonemas.Insert(glosasIndex + i, new List<string>());
                    json.gestos_compostos.Insert(glosasIndex + i, i == compostosList[removeAccents(glosa)].Count - 1 ? "false" : "true");
                    json.glosas.Insert(glosasIndex + i, compostosList[removeAccents(glosa)][i].name.ToUpper());
                    json.pausas.Insert(glosasIndex + i, i == compostosList[removeAccents(glosa)].Count - 1 ? "frase" : "false");
                }
                //newGlosas.AddRange(glosas);
                //glosas = newGlosas;
                glosa = glosas[0];
            }
            if (animationClips.ContainsKey(removeAccents(glosa)))
            {
                glosa = removeAccents(glosa);
                // if (hasExprFaciais){
                //     if ("interrogativa" == json.exprFaciais.First().Value && glosasIndex == (Int32.Parse(split[1])-1)){
                //         StartCoroutine(Interrogativa(animationClips[glosa].length)); // play animations for facial expressions
                //     }
                // }
                // animator.speed = 0.8f;
                animator.speed = 1f;
                animatorOverrideController["_signAnim" + estados % 2] = animationClips[glosa];
                animator.runtimeAnimatorController = animatorOverrideController;

                // foreach (var line in lines) Debug.Log(line);

                // AnimationState state = animator.GetCurrentAnimatorState(0);
                // state.enabled = true;
                // state.normalizedTime = (1.0f/totalAnimationTime) * specificFrame;
                // animatorOverrideController.Sample();

                //Muda a duração da transição se for um gesto composto
                float dur_value = 0.5f; //optimal transition duration based on study
                float trans_duration;
                float offset;
                if (glosasIndexAux > 0)
                {
                    if (animationClips.ContainsKey(removeAccents(glosas_copy[glosasIndexAux - 1])))
                        dur_value = dynamicTrans(glosa, removeAccents(glosas_copy[glosasIndexAux - 1]));
                    else
                    {
                        dur_value = dynamicTrans(glosa, removeAccents(glosas_copy[glosasIndexAux - 1]).Last().ToString());
                    }
                }
                trans_duration = bool.Parse(json.gestos_compostos[glosasIndex]) ? 0.2f : dur_value;
                // Debug.Log(glosa);
                offset = animationClips[glosa].length <= 1.5f ? 1f - trans_duration : 1.2f - trans_duration;
                offset = bool.Parse(json.gestos_compostos[glosasIndex]) ? 1.3f - trans_duration : offset;
                offset = glosasIndexAux > 0 && !animationClips.ContainsKey(glosas_copy[glosasIndexAux - 1]) ? 1f - trans_duration : offset;
                // offset = 1f - trans_duration;
                animator.SetBool("Animating", true);

                // Debug.Log("durationnn: " + trans_duration);

                if (estados > 0)
                    animator.CrossFadeInFixedTime("State" + estados % 2, trans_duration, 2, offset);

                // MOUTHING
                if (animateMouthing && mouthing_bool)
                {
                    float duration = 0.4f * json.fonemas[glosasIndex].Count > animationClips[glosa].length - trans_duration ? animationClips[glosa].length - trans_duration : 0.4f * json.fonemas[glosasIndex].Count;

                    // // Duration each sillable in the glosa --> gesture duration / number of sillables
                    // float trans_duration = (glosasIndex == 0) ? 1.6f : 1f;
                    // trans_duration = (duration <= 1.6f) ? 0.8f : trans_duration; 
                    float sil_duration = (duration) / json.fonemas[glosasIndex].Count;

                    StartCoroutine(mouthing(sil_duration, trans_duration - 0.2f));
                }

                //Adiciona expressão para a clausula condicional
                if (bool.Parse(json.adv_cond[glosasIndex])) animator.CrossFadeInFixedTime("Somb_levantadas", trans_duration, 4, offset);
                else animator.CrossFadeInFixedTime("idle", trans_duration, 4, offset);

                if (json.exprFaciais.Count != 0) animateFacialExpressions(trans_duration); //Has facial expressions and animates them

                nextSign(glosa);


                // bool flag = false;
                // if (estados % 2 == 0) flag = true;
                // animator.SetBool("Animating3", flag);
                // animator.SetBool("Animating2", !flag);
            }
            else Datilologia(glosa);
        }
        catch
        {
            // Debug.Log("ERRO");
            StopAnim("ERRO: animating");
        }
    }

    float dynamicTrans(string glosa, string previous)
    {
        try
        {
            // Debug.Log("PREVIOUSSSS");
            // Debug.Log(previous);
            // if (gestos_compostos.ContainsKey(previous))
            //     previous = gestos_compostos[previous][gestos_compostos[previous].Count - 1].name.ToUpper();
            // Debug.Log(previous);

            // Debug.Log("POSSS1 "+ leftHandPos[previous][1]);
            // Debug.Log("POSSS222 " + leftHandPos[glosa][0]);
            float diff_value_right = (rightHandPos[glosa][0] - rightHandPos[previous][1]).sqrMagnitude;
            float diff_value_left = (leftHandPos[glosa][0] - leftHandPos[previous][1]).sqrMagnitude;
            // Debug.Log("diff_valueeee " + diff_value_right);
            // Debug.Log("diff_valueeee_leftttt " + diff_value_left);
            float percentage;
            if (diff_value_left > 0.1f)
                percentage = ((diff_value_right + diff_value_left) - 0.01f) / (0.3f - 0.01f);
            else
                percentage = ((diff_value_right + diff_value_left) - 0.01f) / (0.15f - 0.01f);
            // Debug.Log("percentageeee " + percentage);
            float dur_value = Mathf.Lerp(0.3f, 0.7f, percentage);
            // Debug.Log("dur_value " + dur_value);

            return dur_value;
        }
        catch
        {
            // Debug.Log("ERROO");
            // StopAnim("ERRO: dynamic transitions");
            return 0.5f;
        }
    }

    // IEnumerator animateGestoComposto(string glosa) {
    //     for (var i = 0; i < gestos_compostos[glosa].Count; i ++) {
    //         Debug.Log(gestos_compostos[glosa][i]);
    //         Debug.Log(estados);
    //         animatorOverrideController["_signAnim" + estados%2] = gestos_compostos[glosa][i];
    //         animator.runtimeAnimatorController = animatorOverrideController;
    //         if (estados  == 0) animator.SetBool("Animating", true);
    //         else if (i == 0) {
    //             float dur_value = dynamicTrans(gestos_compostos[glosa][i].name.ToUpper());
    //             animator.CrossFadeInFixedTime("State" + estados%2, dur_value, 1, 1.3f-dur_value);
    //         }
    //         else if (i > 0)
    //             animator.CrossFadeInFixedTime("State" + estados%2, 0.23f, 1, 1.3f-0.2f);
    //         if (i == gestos_compostos[glosa].Count-1)
    //             nextSign(glosa, gestos_compostos[glosa][gestos_compostos[glosa].Count-1]);

    //         yield return new WaitForSeconds(gestos_compostos[glosa][i].length);
    //         estados += 1;
    //     }
    //     // Debug.Log("LASTTT");
    //     // Debug.Log(gestos_compostos[glosa][gestos_compostos[glosa].Count-1]);

    //     // animatorOverrideController["_signAnim" + estados%2] = gestos_compostos[glosa][gestos_compostos[glosa].Count-1];
    //     // animator.runtimeAnimatorController = animatorOverrideController;
    //     // animator.CrossFadeInFixedTime("State" + estados%2, 0.23f, 1, 1.3f-0.23f);

    //     // nextSign(glosa, gestos_compostos[glosa][gestos_compostos[glosa].Count-1]);
    // }

    void nextSign(string glosa)
    {
        // Debug.Log("intensidadeeeeeeee");
        // Debug.Log(json.adv_intensidade.Count);

        animationEvent = new AnimationEvent();
        animationEvent.time = animationClips[glosa].length;
        // animationEvent.time = glosasIndex < json.gestos_compostos.Count-1 && json.gestos_compostos[glosasIndex+1] ? ((animationClips[glosa].length-1f) / 4) + 1f: animationClips[glosa].length;

        //Adiciona expressão e altera velocidade & repetição para o adverbio de intensidade MUITO
        // animator.SetBool("Adv_Intensidade", json.adv_intensidade[glosasIndex] == "muito");
        // animator.SetFloat("Speed", 1f);
        // if (json.adv_intensidade[glosasIndex] == "muito") {
        //     Debug.Log("yayyyyyyy");
        //     animator.SetFloat("Speed", 2f);
        //     // animator.speed = 1.3f;
        //     // animator.SetFloat("Speed", 1.5f);
        //     // animationClips[glosa].wrapMode = WrapMode.Loop;
        //     animationEvent.functionName = "repeteAnimation";
        //     animationEvent.time = animationClips[glosa].length;
        //     animationEvent.stringParameter = glosa + " " + estados.ToString() + " " + json.pausas[glosasIndex];
        // }
        // animator.SetBool("Repeat", json.adv_intensidade[glosasIndex] == "muito");

        // int repetition = animator.GetBool("Repeat") ? 2 : 1;
        glosas.RemoveAt(0);

        if (glosas.Count == 0) animationEvent.functionName = "StopAnim";
        else if (json.pausas[glosasIndex] != "false")
        {
            animationEvent.stringParameter = json.pausas[glosasIndex];
            animationEvent.functionName = "animatePausa";
        }
        else animationEvent.functionName = "Animating";

        // contabiliza os gestos compostos para a contagem
        glosasIndex += animationUtils.ContainsKey(glosa) ? animationUtils[glosa] : 1;
        // sem contabilizar gestos compostos --> Glosas do Trie já contém gestos compostos
        glosasIndexAux += 1;
        estados += 1;

        animationClips[glosa].AddEvent(animationEvent);
        events.Add(animationClips[glosa]);
    }

    IEnumerator repeteAnimation(string glosa)
    {
        // if (events.Count != 0){
        //     events[0].events = new AnimationEvent[0];
        //     events.RemoveAt(0);
        // }
        string[] info = glosa.Split(' ');
        // Debug.Log("Entrouuuuuuuuuuuu");
        // animator.SetBool("Repeat", true);
        // animator.SetBool("Repeat", true);
        animator.CrossFadeInFixedTime("State" + Int32.Parse(info[1]) % 2, 0f, 2, 1f);

        // animator.SetBool("Repeat", true);
        yield return new WaitForSeconds(animationClips[info[0]].length);
        if (events.Count != 0)
        {
            events[0].events = new AnimationEvent[0];
            events.RemoveAt(0);
        }
        glosasIndex += animationUtils.ContainsKey(info[0]) ? animationUtils[info[0]] : 1;
        estados += 1;
        // Debug.Log("saiuuuuu");
        // Debug.Log(glosas.Count);
        if (glosas.Count == 0) StopAnim();
        else if (info[2] != "false") animatePausa(info[2]);
        else Animating();

    }

    IEnumerator animatePausa(string tipo)
    {
        string animation = (tipo == "frase") ? "Pausa_frase" : "Pausa_oracao";
        animator.CrossFadeInFixedTime(animation, 0.8f, 2, 0.2f);
        float duration = (tipo == "frase") ? 1f : 1f;
        animator.SetBool("ExpFacial0", false);
        animator.SetBool("ExpFacial1", false);
        yield return new WaitForSeconds(duration);
        Animating();
    }

    void animateFacialExpressions(float transDuration)
    {
        try
        {
            Transform RightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            foreach (var key in json.exprFaciais.Keys.ToList())
            {
                split = key.Split('-');
                if (Int32.Parse(split[0]) == glosasIndex)
                {
                    int index = 0;
                    foreach (var expression in json.exprFaciais[key])
                    {
                        animatorOverrideController["_tempExpFacial" + index] = expFacClips[expression];
                        animator.runtimeAnimatorController = animatorOverrideController;
                        if (expression.Contains("interrogativa") || expression.Contains("negativa"))
                        {
                            string expr_name = expression.Contains("interrogativa") ? "interrogativa" : "negativa";

                            animator.CrossFadeInFixedTime("Blendshapes_" + expr_name, transDuration - 0.3f, 3, 1f - transDuration - 0.3f);
                            animator.CrossFadeInFixedTime(expression, transDuration - 0.3f, 6 + index, 1f - transDuration - 0.3f);
                            // animator.SetBool("ExpFacial" + index, true);
                            animator.SetBool("Loop" + index, expression.Contains("negativa"));
                            index += 1;
                            // animator.SetBool("ExpFacial", true);
                        }
                        else animator.SetBool("olhos_franzidos", true);
                    }
                }
                if (Int32.Parse(split[1]) == glosasIndex)
                {
                    int index = 0;
                    foreach (var expression in json.exprFaciais[key])
                    {
                        if (expression.Contains("interrogativa") || expression.Contains("negativa"))
                        {
                            // animator.SetBool("ExpFacial" + index, false);
                            animator.CrossFadeInFixedTime("idle", transDuration, 3, 1f - transDuration);
                            animator.CrossFadeInFixedTime("idle", transDuration, 6 + index, 1f - transDuration);
                            animator.SetBool("Loop" + index, expression.Contains("negativa"));
                            index += 1;
                        }
                        else animator.SetBool("olhos_franzidos", false);
                    }
                    json.exprFaciais.Remove(key);
                }
            }
        }
        catch
        {
            // Debug.Log("ERROO");
            StopAnim("ERRO: facial expressions");
        }
    }

    IEnumerator mouthing(float sil_duration, float waitTime)
    {
        var index = glosasIndex;
        float trans_duration = (index == 0) ? 1f : waitTime;
        // trans_duration = json.gestos_compostos[glosasIndex] ? 0.1f : transDur;
        animator.CrossFadeInFixedTime("idle", 0.4f, 0, 0.2f);
        yield return new WaitForSeconds(trans_duration);

        foreach (var sillables in json.fonemas[index])
        {
            var viseme_index = 0;
            // Duration of each viseme in each sillables --> sillable duration / number os visemes
            float viseme_duration = (sil_duration) / sillables.Length;

            foreach (var viseme in sillables)
            {
                var offset = 1f - viseme_duration;
                animator.CrossFadeInFixedTime(viseme + "_viseme", viseme_duration, 0, offset);
                yield return new WaitForSeconds(viseme_duration);
                viseme_index++;
            }
        }
        animator.CrossFadeInFixedTime("idle", 0.4f, 0, 0.2f);
    }

    private IEnumerator Interrogativa(float time)
    {
        // yield return new WaitForSeconds(time);
        // animator.SetBool("ExpFacial", false);
        animator.SetBool("IntCabeca", true);
        yield return new WaitForSeconds(time + 0.5f);
        animator.SetBool("IntCabeca", false);
        animator.SetBool("ExpFacial0", false);
        animator.SetBool("ExpFacial1", false);
    }

    void Datilologia(string glosa)
    {
        try
        {
            if (events.Count != 0)
            {
                events[0].events = new AnimationEvent[0];
                events.RemoveAt(0);
            }

            // if (glosa == "") Animating();
            // Debug.Log("datilologiaaa");
            List<string> characters = new List<string>();

            // glosa = glosa.Normalize(NLP.NORMALIZATION);

            // foreach (var charr in glosa) Debug.Log(charr);

            string asciiStr = removeAccents(glosa);
            // Debug.Log(asciiStr);
            characters.AddRange(asciiStr.Select(c => c.ToString()));

            // Debug.Log(int.TryParse(glosa, out int n));
            bool repeat = (characters.Count > 1) ? (characters[0] == characters[1]) : false;
            // if(basicIK.ikActive) basicIK.rightHandPosition.transform.position += new Vector3(0.05f, -0.003f, 0);
            if (basicIK.ikActive)
            {
                basicIK.first = false;
            }
            if (basicIK.first) basicIK.difference = (new Vector3(0.001f, -0.0001f, 0f) / characters.Count);
            basicIK.ikActive = int.TryParse(glosa, out int n) ? (Int32.Parse(glosa) >= 20 || Int32.Parse(glosa) < 10 || characters.Count >= 3) : repeat;
            // basicIK.ikActive = true;
            if (repetition.Count == 2) repetition.RemoveAt(0);
            repetition.Add(repeat);
            // Debug.Log(repeat);

            // animator.speed = basicIK.ikActive ? 0.8f : 1f;

            var character = (fingerSpellClips.ContainsKey(glosa)) ? glosa : characters[0];
            // animator.speed = int.TryParse(character, out int i) ? 0.8f : 1f;
            animatorOverrideController["_signAnim" + estados % 2] = fingerSpellClips[character];
            animator.runtimeAnimatorController = animatorOverrideController;

            animator.SetBool("Animating", true);

            characters.RemoveAt(0);

            // animator.SetBool("Repeat", repeat);
            if (repetition.Count == 2 && repetition[0])
            {
                // StartCoroutine(Wait());
                // animator.SetBool("Animating3", false);
                // animator.SetBool("Animating2", false);
                animator.SetBool("Repeat", true);
            }
            else
            {
                float dur_value = int.TryParse(character, out int index) ? 0.7f : 0.7f; //optimal transition duration based on study

                if (basicIK.first && glosasIndexAux > 0)
                {
                    if (animationClips.ContainsKey(removeAccents(glosas_copy[glosasIndexAux - 1])))
                        dur_value = dynamicTrans(character, removeAccents(glosas_copy[glosasIndexAux - 1]));
                    else
                    {
                        dur_value = dynamicTrans(character, removeAccents(glosas_copy[glosasIndexAux - 1]).Last().ToString());
                    }
                }
                float trans_duration = dur_value;
                float offset = 1f - trans_duration;

                // Debug.Log("durationnn: " + trans_duration);
                // Debug.Log("offsetttt: " + offset);

                if (basicIK.first && mouthing_bool)
                {
                    // MOUTHING
                    float duration = (int.TryParse(character, out int integer) ? 0.3f : 0.7f) * json.fonemas[glosasIndex].Count; // 0.6f mas antes estava 0.4f

                    // // Duration each sillable in the glosa --> gesture duration / number of sillables
                    // float trans_duration = (glosasIndex == 0) ? 1.6f : 1f;
                    // trans_duration = (duration <= 1.6f) ? 0.8f : trans_duration; 
                    float sil_duration = (duration) / json.fonemas[glosasIndex].Count;

                    StartCoroutine(mouthing(sil_duration, trans_duration - 0.2f)); //trans_duration
                }

                if (estados > 0)
                    animator.CrossFadeInFixedTime("State" + estados % 2, trans_duration, 2, offset);
                // if (estados % 2 == 0) flag = true;
                // animator.SetBool("Animating3", flag);
                // animator.SetBool("Animating2", !flag);
                if (!repetition[0])
                {
                    animator.SetBool("Repeat", false);
                    estados += 1;
                }
                else
                {
                    animator.SetBool("Repeat", true);
                }
            }

            animationEvent = new AnimationEvent();
            animationEvent.time = fingerSpellClips[character].length;
            // Debug.Log("timeee");
            // Debug.Log(fingerSpellClips[character].length);

            if (fingerSpellClips.ContainsKey(glosa) && glosas.Count != 0 || characters.Count == 0 && glosas.Count != 0)
            {
                glosas.RemoveAt(0);
                glosasIndex += 1;
                glosasIndexAux += 1;
                animationEvent.functionName = "Animating";
                basicIK.ikActive = false;
                // basicIK.last = true;
            }
            else if (characters.Count != 0)
            {
                animationEvent.stringParameter = string.Join("", characters);
                animationEvent.functionName = "Datilologia";
                basicIK.last = false;
                basicIK.first = false;
            }

            if (glosas.Count == 0)
            {
                // repetition = new List<bool>();
                animationEvent.functionName = "StopAnim";
                // basicIK.last = true;
            }
            else if ((glosasIndex > 0 && json.pausas[glosasIndex - 1] != "false" || !animationClips.ContainsKey(removeAccents(glosas_copy[glosasIndexAux]))) && characters.Count == 0)
            {
                animationEvent.stringParameter = json.pausas[glosasIndex - 1] == "false" ? "oracao" : json.pausas[glosasIndex - 1];
                animationEvent.functionName = "animatePausa";
            }

            // Debug.Log("event addeddd");
            fingerSpellClips[character].AddEvent(animationEvent);
            events.Add(fingerSpellClips[character]);
            // Debug.Log(events[0]);
        }
        catch
        {
            // Debug.Log("ERROO");
            StopAnim("ERRO: datilologia");
        }
    }

    string removeAccents(string word)
    {
        word = Regex.Replace(word.Normalize(NLP.NORMALIZATION), @"[^A-Za-z 0-9 \.,\?'""!@#\$%\^&\*\(\)-_=\+;:<>\/\\\|\}\{\[\]`~]*", string.Empty).Trim();
        return word.Replace("-", " ").Replace("_", " ").Replace(" DE ", " ").Replace(" DO ", " ");
    }

    private IEnumerator Wait()
    {
        print("Begin wait() " + Time.time);
        yield return new WaitForSeconds(3f);

    }

    void StopAnim(string erro = null)
    {
        basicIK.ikActive = false;
        basicIK.last = true;
        // Debug.Log("stopp");
        animator.SetBool("Stop", true);
        animator.CrossFadeInFixedTime("idle", 0.5f, 6, 0.2f);
        animator.CrossFadeInFixedTime("idle", 0.5f, 7, 0.2f);
        animator.SetBool("ExpFacial0", false);
        animator.SetBool("ExpFacial1", false);
        animator.CrossFadeInFixedTime("idle", 0.5f, 3, 0.2f);
        animator.CrossFadeInFixedTime("idle", 0.5f, 4, 0.2f);
        animator.SetBool("olhos_franzidos", false);
        animator.ResetTrigger("Adv_Intensidade");

        while (events.Count != 0)
        {
            events[0].events = new AnimationEvent[0];
            events.RemoveAt(0);
        }

        // basicIK.ikActive = false;
        basicIK.first = false;

        // if (erro != null) client.error = erro;
    }

    void Reset()
    {
        /*leftCanvas.SetActive(true);
        rightCanvas.SetActive(true);
        sentence.gameObject.SetActive(true);*/
        sentence.text = "";
        /*button.gameObject.SetActive(true);
        replay_button.gameObject.SetActive(true);
        toggle.gameObject.SetActive(true);
        toggle_mouthing.gameObject.SetActive(true);
        toggle_hand.gameObject.SetActive(true);*/
        cylinder.SetActive(true);
        mirror.SetActive(true);
        text.text = "";
        animator.ResetTrigger("Animating");
        // animator.ResetTrigger("Animating2");
        // animator.ResetTrigger("Animating3");
        animator.ResetTrigger("Stop");
        // animator.SetLayerWeight(animator.GetLayerIndex ("idle"), 1);
        // animator.SetLayerWeight(animator.GetLayerIndex ("idle_animate"), 0);
        basicIK.rightHandPosition.position = defaultPosition;

        // for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
        // {
        //     if (i != skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("mClose"))
        //         skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
        // }
    }
}
