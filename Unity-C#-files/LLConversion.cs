// define namespaces required
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LLConversion : MonoBehaviour
{
    // declare all ui elements that need to be accessed by this code
    public SphereCollider MyCollider;
    public RenderTexture HeatMap;
    public GameObject Earth;
    public GameObject DotPrefab;
    public Texture2D Gradient;
    public Slider Timeline;
    public TextMeshProUGUI YearOutput;
    public TextMeshProUGUI LegendMin;
    public TextMeshProUGUI LegendMax;
    public ToggleGroup Parameters;
    
    // create a class which stores values for each data point
    public class ClimateDataPoint
    {
        public GameObject Dot { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Slope { get; set; }
        public float Intercept { get; set; }
    }

    // enum is used to represent a group of constants, in this case: climate variables
    public enum DataType 
    { 
        Temperature, 
        Precipitation, 
        Snowfall
    }

    // create a class for storing relevant data for each climate variable
    public class allParameterData
    {
        public DataType Parameters { get; set; }
        public List<ClimateDataPoint> VarDataPoints { get; set; }
        public string[][] FileContents { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
    
    // create list to hold three instances (temperature, precipitation and snowfall) of the class allParammeterData
    private List<allParameterData> allData = new List<allParameterData>();

    void Start()
    {
        // sets rotation to default (zero)
        Earth.transform.rotation = Quaternion.identity;
        // gets collider of earth to obtain radius
        MyCollider = GetComponent<SphereCollider>();
        // loops through each type in the enum DataType
        foreach (DataType parameter in Enum.GetValues(typeof(DataType)))
        {
            // create an instance of the class and assign each value from respective functions
            allParameterData data = new allParameterData();
            data.Parameters = parameter;
            // the file contents of the csv file (holding regression results) for each parameter is stored
            data.FileContents = LoadData(parameter);
            Vector2 minMax = GetMinMax(parameter);
            data.MinValue = (int)minMax.x;
            data.MaxValue = (int)minMax.y;
            data.VarDataPoints = CreateDots(data);
            
            // add instance to a list holding all data of all three parameters
            allData.Add(data);
        }
    }

    string[][] LoadData(DataType parameter)
    {
        string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string filePath = "";
        // returns corresponding file  path for each parameter
        switch (parameter)
        {
            case DataType.Temperature:
                filePath = Path.Combine(userProfilePath, @"unity programs\final_unity\Assets\Resources\tavg_regression_results.csv");
                break;
            case DataType.Precipitation:
                filePath = Path.Combine(userProfilePath, @"unity programs\final_unity\Assets\Resources\prcp_regression_results.csv");
                break;
            case DataType.Snowfall:
                filePath = Path.Combine(userProfilePath, @"unity programs\final_unity\Assets\Resources\snow_regression_results.csv");
                break;
        }

        using (var read = new StreamReader(filePath))
        {
            read.ReadLine();
            // holds each value separated by a comma in the array
            var lines = new List<string[]>();
            while (!read.EndOfStream)
            {
                var line = read.ReadLine();
                var values = line.Split(",");
                lines.Add(values);
            }
            return lines.ToArray();
        }
    }

    public List<ClimateDataPoint> CreateDots(allParameterData data)
    {
        List<ClimateDataPoint> dataPoints = new List<ClimateDataPoint>();
        foreach (string[] record in data.FileContents)
        {
            // obtains necessary values from each record of the csv file
            float latitude = float.Parse(record[3]);
            float longitude = float.Parse(record[4]);
            float slope = float.Parse(record[1]);
            float intercept = float.Parse(record[2]);

            float pastVal = 1970 * slope + intercept;
            float futureVal = 2050 * slope + intercept;

            // doesn't instantiate points where the regressed value exceeds the limits
            if (pastVal < data.MinValue || pastVal > data.MaxValue)
            {
                continue;
            }
            if (futureVal < data.MinValue || futureVal > data.MaxValue)
            {
                continue;
            }

            // convert latitude and longitude to 3D spherical coordinates
            Vector3 position = ConvertTo_CartesianCoords(latitude, longitude);
            // creates new dot gameobject based on the prefab at (0, 0, 0) coordinates
            GameObject dot = Instantiate(DotPrefab, Vector3.zero, Quaternion.identity);
            // moves dot to 3D spherical coordinates calculated from latitude and longtitude
            dot.transform.position = position;
            // sets the dot as a child of the Earth sphere gameobject so dots stay fixed on earth regardless of transformation
            dot.transform.SetParent(Earth.transform, true);

            dataPoints.Add(new ClimateDataPoint {Dot = dot, Latitude = latitude, Longitude = longitude, Slope = slope, Intercept = intercept});
        }
        return dataPoints;
    }

    public Vector3 ConvertTo_CartesianCoords(float latitude, float longitude)
    {
        // set radius of sphere the dots are plotted on
        float radius = 0.51f;
        // comvert latitude and longitude degrees to radians
        float phi = longitude * Mathf.PI / 180;
        float theta = latitude * Mathf.PI / 180;
        
        // use formulae with polar angle (theta) and azimuthal angle (phi)
        float Xcoord = radius * Mathf.Cos(theta) * Mathf.Cos(phi);
        float Ycoord = radius * Mathf.Cos(theta) * Mathf.Sin(phi);
        float Zcoord = radius * Mathf.Sin(theta);

        // create 3d coordinates
        Vector3 coordinate = new Vector3 (Xcoord, Zcoord, Ycoord);
        // preserve direction of vector but change magnitude to the radius so all points lie at the same altitude on the earth
        coordinate = coordinate.normalized * radius;
        return coordinate;
    }

    Vector2 GetMinMax(DataType parameter)
    {
        Vector2 minMax = new Vector2();
        // return minimum and maximum values corresponding to the parameter
        switch (parameter)
        {
            case DataType.Temperature:
                minMax.x = 0;
                minMax.y = 30;
                break;
            case DataType.Precipitation:
                minMax.x = 0;
                minMax.y = 20000;
                break;
            case DataType.Snowfall:
                minMax.x = 0;
                minMax.y = 10000;
                break;
        }
        return minMax;
    }

    void Update ()
    {
        //gets selected toggle within toggle group
        Toggle selectedToggle = Parameters.ActiveToggles().FirstOrDefault();

        // if no toggles are selected, set all dots to be invisible
        if (selectedToggle == null)
        {
            foreach (var data in allData)
            {
                foreach (var point in data.VarDataPoints)
                {
                    point.Dot.SetActive(false);
                }
            }
            return;
        }
        
        DataType selectedDataType;
        string units;
        switch (selectedToggle.gameObject.name)
        {
            case "TempToggle":
                selectedDataType = DataType.Temperature;
                units = " °C";
                break;
            case "PrcpToggle":
                selectedDataType = DataType.Precipitation;
                units = " mm";
                break;
            case "SnowToggle":
                selectedDataType = DataType.Snowfall;
                units = " mm";
                break;
            default:
                throw new Exception("Unknown Toggle Selected");
                // catches exception with inconsistencies with naming or other errors 
        }

        foreach (allParameterData parameter in allData)
        {
            if (selectedDataType == parameter.Parameters)
            {
                // displays the maximum and minimum values on the legend
                LegendMin.text = (parameter.MinValue).ToString() + units;
                LegendMax.text = (parameter.MaxValue).ToString() + units;
            }
        }

        // gets the year from the timeline input
        int year = (int)Timeline.value;
        // width and height of render texture
        int width = HeatMap.width;
        int height = HeatMap.height;
        Texture2D HeatMapTex = new Texture2D(width, height);

        foreach (var data in allData)
        {
            foreach (var point in data.VarDataPoints)
            {
                float value = (point.Slope * year) + point.Intercept;
                int normalisedValue = (int)((value/data.MaxValue) * Gradient.width);

                Color pixColour = Gradient.GetPixel(normalisedValue, 5);
                GameObject currentDot = point.Dot;
                currentDot.GetComponent<Renderer>().material.color = pixColour;

                if (data.Parameters == selectedDataType)
                {
                    // only makes data dots of the paramter selected visible
                    currentDot.SetActive(true);
                }
                else
                {
                    // makes data dots of other paramters not visible
                    currentDot.SetActive(false);
                }

                //converts latitude and longtiude to 2d xy coordinates
                int x = (int)((point.Longitude + 180) / 360) * width;
                int y = (int)((point.Latitude + 90) / 180) * height;
                Vector2 pixelCoordinates = new Vector2 (x, y);
                
                // plots points on texture 
                HeatMapTex.SetPixel((int)(pixelCoordinates.x), (int)(pixelCoordinates.y), pixColour);
            }
        }
        HeatMapTex.Apply();
        Graphics.Blit(HeatMapTex, HeatMap);
        // updates the text to show which year is selected on the timeline
        YearOutput.text = $"{year}";
    }
}
