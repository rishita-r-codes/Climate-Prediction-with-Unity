using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingskybox : MonoBehaviour
{
    public float rotateSpeed = 1.3f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
    }
}
