using UnityEngine;

public class ControllerUnlockLevel : MonoBehaviour
{
    [Header("Coin Info")]
    public GameObject CoinAmout;

    [Header("Manager Menu Level")]
    public GameObject MenuLevelManager;

    private LevelItemController levelItemController;

    [Header("Window Unlock Level")]
    public GameObject ShadowBackground;
    public GameObject WindowUnlockLevel;
    public GameObject WindowNotEnoughCoin;
    private static LevelDataController levelData; // akses statis
    public void OnClickOke()
    {
        // Tutup window unlock level dengan animasi
        if (WindowUnlockLevel.activeSelf)
        {
            WindowUnlockLevel.GetComponent<AnimatorScale>().PlayHide();
            WindowUnlockLevel.SetActive(false);
            ShadowBackground.SetActive(false);
        }

        // Tutup window not enough coin dengan animasi
        if (WindowNotEnoughCoin.activeSelf)
        {
            WindowNotEnoughCoin.GetComponent<AnimatorScale>().PlayHide();
            WindowNotEnoughCoin.SetActive(false);
            ShadowBackground.SetActive(false);
        }
    }
    public void Awake()
    {
        if (levelData == null)
        {
            levelData = LevelDataController.I;
        }
    }

    // Jika koin cukup, tampilkan window unlock level
    public void ShowUnlockLevelWindow(int levelNumber)
    {
        if (levelData.GetKoinPlayer() >= levelData.GetHargaUnlockLevel(levelNumber))
        {
            ShadowBackground.SetActive(true);
            // tampilkan window unlock level dengan animasi 
            WindowUnlockLevel.SetActive(true);
            WindowUnlockLevel.GetComponent<AnimatorScale>().PlayShow();
            WindowNotEnoughCoin.SetActive(false);
            // Kurangi koin player
            int newKoin = levelData.GetKoinPlayer();
            newKoin -= levelData.GetHargaUnlockLevel(levelNumber);

            levelData.UpdateKoinPlayer(newKoin);

            // Update pada Amount coin text
            if (CoinAmout != null && CoinAmout.GetComponent<TMPro.TextMeshProUGUI>() != null)
            {
                CoinAmout.GetComponent<TMPro.TextMeshProUGUI>().text = newKoin.ToString();
            }

            // Buka level
            levelData.UnlockLevel(levelNumber);

            levelItemController = FindFirstObjectByType<LevelItemController>();

            // Update Tampilan
            MenuLevelManager.GetComponent<MenuLevelManager>().ArrangeLevelItems();

            // Sound Unlock level
            ManagerAudio.instance.PlayUnlockLevelSound();
            
            // Refresh status level di LevelItemController
            if (levelItemController != null)
            {
                levelItemController.GetComponent<LevelItemController>().CheckAndSetLevelLockedState();
                levelItemController.GetComponent<LevelItemController>().UpdateLevelView();
            }
            
        }
        else
        {
            ShadowBackground.SetActive(true);
            // tampilkan window not enough coin dengan animasi
            WindowNotEnoughCoin.SetActive(true);
            WindowNotEnoughCoin.GetComponent<AnimatorScale>().PlayShow();
            WindowUnlockLevel.SetActive(false);
        }
    }

}
