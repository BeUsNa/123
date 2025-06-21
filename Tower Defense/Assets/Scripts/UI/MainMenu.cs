using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;          
#endif
namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public void StartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}
