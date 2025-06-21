using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes
{
    public class LevelLoader : MonoBehaviour
    {
        public void LoadGameScene(int lvl)
        {
            Preferences.SetCurrentLvl(lvl);
            SaveSystem.shouldLoadFromSave = false; 
            SceneManager.LoadScene("Level " + lvl);
        }

        public void LoadLevel(string sceneName)
        {
            SaveSystem.shouldLoadFromSave = false; 
            SceneManager.LoadScene(sceneName);
        }

        public void LoadLevelSelectionScene() => SceneManager.LoadScene("Level Selection");
        public void LoadCurrentScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        public void LoadNextLvlScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        public void LoadOptionsScene() => SceneManager.LoadScene("Options");
        public void LoadMainMenuScene() => SceneManager.LoadScene("Menu");
    }
}
