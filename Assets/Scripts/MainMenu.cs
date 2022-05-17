using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button playButton, exitButton;
    
    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit(0);
        });
    }
}
