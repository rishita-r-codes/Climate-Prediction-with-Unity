using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomEarth : MonoBehaviour
{
    [SerializeField] private float zoomLimit = 10f;
    [SerializeField] private float zoomSpeed = 5.0f;

    void Update()
    {
        // gets the current field of view from the camera object
        float fieldOfView = Camera.main.fieldOfView;
        // if the user presses ctrl+ and the field of view is larger than the zoom limit
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Equals) && fieldOfView > zoomLimit)
        {
            // decrease the field of view by the zoom speed every second
            fieldOfView -= zoomSpeed * Time.deltaTime;
        }

        // if the user presses ctrl- and the field of view is smaller than 180
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Minus) && fieldOfView < 180)
        {
            // increases the field of view by the zoom speed every second
            fieldOfView += zoomSpeed * Time.deltaTime;
        }
        // clamps the field of view between the zoom limit and 180
        fieldOfView = Mathf.Clamp(fieldOfView, zoomLimit, 180);
        // assign the new field of view to the camera
        Camera.main.fieldOfView = fieldOfView;
    }
}
