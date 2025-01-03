using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlagBehaviour : MonoBehaviour
{
    public string FlagId;
    public bool ActiveWhenHasFlag = false;
    public bool ActiveWhenNoFlag = false;
    public bool InactiveWhenHasFlag = false;
    public bool InactiveWhenNoFlag = false;
    public bool DestroyIfHasFlag = false;
    public bool DestroyIfNoFlag = false;
    public bool AddFlagOnDestroy = false;
    public bool RemoveFlagOnDestroy = false;
    public int StartsOnWeek = 0;
    public int StopsOnWeek = 99;

    public int OnlyWorkInCycle = 0;
    /// <summary>
    /// Higher priority overrides lower
    /// </summary>
    public int FlagPriority = 0;

    public void PerformFlagBehaviour() {
        if(transform.root.IsDestroyed() || (OnlyWorkInCycle != 0 && SaveFile.Instance.Cycle != OnlyWorkInCycle ) || SaveFile.Instance.Week <= StartsOnWeek || SaveFile.Instance.Week >= StopsOnWeek) {
            return;
        }
        bool hasFlag = SaveFile.Instance.Flags.Contains(Utils.GetFormattedFlag(FlagId));
        if((DestroyIfHasFlag && hasFlag) || (DestroyIfNoFlag && !hasFlag)) {
            Transform onDestroy = Utils.GetOnDestroyObject(transform);
            if(onDestroy != null) {
                onDestroy.gameObject.SetActive(true);
                onDestroy.SetParent(transform.root);
                if(onDestroy.childCount > 0) {
                    foreach(Transform child in onDestroy) {
                        child.transform.SetParent(transform.root);
                    }
                }
            }
            MonoBehaviour.Destroy(gameObject);
        }
        else if(gameObject.IsDestroyed() == false && ((ActiveWhenHasFlag && hasFlag ) || (ActiveWhenNoFlag && !hasFlag))){
            gameObject.SetActive(true);
        }
        else if(gameObject.IsDestroyed() == false && ((InactiveWhenHasFlag && hasFlag ) || (InactiveWhenNoFlag && !hasFlag))){
            gameObject.SetActive(false);
        }
    }

    public void OnDestroy() {
        if((OnlyWorkInCycle != 0 && SaveFile.Instance.Cycle != OnlyWorkInCycle ) || SaveFile.Instance.Week <= StartsOnWeek || SaveFile.Instance.Week >= StopsOnWeek) {
            return;
        }
        bool hasFlag = SaveFile.Instance.Flags.Contains(Utils.GetFormattedFlag(FlagId));
        if(AddFlagOnDestroy && !hasFlag) {
            SaveFile.Instance.AddFlag(Utils.GetFormattedFlag(FlagId));
        }
        else if(RemoveFlagOnDestroy && hasFlag) {
            SaveFile.Instance.Flags.Remove(Utils.GetFormattedFlag(FlagId));
        }
    }
}
