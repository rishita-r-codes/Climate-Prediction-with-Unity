using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // create an instance with the class Main
    public static Main Instance;
    public WebRequest WebRequest;
    
    void Start()
    {
        Instance = this;
        WebRequest = GetComponent<WebRequest>();
    }
}
