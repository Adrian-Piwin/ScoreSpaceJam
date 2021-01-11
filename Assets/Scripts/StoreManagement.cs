using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class StoreManagement : MonoBehaviour
{
    [SerializeField] private GameManagement gameManagement;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Player References")]
    [SerializeField] private SpriteRenderer player;
    [SerializeField] private SpriteRenderer playerDrag;

    [SerializeField] private List<Sprite> storeItems;
    [SerializeField] private List<TextMeshProUGUI> storeButtonsTxt;

    private List<bool> isBought;

    private int coins;
    private int currentSelection;

    void Start() {
        isBought = new List<bool> { false, false, false, false };
        PlayerPrefs.SetInt("Bought 0", 1);

        for (int i = 0; i < isBought.Count; i++) {
            isBought[i] = PlayerPrefs.GetInt("Bought " + i, 0) == 0 ? false : true;
        }

        for (int i = 0; i < isBought.Count; i++)
        {
            if (isBought[i])
                storeButtonsTxt[i].text = "Select";
        }

        UpdateSelected(currentSelection);
    }

    void OnEnable()
    {
        coins = gameManagement.coins;
        coinText.text = "COINS: " + coins;
    }

    public void SelectItem (string item) 
    {
        string[] itemInfo = item.Split(null);
        int itemNum = Convert.ToInt32(itemInfo[0]);
        int price = Convert.ToInt32(itemInfo[1]);

        // Buy item
        if (!isBought[itemNum] && coins >= price)
        {
            UpdateSelected(itemNum);

            PlayerPrefs.SetInt("Bought " + itemNum, 1);
            isBought[itemNum] = true;
            storeButtonsTxt[itemNum].text = "Select";
            coins -= price;
            coinText.text = "COINS: " + coins;
            ChangeSprite(itemNum);
        }
        else if (isBought[itemNum]) {
            UpdateSelected(itemNum);

            ChangeSprite(itemNum);
        }

    }

    private void ChangeSprite(int chosen) {
        player.GetComponent<SpriteRenderer>().sprite = storeItems[chosen];
        playerDrag.GetComponent<SpriteRenderer>().sprite = storeItems[chosen];
    }

    private void UpdateSelected(int newSelect) {
        foreach (TextMeshProUGUI btn in storeButtonsTxt) {
            btn.color = new Color32(0, 0, 0, 255);
        }

        storeButtonsTxt[newSelect].color = new Color32(70, 70, 70, 255);
        currentSelection = newSelect;
    }
}
