using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class hoverText : MonoBehaviour
{
    public GameObject textBox;
    public GameObject textBG;
    //called when OnMouseEver event is triggered
    public void ShowText()
    {
        textBox.SetActive(true);
        textBG.SetActive(true);
    }
    //called when OnMouseExit is triggered
    public void HideText()
    {
        textBox.SetActive(false);
        textBG.SetActive(false);
    }
}
