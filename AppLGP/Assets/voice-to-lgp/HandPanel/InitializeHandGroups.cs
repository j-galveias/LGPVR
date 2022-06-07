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
    public const string GROUP_FILE_PATH = "handGroups";

    //These image folders, can have images or subfolders, BUT NOT BOTH
    public const string BACK_IMAGE_PATH = "Images/UI/back";
    public const string GROUP_IMAGE_FILE_PATH = "Images/HandPoseGroups/";
    public const string HAND_IMAGE_FILE_PATH = "Images/HandPoses/";
    public const string SIGN_IMAGE_FILE_PATH = "Images/SignImages/";

    private const string MAIN_LIST_NAME = "Main List";

    public static MenuList currentList;
    public static InitializeHandGroups singleton;

    public GameObject modelHandList;
    public Manager manager;
    public GameObject backButton;
    public GameObject closeButton;

    private Dictionary<string, InitializeHandButtons> everyList = new Dictionary<string, InitializeHandButtons>();
    //private InitializeHandButtons mainList;
    private MenuList mainList;
    private static Dictionary<string, HashSet<string>> signsThatUsePose = new Dictionary<string, HashSet<string>>(); //Key is a Hand Pose, value is a set of Signs
    private HandGroupsJson groupData;

    void Start()
    {
        singleton = this;

        MenuList.handGroupManager = this;
        InitSignHandPoses();

        var json = Resources.Load(GROUP_FILE_PATH);
        string jsonString = json.ToString().Normalize(NLP.NORMALIZATION);
        groupData = JsonSerializer.Deserialize<HandGroupsJson>(jsonString);
        modelHandList.SetActive(false);

        mainList = new MenuList(MAIN_LIST_NAME);

        foreach (var groupEntry in groupData)
        {
            string groupName = groupEntry.Key;
            var subgroup = groupEntry.Value;

            MenuList list = new MenuList(groupName, mainList);

            MenuButton button = mainList.AddButtonSelect(groupName, false, GROUP_IMAGE_FILE_PATH + groupName, list);


            foreach (var subgroupEntry in subgroup)
            {
                string mainPose = subgroupEntry.Key;
                mainPose = Path.GetFileName(mainPose);

                var subPoses = subgroupEntry.Value;
                subPoses.Sort((s1, s2) => EditDistanceComparator(mainPose, s1, s2));
                List<string> signsInGroup = GetSignsInGroup(subPoses);

                MenuList subList = new MenuList(mainPose, list);

                button = list.AddButtonSelect(mainPose, false, HAND_IMAGE_FILE_PATH + mainPose, subList);

                foreach (string sign in signsInGroup) {
                    button = subList.AddButtonPlayAnimation(sign.ToUpper(), false, SIGN_IMAGE_FILE_PATH + sign, sign.ToUpper());
                }
            }
        }

        mainList.Instantiate();
        currentList = mainList;
    }

    public void SelectList(MenuList list)
    {
        if (list != currentList)
        {
            MenuList oldList = currentList;
            currentList = list;

            if (list.obj == null)
                list.Instantiate();

            list.obj.SetActive(true);

            if (oldList.obj == null)
                oldList.Instantiate();

            oldList.obj.SetActive(false);
        }

        if (currentList.GetParent() == null) {
            backButton.SetActive(false);
            closeButton.SetActive(true);
        }
        else
        {
            backButton.SetActive(true);
            closeButton.SetActive(false);
        }
    }

    public void GoBack()
    {
        SelectList(currentList.GetParent());
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

    /*private InitializeHandButtons CreateList(string listName)
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
    }*/

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
