using UnityEngine;
using UnityEngine.UI;

public class ShowHideToggle : MonoBehaviour
{
    // The game objects that you want to show or hide
    public GameObject Decimal_System;
    public GameObject DMS_System;

    // The toggle group that contains the toggles
    public ToggleGroup toggleGroup;

    // The toggles that control the visibility of the game objects
    private Toggle DecToggle;
    private Toggle DmsToggle;

    void Start()
    {
        // Get the toggles from the toggle group
        Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
        DecToggle = toggles[0];
        DmsToggle = toggles[1];

        // Add listeners to the toggles
        DecToggle.onValueChanged.AddListener(delegate { OnToggle1Changed(); });
        DmsToggle.onValueChanged.AddListener(delegate { OnToggle2Changed(); });

        // Set the initial visibility of the game objects
        Decimal_System.SetActive(DecToggle.isOn);
        DMS_System.SetActive(DmsToggle.isOn);
    }

    void OnToggle1Changed()
    {
        // Set the visibility of object1 to match the toggle state
        Decimal_System.SetActive(DecToggle.isOn);
    }

    // This method is called when toggle2 changes its value
    void OnToggle2Changed()
    {
        // Set the visibility of object2 to match the toggle state
        DMS_System.SetActive(DmsToggle.isOn);
    }
}
