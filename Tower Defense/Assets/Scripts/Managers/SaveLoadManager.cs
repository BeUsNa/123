using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Managers;

public class SaveLoadManager : MonoBehaviour
{
    public Button continueButton;

    void Start()
    {
        if (!SaveSystem.SaveExists())
        {
            continueButton.interactable = false;
            return;
        }

        SaveData data = SaveSystem.LoadGame();
        if (data == null || string.IsNullOrEmpty(data.sceneName))
        {
            continueButton.interactable = false;
            return;
        }

        continueButton.interactable = true;
    }

    public void ContinueGame()
    {
        if (!SaveSystem.SaveExists()) return;

        SaveSystem.shouldLoadFromSave = true;
        SaveData data = SaveSystem.LoadGame();
        if (data == null || string.IsNullOrEmpty(data.sceneName)) return;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(data.sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Сброс режима строительства и загрузка состояния
        BuildManager.GetInstance()?.CancelBuildMode();
        GameManager.GetInstance()?.LoadGameState();

        Debug.Log("[LOAD] Завершено восстановление из GameManager.");
    }
}
