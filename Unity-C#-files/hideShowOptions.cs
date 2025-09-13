using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideShowOptions : MonoBehaviour
{
    public CanvasGroup ToHide;

    public void HideUI()
    {
        // Set the alpha value to 0 to make the UI invisible
        ToHide.alpha = 0f;
        ToHide.blocksRaycasts = false;
        ToHide.interactable = false;
    }

    public void ShowUI()
    {
        // Set the alpha value to 1 to make the UI visible
        ToHide.alpha = 1f;
        ToHide.blocksRaycasts = true;
        ToHide.interactable = true;
    }
}
