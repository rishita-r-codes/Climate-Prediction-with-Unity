using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // namespace for classes that are required for network communcation in Unity
using UnityEngine.UI;
using TMPro;

public class WebRequest : MonoBehaviour
{
    // declare text fields so messages can be outputted
    public TextMeshProUGUI LoginMsgs;
    public TextMeshProUGUI RegisterMsgs;

    // declares a coroutine named 'LoginFunction' that takes two string parameters: 'username' and 'password'
    public IEnumerator LoginFunction(string username, string password, Action<bool> callback)
    {
        // create form object for sending the username and password to the server
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPassword", password);
        
        // sends the form data created to the mysqli server via HTTP POST
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/back_to_the_climate/Login.php", form);
        // sends the web request and waits
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            // logs error message to the console log
            Debug.Log(www.error);
            LoginMsgs.text = "Error occured...Try again";
            callback(false);
        }
        else if (www.downloadHandler.text != "Login success")
        {
            LoginMsgs.text = (www.downloadHandler.text);
            callback(false);
        }
        else // if web request was successful
        {
            // sets the LoginMsgs text to the response message from the server
            LoginMsgs.text = (www.downloadHandler.text);
            callback(true);
        }
    }

    public IEnumerator RegisterUser(int employeeID, string firstName, string lastName, string username, string password, Action<bool> callback)
    {
        // creates a form object for sending the employeeID, first name, last name, username and password to the server
        WWWForm form = new WWWForm();
        form.AddField("loginID", employeeID);
        form.AddField("firstName", firstName.ToLower());
        form.AddField("lastName", lastName.ToLower());
        form.AddField("loginUser", username);
        form.AddField("loginPassword", password);

        // sends registration details to the server
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/back_to_the_climate/RegisterUser.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            // logs error message to the console
            Debug.Log(www.error);
            RegisterMsgs.text = "Error occured...Try again";
            // sets boolean 'success' to false for coroutine in RegistrationSystem.cs
            callback(false);
        }
        else if (www.downloadHandler.text != "New User created successfully!")
        {
            RegisterMsgs.text = www.downloadHandler.text;
            callback(false);
        }
        else
        {
            // sets out message text of RegisterMsgs to the response of server
            RegisterMsgs.text = www.downloadHandler.text;
            // sets boolean 'success' to false for coroutine in RegistrationSystem.cs
            callback(true);
        }
    }
}
