/*===============================================================================
Copyright (c) 2024 PTC Inc. and/or Its Subsidiary Companies. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using UnityEngine;
using UnityEngine.UI;

public class VuMarkObserverStatusUI : MonoBehaviour
{
    public Image Image;
    public GameObject Info;

    const string DEFAULT_INFO_TEXT = "<color=yellow>VuMark Instance Id:</color>\nNone\n\n<color=yellow>VuMark Type:</color>\nNone";

    public void Show(string vuMarkId, string vuMarkDataType, string vuMarkDesc, Sprite vuMarkImage)
    {
        var text = "<color=yellow>VuMark Instance Id: </color>\n" +
                      $"{vuMarkId} - {vuMarkDesc}\n\n" +
                      "<color=yellow>VuMark Type: </color>\n" +
                      $"{vuMarkDataType}";
        SampleUtil.AssignStringToTextComponent(Info, text);

        Image.sprite = vuMarkImage;
        Image.enabled = true;
    }

    public void ResetUI()
    {
        SampleUtil.AssignStringToTextComponent(Info, DEFAULT_INFO_TEXT);
        Image.enabled = false;
    }
}
