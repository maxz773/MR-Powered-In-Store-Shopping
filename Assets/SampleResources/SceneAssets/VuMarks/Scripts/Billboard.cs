/*===============================================================================
Copyright (c) 2024 PTC Inc. and/or Its Subsidiary Companies. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using UnityEngine;
using Vuforia;

public class Billboard : MonoBehaviour
{
    Transform mCamera;

    void Start()
    {
        mCamera = VuforiaBehaviour.Instance.transform;
    }

    void Update()
    {
        var direction = transform.position - mCamera.position;
        var targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1);
    }
}
