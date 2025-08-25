using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LevelItemController : MonoBehaviour
{
    public TMP_Text levelNameText;
    public GameObject characterImage;
    public GameObject InactiveBackground;
    public GameObject lockIcon;
    public GameObject iconCoin;
    public GameObject playButton;
    public GameObject unlockButton;
    public TMP_Text coinAmountText;

    [Header("Debug Setup")]
    public bool isUnlocked = false;
    public string levelName = "Level 1";
    public int levelNumber = 1;
    public int coinCost = 100;
    
    // Level data controller untuk cek status terkunci atau tidak
    public static LevelDataController levelData; // akses statis

    public void Awake()
    {
      
        levelData = LevelDataController.I;
    }

    void Start()
    {
        UpdateLevelView();
        CheckAndSetLevelLockedState();
    }

    // Panggil fungsi ini ketika ada perubahan pada inspector
    void OnValidate()
    {
        UpdateLevelView();
    }

    public void UpdateLevelView()
    {
        levelNameText.text = levelName;
        coinAmountText.text = coinCost.ToString();
        if (isUnlocked)
        {
            coinAmountText.text = "";
            lockIcon.SetActive(false);
            iconCoin.SetActive(false);
            InactiveBackground.SetActive(false);
        }
        else
        {
            lockIcon.SetActive(true);
            iconCoin.SetActive(true);
            InactiveBackground.SetActive(true);
        }
        playButton.SetActive(isUnlocked);
        unlockButton.SetActive(!isUnlocked);
    }

    // fungsi untuk cek status terkunci atau tidak dari LevelDataController
    public void CheckAndSetLevelLockedState()
    {
        if (levelData != null)
        {
            var levelInfo = levelData.GetLevelData(levelNumber);
            if (levelInfo != null)
            {
                if (levelInfo.status_level == 1)
                {
                    isUnlocked = true;
                }
                else
                {
                    isUnlocked = false;
                }
                UpdateLevelView();
            }
            else
            {
                Debug.LogWarning($"Level data untuk level {levelNumber} tidak ditemukan.");
            }
        }
        else
        {
            Debug.LogError("LevelDataController tidak ditemukan!");
        }
    }

    // Fungsi untuk set level terkunci atau tidak
    public void SetLevelLockedState(bool unlocked)
    {
        isUnlocked = unlocked;
        UpdateLevelView();
    }

    public void OnPlayButtonPressed()
    {
        Debug.Log("Play " + levelName);
        // Jika tombol ditekan
    }
}
