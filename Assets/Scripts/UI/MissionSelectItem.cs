using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MissionSelectItem : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    Mission Mission;
    public void Start() {
        Mission = SaveFile.Instance.Missions.FirstOrDefault(m => m.MissionSelectGameObject == gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowMarker();
    }

    public void OnSelect(BaseEventData eventData)
    {
        ShowMarker();
    }

    public void ShowMarker() {
        Transform markers = Utils.GetSceneRootObject("Mission Select").Find("Markers");
        foreach(Transform marker in markers) {
            marker.gameObject.SetActive(marker.gameObject.name == Mission?.MapMarker);
            if(marker.gameObject.name == Mission?.MapMarker) {
                marker.GetComponent<Image>().sprite = Resources.Load("Sprites/UI/MarkerInactive", typeof(Sprite)) as Sprite;
            }
        }
    }

    public void ShowDetails() {
        MenuManager.Instance.ShowMissionDetails(gameObject);
    }
}
