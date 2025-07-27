using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine.Events;

public class SandboxArenaController : MonoBehaviour
{
    public static List<string> UnitsToCreate = new();

    public void Start()
    {
        if(transform.Find("Sandbox Arena Select/Items/Regular/Items").childCount > 0)
        {
            Clean();
        }
        List<int> limits = new List<int> { 3, 6, 12};
        List<string> types = new List<string> { "Boss", "Elite", "Regular" };
        List<List<string>> prefabs = new List<List<string>> {Constants.BossEnemies, Constants.EliteEnemies, Constants.RegularEnemies};
        for(int i =0; i < 3; i++)
        {
            Transform container = transform.Find("Sandbox Arena Select/Items/" + types[i] + "/Items");
            foreach (string file in prefabs[i])
            {
                GameObject item = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_SandboxArenaItem")) as GameObject;
                item.transform.SetParent(container);
                item.name = file.Replace("(Clone)", "");
                item.transform.localScale = Vector3.one;
                item.transform.Find("Title/Text").GetComponent<TextMeshProUGUI>().text = Label.Get(file.Replace("Unit_", "Title_"));
                item.transform.Find("Portrait Body").GetComponent<Image>().sprite = Resources.Load("Sprites/Body Portrait/" + file.Replace("Unit_", ""), typeof(Sprite)) as Sprite;
                item.transform.Find("Portrait Face").GetComponent<Image>().sprite = Resources.Load("Sprites/Face Portrait/" + file.Replace("Unit_", ""), typeof(Sprite)) as Sprite;
                item.transform.Find("Controls/Slider").GetComponent<Slider>().maxValue = limits[i];
                if(item.name.Contains("Unit_Criminal_Spear")) {
                    item.transform.Find("Controls/Slider").GetComponent<Slider>().value = 3;
                }
            }
        }
    }

    public void Clean()
    {
        List<Transform> lists = new List<Transform> { transform.Find("Sandbox Arena Select/Items/Boss/Items"), transform.Find("Sandbox Arena Select/Items/Elite/Items"), transform.Find("Sandbox Arena Select/Items/Regular/Items") };
        foreach(Transform list in lists)
        {
            Utils.DestroyAllChildren(list);
        }
    }

    public void StartSandboxArena()
    {
        UnitsToCreate.Clear();
        GameController.Instance.CurrentSaveFile = new SaveFile("Sandbox", SaveFile.SaveFileTypeEnum.Challenge);
        List<string> types = new List<string> { "Boss",  "Elite", "Regular" };
        for (int i = 0; i < 3; i++)
        {
            Transform container = Utils.GetSceneRootObject("Start Screen").Find("Sandbox Arena/Sandbox Arena Select/Items/" + types[i] + "/Items");
            foreach (Transform item in container.transform)
            {
                float amount = item.Find("Controls/Slider").gameObject.activeSelf ? item.Find("Controls/Slider").GetComponent<Slider>().value : 0;
                for (int j = 0; j < amount; j++) {
                    UnitsToCreate.Add(item.name);
                }
            }
        }
        Utils.MoveIntoArea(true, "SandboxArena");
    }

    public static void OnStart()
    {
        foreach (Effect e in Player.Instance.CurrentEffects.ToArray())
        {
            e.EndThisEffect();
        }
        SaveFile.Instance.InitializeSaveFile();
        SaveFile.Instance.Level = 75;
        SaveFile.Instance.GameType = Constants.GameType.Arena;
        SaveFile.Instance.Difficulty = Constants.Difficulty.Regular;
        GameController.Instance.GameplayMode = Constants.GameplayMode.Regular;
        foreach (string unit_name in UnitsToCreate)
        {
            GameObject unit = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Unit/" + unit_name)) as GameObject;
            unit.transform.SetParent(Area.Instance.transform);
            Vector3 intended_position = new Vector2(3.5f, 0) + UnityEngine.Random.insideUnitCircle * new Vector2(2f, 2f);
            NavMeshHit closestHit;
            unit.GetComponent<Unit>().DisplayedName = unit.gameObject.name.Replace("(Clone)", "");
            unit.GetComponent<Actions>().IsFlipped = true;
            if (NavMesh.SamplePosition(intended_position, out closestHit, 500, 1))
            {
                unit.transform.position = new Vector3(closestHit.position.x, closestHit.position.y, 0);
            }
        }
        DebugController.Instance.GiveAllItems(0);
        //DebugController.Instance.ToggleSpamMode(1);
    }

}
