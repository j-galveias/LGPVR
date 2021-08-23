using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    const string PATH_FINGERSPELL = "/Fingerspelling";
    const string PATH_SIGNS = "/Signs";

    public GameObject buttonModel;
    public Animator animator;
    public AnimationClip emptyAnim;

    private int signLayer;
    private string currentSign = "";
    private bool idle = true;
    private Coroutine idleCoroutine;
    private int repeat = 1;
    private bool doRepeat = false;

    private Dictionary<string, bool> hasPause = new Dictionary<string, bool>();
    private Dictionary<string, AnimatedSignData> animData = new Dictionary<string, AnimatedSignData>();

    //private Dictionary<string, Motion> animations = new Dictionary<string, Motion>();

    void Start()
    {
        signLayer = animator.GetLayerIndex("signLayer");
        InitializeButtons("Animations"+PATH_FINGERSPELL, true);
        InitializeButtons("Animations" + PATH_SIGNS);

        StartCoroutine(Blink(2f, 6f));
    }

    private void InitializeButtons(string path, bool pause=false)
    {
        buttonModel.SetActive(true);

        Object[] animationsArray = Resources.LoadAll<AnimationClip>(path);
        TextAsset[] jsonArray = Resources.LoadAll<TextAsset>(path);
        Dictionary<string, AnimationClip> animations = new Dictionary<string, AnimationClip>();
        Dictionary<string, TextAsset> jsonFiles = new Dictionary<string, TextAsset>();

        foreach (Object animFile in animationsArray)
            animations.Add(animFile.name, animFile as AnimationClip);

        foreach (TextAsset json in jsonArray)
            jsonFiles.Add(json.name, json);

        List<string> list = animations.Keys.ToList();
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


            string animName = animations[key].name;
            string shortName = animName.ToLower().StartsWith("gesto_") ? animName.Substring("gesto_".Length) : animName;

            newButton.name = animName;
            hasPause[animName] = pause;

            newButton.transform.GetChild(0).GetComponent<Text>().text = shortName;
            newButton.transform.SetParent(buttonModel.transform.parent);

            animations[key].wrapMode = WrapMode.Once;


            AnimatedSignData data = new AnimatedSignData();

            try
            {
                //TextAsset json = Resources.Load<TextAsset>(path + "/" + animName + ".json");
                //string jsonString = File.ReadAllText("Assets/Resources/"+path +"/"+animName+".json");
                //data = JsonConvert.DeserializeObject<AnimatedSignData>(jsonString);
                data = JsonSerializer.Deserialize<AnimatedSignData>(jsonFiles[key].text);
            }
            catch (System.Exception e)
            {
                Debug.Log("ERROR: Couldn't open " + animName + ".json");
                Debug.Log("Exception " + e.GetType() + " --- " + e.Message);
            }

            animData[animName] = data;

            //controller.AddMotion(animations[key] as AnimationClip, signLayer);
        }

        buttonModel.SetActive(false);
    }

    private IEnumerator Blink(float waitTimeMin, float waitTimeMax)
    {
        while (true)
        {
            float time = Random.Range(waitTimeMin, waitTimeMax);
            yield return new WaitForSeconds(time);
            animator.Play("blink");
        }
    }

    public void PlaySign(string sign)
    {
        // animator.SetFloat("speedParam", animData[sign].globalSpeed);
        // repeat = animData[sign].globalRepetitions;
        animator.CrossFadeInFixedTime(sign, 0.5f, -1, 0.5f);
        currentSign = sign;
        idle = false;
        doRepeat = false;

        if (idleCoroutine != null)
            StopCoroutine(idleCoroutine);
    }

    public int GetSignLayer()
    {
        return signLayer;
    }

    public void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(GetSignLayer());

        /*if (state.normalizedTime > 0.99f && !idle)
            animator.SetFloat("speedParam", 0.005f);
        else
          animator.SetFloat("speedParam", 1f);*/

        if (!state.IsName("signEmpty") && !idle)
        {

            if (state.normalizedTime < 0.5f)
            {
                doRepeat = true;
            }

            if (state.normalizedTime > 0.999f && doRepeat)
            {
                Debug.Log(repeat);
                repeat--;

                if (repeat > 0)
                {
                    animator.CrossFadeInFixedTime(currentSign, 0.3f, -1, 0.9f);
                    doRepeat = false;
                }
                else
                {
                    float waitTime = hasPause.ContainsKey(currentSign) && hasPause[currentSign] ? 1f : 0f;
                    idleCoroutine = StartCoroutine(BecomeIdle(waitTime));
                    idle = true;
                }
            }
        }
    }

    private IEnumerator BecomeIdle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.CrossFadeInFixedTime("signEmpty", 0.5f, -1, 0f);
    }
}