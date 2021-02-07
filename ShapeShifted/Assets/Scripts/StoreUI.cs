using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StoreUI : MonoBehaviour
{
    [SerializeField] StoreManagement storeManagement;

    private void OnEnable() 
    {
        storeManagement.MenuEnabled();
    }
}
