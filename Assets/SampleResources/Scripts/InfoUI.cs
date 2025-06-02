/*===============================================================================
Copyright (c) 2024 PTC Inc. and/or Its Subsidiary Companies. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using System.Collections;
#if MIXED_REALITY_TOOLKIT_CORE && UNITY_XR_INTERACTION_TOOLKIT
using MixedReality.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
#endif
using UnityEngine;

public class InfoUI : MonoBehaviour
{
#if MIXED_REALITY_TOOLKIT_CORE && UNITY_XR_INTERACTION_TOOLKIT
    public Camera Camera;
    public LazyFollow FollowComponent;
    public Collider UICollider;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveToCameraView());
    }

    IEnumerator MoveToCameraView()
    {
        // Wait one frame for the scene to be fully loaded
        yield return null;

        while(Camera == null || !Camera.IsInFOVCached(UICollider))
        {
            // Wait until the InfoUI Canvas is in the Camera's field of view
            yield return null;
        }

        FollowComponent.enabled = false;
    }
#endif
}
