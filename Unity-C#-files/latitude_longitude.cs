using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class latitude_longitude : MonoBehaviour
{
    public GameObject timeLine;
    public RenderTexture renderTexture;
    public int date;

    // Start is called before the first frame update
    void Start()
    {
        string filePath = @"C:\Users\Rishi\unity programs\final unity\Assets\Resources\tavg_regression_results.csv";
        string[][] fileContents;

        using (var read = new StreamReader(filePath))
        {
            // skips the first line in the csv file
            read.ReadLine();
            var lines = new List<string[]>();
            
            while (!read.EndOfStream)
            {
                var line = read.ReadLine();
                var values = line.Split(",");
                lines.Add(values);
                // adds values to list
            }
            fileContents = lines.ToArray();
            // adds lists to 2d array
            
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);


            foreach (string[] record in fileContents)
            {
                //converts data type from string to float
                double latitude = double.Parse(record[3]);
                double longitude = double.Parse(record[4]);
                float slope = float.Parse(record[1]);
                float intercept = float.Parse(record[2]);

                // Calculates temperature 
                float temperature = (date * slope) + intercept;

                // Converts degrees to pixels on Render Texture object
                //UnwrappedTileId tileCoordinates = Conversions.LatitudeLongitudeToTileId(latitude, longitude, zoomLevel);
                //Vector2d pixelCoordinates = Conversions.TileIdToPixel(tileCoords, zoomLevel, tileSize);

                //float x = (float)(pixelCoordinates.x - renderTexture.width / 2 + renderTexture.texelSize.x);
                //float y = (float)(pixelCoordinates.y - renderTexture.height / 2 + renderTexture.texelSize.y);

                // Set the pixel color on the render texture
                //texture.SetPixel((int)x, (int)y, new Color(1, 1, 1, temperature/255));

                //Vector2 mercatorCoords = LatLongToPixels(latitude, longitude, renderTexture.width, renderTexture.height);
                //texture.SetPixel((int)mercatorCoords.x, renderTexture.height - (int)mercatorCoords.y, new Color(1, 1, 1, temperature/100));
            }

            texture.Apply();
            Graphics.Blit(texture, renderTexture);
            // copies texture2d to render texture
            Debug.Log("Copied to render texture successfully");

        }
    } 

    //public Vector2 LatLongToPixels(double latitude, double longitude, int width, int height)
    //{
        //const int earthRadius = 6378137;
        //int zoomLevel = 0;
        //float initialResolution = 2 * Math.PI * earthRadius / tileSize;
        //float pixelsPerMeter = width / (2 * earthRadius * Math.PI);
        //float originShift = 2 * Math.PI * earthRadius/ 2.0;

        // conversion to radians
        //longitude = (Math.PI / 180) * longitude;
        //latitude = (Math.PI / 180) * latitude;

        // convert to spherical mercators in xy metres
        //double x = earthRadius * longitude;
        //double y = earthRadius * (Math.Log(Math.Tan((Math.PI/4 + latitude/2))));

        // +180 shifts range from -180<x<180 to 0<x<360

        //Vector2 mercators = new Vector2 ();
        //mercators.x = longitude * originShift/180;
        //mercators.y = Math.Log(Math.Tan((90 + latitude) * Math.PI / 360)) / (Math.PI / 180);

        //Vector2 pixels = new Vector2 ();
        //pixels.x = (mercators.x + originShift) * pixelsPerMeter * Math.Pow(2.0, zoomLevel);
        //pixels.y = (mercarors.y);

        //float x = (float)(longitude + 180) * (width / 360f);
        //float y = (float)(((height / 2f) - (width * Mathf.Log(Mathf.Tan((Mathf.PI / 4f) + (((float)latitude * Mathf.PI / 180f) / 2f))))) / 2f);
        //Debug.Log($"x = {x}, y = {y}");
 
        //return new Vector2(x, y);

    //} 
}