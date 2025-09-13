using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetingsMenu : MonoBehaviour
{
    public Transform settingsIcon;
    public TMP_Dropdown GraphicsDropdown;
    public TMP_Dropdown ResolutionDropdown;
    public Toggle toggle;
    public Image toggleOn;
    public Image toggleOff;

    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;
        ResolutionDropdown.ClearOptions();
        List<string> newOptions = new List<string>();

        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++ )
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            newOptions.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        ResolutionDropdown.AddOptions(newOptions);
        ResolutionDropdown.value = currentResIndex;
        ResolutionDropdown.RefreshShownValue();
    }
    public void SettingResolution(int resIndex)
    {
        Resolution setResolution = resolutions[resIndex];
        Screen.SetResolution(setResolution.width, setResolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        // sets the quality in project settings
        QualitySettings.SetQualityLevel(qualityIndex);
        switch (qualityIndex)
        {
            case 0: 
                GraphicsDropdown.image.color = new Color (0.8773585f, 0.2589695f, 0.2524475f, 1f);
                break;
            case 1:
                GraphicsDropdown.image.color = new Color (0.8773585f, 0.5677743f, 0.2524475f, 1f);
                break;
            case 2:
                GraphicsDropdown.image.color = new Color (0.8153692f, 0.8773585f, 0.2524475f, 1f);
                break;
            case 3:
                GraphicsDropdown.image.color = new Color (0.4454046f, 0.8773585f, 0.2524475f, 1f);
                break;
            case 4:
                GraphicsDropdown.image.color = new Color (0.505162f, 0.7594199f, 0.8301887f, 1f);
                break;
               
        }
    }

    void Update ()
    {
        // rotates the cog image in the bacground
        settingsIcon.transform.Rotate (0 , 0, 10 * Time.deltaTime);
    }

    public void SetFullscreen(bool fullScreenOn)
    {
        // changes true false value to float
        float value = (fullScreenOn) ? 1f:0f;
        // sets alpha value toggle graphic depending on value of toggle
        toggleOn.color = new Color (0, 0, 0, value);
        toggleOff.color = new Color (0, 0, 0, 1-value);
        // sets screen to fullscreen
        Screen.fullScreen = fullScreenOn;
    }

}
