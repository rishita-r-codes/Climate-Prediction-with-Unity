using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using System;

public class coordLookup : MonoBehaviour
{
    [SerializeField] private Transform earthTransform; 
    [SerializeField] private TMP_InputField LatInput;
    [SerializeField] private TMP_InputField LongInput;
    [SerializeField] private float zoomDistance = 1.0f; // The distance from the point to zoom in on
    [SerializeField] private TextMeshProUGUI ErrorMessages;
    
    public float searchLatitude;
    public float searchLongitude;

    public void ZoomToLatLong()
    {
        // checks if null to output correct error message
        if (LatInput.text != null && LongInput.text != null )
        {
            try
            {
                // retrieves latitude and longitude from input fields
                searchLatitude = float.Parse(LatInput.text);
                searchLongitude = float.Parse(LongInput.text);
                if (searchLatitude >= -90 && searchLatitude <= 90 && searchLongitude >= -180 && searchLongitude <= 180)
                {
                    ConvertAndTransform (searchLatitude, searchLongitude);
                }
                else
                {
                    ErrorMessages.text = "Values are not within range \n Latitude must be -90 to 90 & Longitude -180 to 180";
                    throw new Exception ("Latitude and/or longitude is not valid");
                }
                
            }
            catch (FormatException)
            {
                // outputs if the input isn't a number
                ErrorMessages.text = "Incorrect format";
                throw new Exception("Incorrect Decimal format");
            }
        }
        else
        {
            // outputs error message if field is blank
            ErrorMessages.text = "You cannot leave any field blank";
            throw new Exception("Field blank");
        }    
    }

    public void ConvertAndTransform (float latitude, float longitude)
    {
        // converts latitude and longitude to radians
        float theta = latitude * Mathf.Deg2Rad;
        float phi = longitude * Mathf.Deg2Rad;

        // calculate the position camera needs to zoom in on with formulae
        float x = Mathf.Cos(theta) * Mathf.Cos(phi);
        float y = Mathf.Cos(theta) * Mathf.Sin(phi);
        float z = Mathf.Sin(theta);

        // create 3D coordinates to move camera to
        Vector3 zoomPosition = new Vector3(x, z, y) * zoomDistance;
        // sets the earths rotation to zero
        earthTransform.rotation = Quaternion.identity;
        // moves the camera to the zoom position
        transform.position = earthTransform.position + zoomPosition;
        // rotates the camera to look at the Earth's center
        transform.LookAt(earthTransform.position);

    }
}
