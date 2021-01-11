using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject settingsUI;

    [Header("Help")]
    [SerializeField] private GameObject helpUI;


    public void ToggleSettings()
    {
        settingsUI.SetActive(!settingsUI.active);
    }

    public void ToggleHelp()
    {
        helpUI.SetActive(!helpUI.active);
    }
}
