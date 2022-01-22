using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InitializeHandGroups : MonoBehaviour
{
    public const string GROUP_FILE_PATH = "Assets\\HandPanel\\HandGroups\\groups.json";
    public const string GROUP_IMAGE_FILE_PATH = "Assets\\Resources\\HandPoseGroups\\";
    public const string HAND_IMAGE_FILE_PATH = "Assets\\Resources\\HandPoses\\";
    public const string SIGN_IMAGE_FILE_PATH = "Assets\\Resources\\SignImages\\";

    private const string MAIN_LIST_NAME = "Main List";

    public GameObject modelHandList;
    public Manager manager;

    private Dictionary<string, InitializeHandButtons> everyList = new Dictionary<string, InitializeHandButtons>();
    //private InitializeHandButtons mainList;
    private MenuList mainList;
    private GameObject currentList = null;
    private static Dictionary<string, HashSet<string>> signsThatUsePose = new Dictionary<string, HashSet<string>>(); //Key is a Hand Pose, value is a set of Signs
    private HandGroupsJson groupData;

    void Start()
    {
        MenuList.handGroupManager = this;
        InitSignHandPoses();

        string jsonString = File.ReadAllText(GROUP_FILE_PATH);
        groupData = JsonSerializer.Deserialize<HandGroupsJson>(jsonString);
        modelHandList.SetActive(false);

        mainList = new MenuList(MAIN_LIST_NAME);
        mainList.AddButtonClose("Close", true, "Assets\\Resources\\back.png");

        //mainList = CreateList(MAIN_LIST_NAME);
        //currentList = mainList.gameObject;
        //currentList.SetActive(true);

        foreach (var groupEntry in groupData)
        {
            string groupName = groupEntry.Key;
            var subgroup = groupEntry.Value;

            MenuList list = new MenuList(groupName, mainList);
            MenuButton backButton = list.AddButtonBack("Back", true, "Assets\\Resources\\back.png");

            MenuButton button = mainList.AddButtonSelect(groupName, false, GROUP_IMAGE_FILE_PATH + groupName + ".png", list);


            /*var list = CreateList(groupName);
            Button backButton = list.AddButton("Back", true, "Assets\\Resources\\back.png");
            AddListenerSelectList(backButton, mainList);

            Button button = mainList.AddButton(groupName, false, GROUP_IMAGE_FILE_PATH + groupName + ".png");
            AddListenerSelectList(button, list);*/


            foreach (var subgroupEntry in subgroup)
            {
                string mainPose = subgroupEntry.Key;
                var subPoses = subgroupEntry.Value;
                subPoses.Sort((s1, s2) => EditDistanceComparator(mainPose, s1, s2));
                List<string> signsInGroup = GetSignsInGroup(subPoses);

                MenuList subList = new MenuList(mainPose, list);
                backButton = subList.AddButtonBack("Back", true, "Assets\\Resources\\back.png");

                button = list.AddButtonSelect(mainPose, false, HAND_IMAGE_FILE_PATH + mainPose + ".png", subList);

                /*var subList = CreateList(mainPose);
                backButton = subList.AddButton("Back", true, "Assets\\Resources\\back.png");
                AddListenerSelectList(backButton, list);

                button = list.AddButton(mainPose, false, HAND_IMAGE_FILE_PATH + mainPose + ".png");
                AddListenerSelectList(button, subList);*/

                foreach (string sign in signsInGroup) {
                    button = subList.AddButtonPlayAnimation(sign.ToUpper(), false, SIGN_IMAGE_FILE_PATH + sign + ".png", sign.ToUpper());
                    //button.GetComponent<AnimationButton>().playAnimation = true;
                }

                /*foreach (string sign in signsInGroup) {
                    button = subList.AddButton(sign.ToUpper(), false, SIGN_IMAGE_FILE_PATH + sign + ".png");
                    button.GetComponent<AnimationButton>().playAnimation = true;
                }*/

                //subList.gameObject.SetActive(false);
            }

            //list.gameObject.SetActive(false);
        }

        mainList.Instantiate();
        MenuButton.currentList = mainList;
    }

    private List<string> GetSignsInGroup(List<string> poseGroup)
    {
        HashSet<string> dontRepeat = new HashSet<string>();
        List<string> result = new List<string>();

        foreach (string subpose in poseGroup)
        {
            var add = GetSignsThatUsePose(subpose).Except(dontRepeat);
            var addList = add.ToList();
            dontRepeat.UnionWith(add);
            result.AddRange(addList);
        }

        return result;
    }

    private static void InitSignHandPoses()
    {
        var allSigns = Manager.GetAnimAllData();

        foreach (var entry in allSigns)
        {
            string signName = entry.Key;
            AnimatedSignMini anim = entry.Value;

            AddToHandPoses(signName, anim.GetHandPosesSet());
        }
    }

    private static HashSet<string> GetSignsThatUsePose(string poseName)
    {
        if (signsThatUsePose.ContainsKey(poseName))
            return signsThatUsePose[poseName];
        else
            return new HashSet<string>();
    }

    private static void AddToHandPoses(string signName, HashSet<string> handPoses)
    {
        foreach (string pose in handPoses)
        {
            if (!signsThatUsePose.ContainsKey(pose))
                signsThatUsePose[pose] = new HashSet<string>();

            signsThatUsePose[pose].Add(signName);
        }
    }

    private InitializeHandButtons CreateList(string listName)
    {
        string name = listName;
        int i = 2;

        while (everyList.ContainsKey(name))
        {
            name = listName + "_" + i++;
            Debug.Log("WARNING! There's more hand one list with the name " + name);
        }

        GameObject newList = Instantiate(modelHandList);
        newList.transform.SetParent(modelHandList.transform.parent);
        newList.name = name;
        newList.SetActive(true);
        everyList[name] = newList.GetComponent<InitializeHandButtons>();

        return everyList[name];
    }

    private void HideAllLists()
    {
        for (int i=0; i<transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /*public void SelectMainList()
    {
        SelectList(mainList.gameObject);
    }*/

    public void SelectList(GameObject list)
    {
        if (list != currentList)
        {
            GameObject oldList = currentList;
            currentList = list;
            list.SetActive(true);
            oldList.SetActive(false);
        }
    }

    private int EditDistanceComparator(string baseStr, string s1, string s2)
    {
        int d1 = NLP.LevenshteinDistance(baseStr, s1);
        int d2 = NLP.LevenshteinDistance(baseStr, s2);

        if (d1 < d2)
            return -1;

        if (d1 == d2)
            return 0;

        return 1;
    }
}
