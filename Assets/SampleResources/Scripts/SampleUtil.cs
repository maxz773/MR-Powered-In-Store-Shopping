/*===============================================================================
Copyright (c) 2024 PTC Inc. and/or Its Subsidiary Companies. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

#if TEXT_MESH_PRO
using TMPro;
#endif
using UnityEngine;

public static class SampleUtil
{
    public static void AssignStringToTextComponent(GameObject textObj, string text)
    {
        if (!textObj)
        {
            Debug.LogWarning("Destination Text GameObject is Null.");
            return;
        }

#if TEXT_MESH_PRO
        var textMesh = textObj.GetComponentInChildren<TMP_Text>();
        if (textMesh)
        {
            textMesh.text = text;
            return;
        }
#endif

        var canvasText = textObj.GetComponent<UnityEngine.UI.Text>();
        if (!canvasText)
            return;
        canvasText.text = text;
    }
}
