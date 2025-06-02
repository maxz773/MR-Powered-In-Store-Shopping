/*===============================================================================
Copyright (c) 2024 PTC Inc. and/or Its Subsidiary Companies. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
#if UNITY_XR_OPENXR
using UnityEditor.XR.Management;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
#endif
#if MAGIC_LEAP_UNITYSDK
using MagicLeap.OpenXR.InteractionProfiles;
#endif

public class XRSettingsValidator: IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    const string HOLOLENS_FEATURE_SET_ID = "com.microsoft.openxr.featureset.hololens";
    const string HAND_TRACKING_FEATURE_ID = "com.microsoft.openxr.feature.handtracking";
    const string MAGIC_LEAP_FEATURE_SET_ID = "com.magicleap.openxr.featuregroup";
    const string MAGIC_LEAP_SUPPORT_FEATURE_ID = "com.magicleap.openxr.feature.ml2";
    
    public void OnPreprocessBuild(BuildReport report)
    {
        var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
#if !UNITY_XR_OPENXR
        Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the Open XR Plugin to be installed"));
#else
        var xrGeneralSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
        if (!xrGeneralSettings.InitManagerOnStart)
            Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the XR Loader to initialize on Startup."));

        var xrLoaders = xrGeneralSettings.AssignedSettings.activeLoaders;
        if (!xrLoaders.Any(l => l is OpenXRLoader))
            Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the Open XR Loader to be enabled in the XR Plug-in Management Settings."));

#if MICROSOFT_MIXED_REALITY_OPENXR && UNITY_WSA
        var featureSets = OpenXRFeatureSetManager.FeatureSetsForBuildTarget(BuildTargetGroup.WSA);
        var hlFeatureSet = featureSets.First(fs => fs.featureSetId.Equals(HOLOLENS_FEATURE_SET_ID));
        if (!hlFeatureSet.isEnabled)
            Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the Microsoft HoloLens Feature Group to be enabled."));

        var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.WSA);
        if (openXRSettings.depthSubmissionMode != OpenXRSettings.DepthSubmissionMode.Depth16Bit)
            Debug.LogWarning("Depth Submission Mode for HoloLens should be set to 16-Bit.");

        if (openXRSettings.renderMode != OpenXRSettings.RenderMode.SinglePassInstanced)
            Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the Open XR Render Mode on UWP to be set to Single Pass Instanced."));

        var interactionProfile = openXRSettings.GetFeature<MicrosoftHandInteraction>();
        if (interactionProfile == null || !interactionProfile.enabled)
            Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the Microsoft Hand Interaction Profile to be enabled."));

        var handTracking = FeatureHelpers.GetFeatureWithIdForBuildTarget(BuildTargetGroup.WSA, HAND_TRACKING_FEATURE_ID);
        if (!handTracking.enabled)
            Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the Hand Tracking feature to be enabled."));
#elif MAGIC_LEAP_UNITYSDK && UNITY_ANDROID
        var featureSets = OpenXRFeatureSetManager.FeatureSetsForBuildTarget(BuildTargetGroup.Android);
        var mlFeatureSet = featureSets.First(fs => fs.featureSetId.Equals(MAGIC_LEAP_FEATURE_SET_ID));
        if (!mlFeatureSet.isEnabled)
            Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the Magic Leap Feature Group to be enabled."));

        var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
        if (openXRSettings.depthSubmissionMode != OpenXRSettings.DepthSubmissionMode.None)
            Debug.LogWarning("Depth Submission Mode for Magic Leap should be set to None.");

        if (openXRSettings.renderMode != OpenXRSettings.RenderMode.SinglePassInstanced)
            Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the Open XR Render Mode on Magic Leap to be set to Single Pass Instanced \\ Multi-View."));

        var interactionProfile = openXRSettings.GetFeature<MagicLeapControllerProfile>();
        if (interactionProfile == null || !interactionProfile.enabled)
            Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the Magic Leap 2 Controller Interaction Profile to be enabled."));

        var handTracking = FeatureHelpers.GetFeatureWithIdForBuildTarget(BuildTargetGroup.Android, MAGIC_LEAP_SUPPORT_FEATURE_ID);
        if (!handTracking.enabled)
            Debug.LogException(new BuildFailedException("Vuforia Digital Eyewear Sample requires the Magic Leap 2 Support feature to be enabled."));
#endif
#endif
    }
}