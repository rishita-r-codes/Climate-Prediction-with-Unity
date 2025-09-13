using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DMStoLL : MonoBehaviour
{
    public GameObject Camera;
    public TMP_InputField Lat_Degrees;
    public TMP_InputField Lat_Minutes;
    public TMP_InputField Lat_Seconds; 

    public TMP_InputField Lon_Degrees;
    public TMP_InputField Lon_Minutes;
    public TMP_InputField Lon_Seconds; 

    [SerializeField] private TextMeshProUGUI ErrorMessage;

    private coordLookup coordLookup;

    public void Convert_DMStoLL ()
    {
        coordLookup = Camera.GetComponent<coordLookup>();

        if (Lat_Degrees!=null && Lat_Minutes!=null && Lat_Degrees!= null && Lon_Degrees!=null && Lon_Minutes!= null && Lon_Seconds!= null)
        {
            // will try block of code and catches any format exception to output to screen
            try
            {
                // parses value from input field to integer and float data types
                int degreesLat = Int32.Parse(Lat_Degrees.text);
                int minsLat = Int32.Parse(Lat_Minutes.text);
                float secsLat = float.Parse (Lat_Seconds.text);

                int degreesLon = Int32.Parse(Lon_Degrees.text);
                int minsLon = Int32.Parse(Lon_Minutes.text);
                float secsLon = float.Parse (Lon_Seconds.text);

                // calls the subroutine to calculate the decimal value for latitude and longtiude
                float latitude = DMS2Dec (degreesLat, minsLat, secsLat);
                float longitude = DMS2Dec (degreesLon, minsLon, secsLon);

                // calls the subroutine from coordLookup to transform camera
                coordLookup.ConvertAndTransform(latitude, longitude);
            }
            catch (FormatException)
            {
                // if format exception is thrown because values cannot be converted to Int32 or Float, error message is shown
                ErrorMessage.text = "Degrees and Minutes must be integers";
                throw new Exception("DMS coordinates are in the wrong format");
            } 
        }
        else
        {
            ErrorMessage.text = "A field is blank";
        }
    }

    private float DMS2Dec (int degrees, int minutes, float seconds)
    {
        // uses DMS to decimal formula to calculate the latitude and longtiude decimal values
        float decimalValue = degrees + (minutes/60) + (seconds/3600);
        return decimalValue;
    }
}
