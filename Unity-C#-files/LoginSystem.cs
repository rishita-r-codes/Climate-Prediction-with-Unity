using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginSystem : MonoBehaviour
{
    // declares the input fields to retrieve the user inputs
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    public Button LoginButton;

    private SwitchScenes switchScenes;

    void Start ()
    {
        // gets the script attached to the Login button game object
        switchScenes = LoginButton.GetComponent<SwitchScenes>();
        // only runs code within lambda function once the button is pressed
        LoginButton.onClick.AddListener(() => 
        { 
            // executes subroutine LoginFunction within WebRequest script with the parameters of the inputted username and password
            StartCoroutine(Main.Instance.WebRequest.LoginFunction(UsernameInput.text, PasswordInput.text, success => 
            {
                if (success)
                {
                    // if successful the login button will proceed to the main screen
                    switchScenes.nextScene = "main v3";
                    // calls function from another script
                    switchScenes.SwitchScene();
                }
            }));
        });
    }
}
