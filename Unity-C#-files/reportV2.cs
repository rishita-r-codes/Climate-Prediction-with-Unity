using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.IO;


public class reportV2 : MonoBehaviour
{
    [SerializeField] private TMP_InputField YearInput;
    [SerializeField] private TMP_InputField DecLat;
    [SerializeField] private TMP_InputField DecLon;
    [SerializeField] private TMP_InputField DegLat;
    [SerializeField] private TMP_InputField MinLat;
    [SerializeField] private TMP_InputField SecLat;
    [SerializeField] private TMP_InputField DegLon;
    [SerializeField] private TMP_InputField MinLon;
    [SerializeField] private TMP_InputField SecLon;
    [SerializeField] private TextMeshProUGUI OutputMsgDec;
    [SerializeField] private TextMeshProUGUI OutputMsgDms;    
    [SerializeField] private GameObject DecSystem;

    // create class to hold atributes of each plot point
    class Plot
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double VariableData { get; set; }

        public Plot (double latitude, double longitude, double varData)
        {
            Latitude = latitude;
            Longitude = longitude;
            VariableData = varData;
        }
    }

    // create a new list with the type Plot to hold the plot points for each parameter
    List<Plot> temperatureData = new List<Plot>();
    List<Plot> precipitationData = new List<Plot>();
    List<Plot> snowfallData = new List<Plot>();

    public int year;
    public double inputLat;
    public double inputLon;

    public void Main()
    {
        // checks if the decimal system was selected or the DMS system
        if (DecSystem.activeInHierarchy)
        {
            try
            {
                inputLat = double.Parse(DecLat.text);
                inputLon = double.Parse(DecLon.text);
            }
            catch (FormatException)
            {
                OutputMsgDec.text = "Fields are in incorrect format or blank";
                throw new Exception ("Incorrect format or blank");
            } 
            if (inputLat < -90 | inputLat > 90 || inputLon < -180 || inputLon > 180)
            {
                OutputMsgDec.text = "Latitude and/or longitude out of range";
                throw new Exception ("Latitude and longitude out of range");
            }
        }
        else
        {
            try
            {
                // converts the input values from DMS (degrees, minutes, seconds) to decimal
                inputLat = DMS2Dec(int.Parse(DegLat.text), int.Parse(MinLat.text), float.Parse(SecLat.text));
                inputLon = DMS2Dec(int.Parse(DegLon.text), int.Parse(MinLon.text), float.Parse(SecLon.text));
            }
            catch (FormatException)
            {
                OutputMsgDms.text = "Fields are in incorrect format or blank";
                throw new Exception ("Incorrect format or blank");
            } 
            if (inputLat < -90 | inputLat > 90 || inputLon < -180 || inputLon > 180)
            {
                OutputMsgDms.text = "Values out of range";
                throw new Exception ("Latitude and longitude out of range");
            }
        }

        
        // retrieves year inputted from input field and parses to integer
        year = int.Parse(YearInput.text);

        // loads data from csv and formats it into the list created for each parameter
        temperatureData = LoadData(@"unity programs\final_unity\Assets\Resources\tavg_regression_results.csv");
        precipitationData = LoadData(@"unity programs\final_unity\Assets\Resources\prcp_regression_results.csv");
        snowfallData = LoadData(@"unity programs\final_unity\Assets\Resources\snow_regression_results.csv");

        // creates plot point for inputted latitude and longitude values
        Plot unknown = new Plot (inputLat, inputLon, 0.0);

        // power used when calculating weight
        double pValue = 2.0;
        // retrieves interpolated temp, prcp and snowfall values from Interpolate function
        double interTemp = Interpolate(temperatureData, unknown, pValue);
        double interPrcp = Interpolate(precipitationData, unknown, pValue);
        double interSnow = Interpolate(snowfallData, unknown, pValue);

        // use a ternary function to set precipitation to 0 if negative
        interPrcp = (interPrcp < 0) ? 0 : interPrcp;
        // use ternary to set snowfall to 0 if value is negative
        interSnow = (interSnow < 0) ? 0 : interSnow;

        ExportTXT(interTemp, interPrcp, interSnow);
    }
    
    // subroutine to convert from DMS format to decimals
    double DMS2Dec(int degrees, int minutes, float seconds)
    {
        double decimalValue = degrees + (minutes/60) + (seconds/3600);
        return decimalValue;
    } 
    

    List<Plot> LoadData(string path)
    {
        string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        // combines the user profile path and path inputted
        string filePath = Path.Combine(userProfilePath, path);
        var lines = new List<string[]>();

        using (var read = new StreamReader(filePath))
        {
            read.ReadLine();
            // holds each value separated by a comma in the array
            while (!read.EndOfStream)
            {
                var line = read.ReadLine();
                var values = line.Split(",");
                lines.Add(values);
            }
            // converts list to array
            lines.ToArray();
        }

        List<Plot> contentList = new List<Plot>();
        foreach (string[] record in lines)
        {
            float slope = float.Parse(record[1]);
            float intercept = float.Parse(record[2]);
            float latitude = float.Parse(record[3]);
            float longitude = float.Parse(record[4]);

            // creates new plot point for each record in the 2D array
            Plot newPoint = new Plot(latitude, longitude, ((slope*year)+intercept));
            // adds each plot point to the list
            contentList.Add(newPoint);
        }

        return contentList;
    }

    double Interpolate(List<Plot> points, Plot unknownPoint, double pValue)
    {
        double weightSum = 0;
        double weightedVarSum = 0;

        // loops through each plot point in the list of points
        foreach (Plot point in points)
        {
            // retrieve value of weight for each point by calling CalcWeight function
            double weight = CalcWeight(point, unknownPoint, pValue);
            weightSum += weight;
            // finds sum of weighted temperatures or precipitation values or snowfall levels
            weightedVarSum += weight * point.VariableData;
        }

        // prevents dividing by zero and error
        if (weightSum == 0)
        {
            return double.NaN;
        }

        // inverse distance weighting interpolation formula
        double interpolatedValue = weightedVarSum / weightSum; 
        return interpolatedValue;
    }

    // function to calculate weights
    double CalcWeight (Plot point1, Plot point2, double pValue)
    {
        double distance = CalcDistance(point1, point2);

        if (distance == 0)
        {
            return double.MaxValue;
        }

        double weight = 1.0 / Math.Pow (distance, pValue);
        return weight;
    }

    // calculating distance between two points on a sphere in metres (latitude and longitude)
    double CalcDistance (Plot p1, Plot p2)
    {
        // radius of the earth in metres
        const int radius = 6378137;

        // converts values from degrees to radians
        double lati1 = p1.Latitude * Mathf.Deg2Rad;
        double long1 = p1.Longitude * Mathf.Deg2Rad;
        double lati2 = p2.Latitude * Mathf.Deg2Rad;
        double long2 = p2.Longitude * Mathf.Deg2Rad;

        // find difference between latitudes and longitudes
        double latDiff = lati2 - lati1;
        double lonDiff = long2 - long1;

        // Using Haversine formulae to calculate distance between two points on a sphere
        double a = Math.Pow(Math.Sin(latDiff / 2.0), 2) + Math.Cos(lati1) * Math.Cos(lati2) * Math.Sin(lonDiff / 2.0) * Math.Sin(lonDiff / 2.0);
        double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

        // IDW formula
        double distance = radius * c;
        return distance;
    }

    void ExportTXT (double temp, double prcp, double snow)
    {
        // gets current time and date
        DateTime currentDateTime = DateTime.Now;
        // finds userprofile path and downloads folder in path
        string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";    
        // formats the date and time    
        string now = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss");
  
        // create file name based on latitude and longitude and year inputted
        string fileName = $"{Math.Round(inputLat, 2)}N {Math.Round(inputLon, 2)}W ClimateReport{year}.txt";
        // obtain final file path by combining download folder path and the new file name
        string filePath = Path.Combine(downloadsFolder, fileName);
        // create an instance of the StreamWriter class
        var writer = new StreamWriter(filePath, true);

        writer.WriteLine($"Report Created : [{now}]");
        writer.WriteLine($"\n  Year: {year}");
        writer.WriteLine($"  Coordinates: {inputLat}, {inputLon}");
        writer.WriteLine($"  Annual Mean Temperature: {temp} °C");
        writer.WriteLine($"  Annual Precipitation: {prcp} mm");
        writer.WriteLine($"  Annual Snowfall: {snow} mm");

        writer.Close();

        if (DecSystem.activeInHierarchy)
        {
            OutputMsgDec.text = "File successfully generated in downloads folder";
        }
        else
        {
            OutputMsgDms.text = "File successfully generated in downloads folder";
        }
        Debug.Log("Exported Text file!");
    }
}
