using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine;
using UnityEngine.UI;

public class InitializeHandGroups : MonoBehaviour
{
    public const string GROUP_FILE_PATH = "Assets\\HandPanel\\HandGroups\\groups.json";
    public const string GROUP_IMAGE_FILE_PATH = "Assets\\Resources\\HandPoseGroups\\";
    public const string HAND_IMAGE_FILE_PATH = "Assets\\Resources\\HandPoses\\";

    private const string MAIN_LIST_NAME = "Main List";

    public GameObject modelHandList;

    private Dictionary<string, InitializeHandButtons> everyList = new Dictionary<string, InitializeHandButtons>();
    private InitializeHandButtons mainList;
    private static Dictionary<string, HashSet<string>> poseUsedInSigns = new Dictionary<string, HashSet<string>>(); //Key is a Hand Pose, value is a set of Signs

    void Start()
    {
        InitSignHandPoses();

        string jsonString = File.ReadAllText(GROUP_FILE_PATH);
        var groupData = JsonSerializer.Deserialize<HandGroups>(jsonString);
        modelHandList.SetActive(false);

        mainList = CreateList(MAIN_LIST_NAME);

        foreach (var groupEntry in groupData)
        {
            string groupName = groupEntry.Key;
            var subgroup = groupEntry.Value;

            var list = CreateList(groupName);
            Button backButton = list.AddButton("Back", true, "Assets\\Resources\\back.png");
            backButton.onClick.AddListener(delegate () { SelectMainList(); });

            Button button = mainList.AddButton(groupName, false, GROUP_IMAGE_FILE_PATH + groupName + ".png");
            button.onClick.AddListener(delegate () { SelectList(list.gameObject); });


            foreach (var subgroupEntry in subgroup)
            {
                string mainPose = subgroupEntry.Key;
                var subPoses = subgroupEntry.Value;
                subPoses.Sort((s1, s2) => EditDistanceComparator(mainPose, s1, s2));
                List<string> signsInGroup = GetSignsInGroup(subPoses);

                button = list.AddButton(mainPose, false, HAND_IMAGE_FILE_PATH + mainPose + ".png");
                //button.onClick.AddListener(delegate () { SelectList(handPose); });
            }
        }
    }

    private List<string> GetSignsInGroup(List<string> poseGroup)
    {
        HashSet<string> dontRepeat = new HashSet<string>();
        List<string> result = new List<string>();

        foreach (string subpose in poseGroup)
        {
            var add = poseUsedInSigns[subpose].Except(dontRepeat);
            dontRepeat.Concat(add);
            result.AddRange(add);
        }

        return result;
    }

    private static void InitSignHandPoses()
    {
        var allSigns = Manager.GetAnimAllData();

        foreach (var entry in allSigns)
        {
            string signName = entry.Key;
            AnimatedSignData anim = entry.Value;

            AddToHandPoses(signName, anim.GetHandPoses());
        }
    }

    private static void AddToHandPoses(string signName, HashSet<string> handPoses)
    {
        foreach (string pose in handPoses)
        {
            if (!poseUsedInSigns.ContainsKey(pose))
                poseUsedInSigns[pose] = new HashSet<string>();

            poseUsedInSigns[pose].Add(signName);
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

    public void SelectMainList()
    {
        SelectList(mainList.gameObject);
    }

    public void SelectList(GameObject list)
    {
        HideAllLists();
        list.SetActive(true);
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
