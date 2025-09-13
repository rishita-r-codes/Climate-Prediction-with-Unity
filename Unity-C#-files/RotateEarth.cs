using UnityEngine;

public class RotateEarth : MonoBehaviour
{
    // Sets default rotation speed
    public float rotateSpeed = 50f;

    void Update()
    {
        // Gets keyboard input of left and right arrows
        float X_Axis = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
        // Gets keyboard input of up and down arrows
        float Y_Axis = Input.GetAxis("Vertical") * rotateSpeed * Time.deltaTime;

        // Rotate around the local Y-axis (left and right)
        transform.Rotate(0, X_Axis, 0);

        // Rotate around the local X-axis (up and down)
        transform.Rotate(Y_Axis, 0, 0);
    }
}
