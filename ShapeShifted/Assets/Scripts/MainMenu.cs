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

    [Header("Store")]
    [SerializeField] private GameObject storeUI;

    public void ToggleSettings()
    {
        if (!helpUI.active && !storeUI.active)
            settingsUI.SetActive(!settingsUI.active);
    }

    public void ToggleHelp()
    {
        if (!settingsUI.active && !storeUI.active)
            helpUI.SetActive(!helpUI.active);
    }

    public void ToggleStore()
    {
        if (!helpUI.active && !settingsUI.active)
            storeUI.SetActive(!storeUI.active);
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
