/*===============================================================================
Copyright (c) 2024 PTC Inc. and/or Its Subsidiary Companies. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;

public class VuforiaStateInfo : MonoBehaviour
{
    public GameObject TextObject;

    const string ACTIVE_TARGETS_TITLE = "<b>Active Targets: </b>";

    readonly Dictionary<ObserverBehaviour, string> mObserversStatusMap = new ();

    public enum TargetType
    {
        IMAGE_TARGET,
        MODEL_TARGET
    }

    public TargetType desiredTargetType;

    void Start()
    {
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    void OnDestroy()
    {
        VuforiaApplication.Instance.OnVuforiaStarted -= OnVuforiaStarted;

        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.World.OnObserverCreated -= OnObserverCreated;
        }
        foreach (var observerBehaviour in mObserversStatusMap.Keys)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
            observerBehaviour.OnBehaviourDestroyed -= OnBehaviourDestroyed;
        }
    }

    void OnVuforiaStarted()
    {
        var observerBehaviours = VuforiaBehaviour.Instance.World.GetObserverBehaviours();

        foreach (var observerBehaviour in observerBehaviours.Where(MatchesDesiredType))
        {
            mObserversStatusMap.Add(observerBehaviour, GetStatusString(observerBehaviour.TargetStatus));
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
            observerBehaviour.OnBehaviourDestroyed += OnBehaviourDestroyed;
        }

        VuforiaBehaviour.Instance.World.OnObserverCreated += OnObserverCreated;
        UpdateText();
    }

    bool MatchesDesiredType(ObserverBehaviour observerBehaviour)
    {
        return desiredTargetType == TargetType.IMAGE_TARGET && observerBehaviour is ImageTargetBehaviour ||
               desiredTargetType == TargetType.MODEL_TARGET && observerBehaviour is ModelTargetBehaviour;
    }

    void OnObserverCreated(ObserverBehaviour observerBehaviour)
    {
        if (!MatchesDesiredType(observerBehaviour))
            return;
        
        if (mObserversStatusMap.ContainsKey(observerBehaviour))
            return;

        mObserversStatusMap.Add(observerBehaviour, GetStatusString(observerBehaviour.TargetStatus));
        observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        observerBehaviour.OnBehaviourDestroyed += OnBehaviourDestroyed;
    }

    void OnBehaviourDestroyed(ObserverBehaviour observerBehaviour)
    {
        observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        observerBehaviour.OnBehaviourDestroyed -= OnBehaviourDestroyed;
        mObserversStatusMap.Remove(observerBehaviour);
    }

    void OnTargetStatusChanged(ObserverBehaviour observerBehaviour, TargetStatus targetStatus)
    {
        mObserversStatusMap[observerBehaviour] = GetStatusString(targetStatus);
        UpdateText();
    }

    void UpdateText()
    {
        var targetStatusInfo = GetTargetsStatusInfo();

        var completeInfo = ACTIVE_TARGETS_TITLE;

        if (targetStatusInfo.Length > 0)
            completeInfo += $"\n{targetStatusInfo}";

        SampleUtil.AssignStringToTextComponent(TextObject ? TextObject : gameObject, completeInfo);
    }

    string GetStatusString(TargetStatus targetStatus)
    {
        return $"{targetStatus.Status} -- {targetStatus.StatusInfo}";
    }
    
    string GetTargetsStatusInfo()
    {
        var targetsAsMultiLineString = "";
        
        foreach (var targetStatus in mObserversStatusMap)
            targetsAsMultiLineString += "\n" + GetObserverDisplayName(targetStatus.Key) + ": " + targetStatus.Value;

        return targetsAsMultiLineString;
    }

    string GetObserverDisplayName(ObserverBehaviour behaviour)
    {
        return $"{behaviour.TargetName}_{behaviour.ID}";
    }
}
