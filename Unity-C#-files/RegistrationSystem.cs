using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;   
using UnityEngine.SceneManagement;
using TMPro;

public class RegistrationSystem : MonoBehaviour
{
    // declare the input fields to retrive user inputs
    public TMP_InputField employeeID;
    public TMP_InputField firstName;
    public TMP_InputField lastName;
    public TMP_InputField password;
    public TMP_InputField passwordRepeat;
    public Button RegisterButton;
    public TextMeshProUGUI RegisterMsgs;

    public GameObject Username_PopUp;
    public TextMeshProUGUI UsernameMsg;

    void Start ()
    {
        // the code within the lambda function will be execute when the button is clicked
        RegisterButton.onClick.AddListener(() => 
        { 
            // only executes code if the password is considered valid
            if (PasswordValid(password.text, passwordRepeat.text))
            {
                // converts user input (string) to an integer to verify against database
                int employeeID_int = Convert.ToInt32(employeeID.text);
                // create unique username using first name, surname and employee ID
                string username = (firstName.text.ToLower()).Substring(0, 1) + lastName.text.ToLower() + employeeID.text;
                // output the created username as text on the screen
                RegisterMsgs.text = username;
                // coroutine calls the RegisterUser function from the WebRequest class of the Main instance
                StartCoroutine(Main.Instance.WebRequest.RegisterUser(employeeID_int, firstName.text, lastName.text, username, password.text, success =>
                {
                    if (success)
                    {
                        // pop-up screen becomes visible and displays username and message 
                        Username_PopUp.SetActive(true);
                        UsernameMsg.text = ($"Your generated username is: {username} \n Please use this and your password to login...");
                    }
                }));
            }
        });
    }

    public bool PasswordValid(string passwordInput, string repeatedPass)
    {
        bool valid = false;
        if (passwordInput == repeatedPass)
        {
            //regular expression only valdates strings with length of 8-25 upper and lowercase, a digit and a special character
            Regex passwordStrong = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,25}$");
            // IsMatch method checks if password follows regular expression rules
            bool strong = passwordStrong.IsMatch(passwordInput);
            if (!strong)
            {
                RegisterMsgs.text = "Password must meet minimum requirements";
                //  upper and lowercase, number & special character. \n Must also have length from 8-25 characters.
            }
            else
            {
                valid = true;
            }
        }
        else
        {
            RegisterMsgs.text = ("Passwords do not match");
        }
        return valid;
    }
}
