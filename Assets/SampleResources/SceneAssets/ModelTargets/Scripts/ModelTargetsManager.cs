/*===============================================================================
Copyright (c) 2024 PTC Inc. and/or Its Subsidiary Companies. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ModelTargetsManager : MonoBehaviour
{
    public enum ModelTargetMode
    {
        MODE_STANDARD,
        MODE_ADVANCED
    }

    [Header("Initial Model Target Mode")]
    [SerializeField] ModelTargetMode TargetMode = ModelTargetMode.MODE_STANDARD;
    [SerializeField] bool AutoActivate = false;

    [Header("Model Target Shared Augmentation")]
    [SerializeField] GameObject Augmentation = null;

    [Header("Model Target Behaviours")]
    [SerializeField] ModelTargetBehaviour ModelStandard = null;
    [SerializeField] ModelTargetBehaviour ModelAdvanced = null;

    readonly List<ModelTargetBehaviour> mAdvancedModelTargets = new List<ModelTargetBehaviour>();
    
    [Header("Advanced Model Targets")]
    [SerializeField] Canvas AdvancedModelTargetsUI = null;

    void Start()
    {
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    void LateUpdate()
    {
        if (TargetMode != ModelTargetMode.MODE_ADVANCED || mAdvancedModelTargets.Count == 0) 
            return;
        
        var areAllAdvancedTargetsInitializing = false;
        
        // Loop through a List of ModelTargetBehaviours checking the CurrentStatusInfo
        // to verify that all of them are in Initializing state.
        foreach (var mtb in mAdvancedModelTargets)
        {
            if (mtb && mtb.TargetStatus.StatusInfo != StatusInfo.INITIALIZING)
            {
                areAllAdvancedTargetsInitializing = false;
                break;
            }

            areAllAdvancedTargetsInitializing = true;
        }

        // If all of the MTBs are initializing
        EnableSymbolicTargetsUI(areAllAdvancedTargetsInitializing);
    }

    void OnDestroy()
    { 
        VuforiaApplication.Instance.OnVuforiaStarted -= OnVuforiaStarted;
    }


    void OnVuforiaStarted()
    {
        // We can only have one ModelTarget active at a time, so disable all MTBs at start.
        var behaviours = FindObjectsByType<ModelTargetBehaviour>(FindObjectsSortMode.None);
        foreach (var behaviour in behaviours)
            behaviour.enabled = false;

        if (!AutoActivate) 
            return;
        
        switch (TargetMode)
        {
            case ModelTargetMode.MODE_STANDARD:
                // Start with the Standard Model Target
                SelectDataSetStandard();
                break;
            case ModelTargetMode.MODE_ADVANCED:
                // Start with the Advanced Model Target
                SelectDataSetAdvanced();
                break;
        }
    }


    public void EnableSymbolicTargetsUI(bool enable)
    {
        AdvancedModelTargetsUI.enabled = enable;
    }

    public void AddAdvancedModelTarget(ModelTargetBehaviour behaviour)
    {
        if (behaviour != null && mAdvancedModelTargets != null)
            mAdvancedModelTargets.Add(behaviour);
		
        EnableSymbolicTargetsUI(mAdvancedModelTargets.Count == 0);
    }

    public void SelectModelTargetDataSetType(string modelTargetType)
    {
        switch (modelTargetType)
        {
            case "Standard":
                SelectDataSetStandard();
                break;
            case "Advanced":
                SelectDataSetAdvanced();
                break;
        }
    }

    public void SelectDataSetStandard()
    {
        foreach (var model in mAdvancedModelTargets)
            model.enabled = false;
        ModelStandard.enabled = true;
        
        TargetMode = ModelTargetMode.MODE_STANDARD;

        EnableSymbolicTargetsUI(false);
        ResetAugmentationTransform(ModelStandard.transform);
    }

    public void SelectDataSetAdvanced()
    {
        ModelStandard.enabled = false;
        foreach (var model in mAdvancedModelTargets)
            model.enabled = true;
        
        TargetMode = ModelTargetMode.MODE_ADVANCED;
        
        EnableSymbolicTargetsUI(true);
        ResetAugmentationTransform(ModelAdvanced.transform);
    }

    void ResetAugmentationTransform(Transform targetTransform)
    {
        Augmentation.transform.SetParent(targetTransform);
        Augmentation.transform.localPosition = Vector3.zero;
        Augmentation.transform.localRotation = Quaternion.identity;
        Augmentation.transform.localScale = Vector3.one;
    }
}