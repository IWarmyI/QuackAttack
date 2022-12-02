using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static AudioSliders;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    public GameObject main;
    public GameObject difficulty;
    public GameObject options;
    public GameObject customize;
    public GameObject credits;

    public GameObject mainMenuFirstOption;
    public GameObject DifficultyFirstOption;
    public GameObject OptionsFirstOption;
    public GameObject CustomizeFirstOption;
    public GameObject CreditsFirstObject;

    public GameObject canvas;
    private static bool firstTime = true;

    public InputActionAsset actions;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
        Time.timeScale = 1.0f;

        if (firstTime)
        {
            musicFloat = 0.5f;
            sfxFloat = 0.5f;
        }
        canvas.GetComponent<AudioSource>().volume = musicFloat;

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
    }

    public void Save()
    {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    void Update()
    {
        canvas.GetComponent<AudioSource>().volume = musicFloat;
    }

    public void DifficultyButton()
    {
        // Play Game
        Player.Initialize();
        SceneManager.LoadScene("Game");
    }

    public void TutorialButton()
    {
        firstTime = false;
        //Play Tutorial
        Player.Initialize();
        LevelManager.Instance.NewGame();
    }
    public void PlayLevel(int level)
    {
        firstTime = false;
        Player.Initialize();
        LevelManager.Instance.LoadNewLevel(LevelManager.FirstLevel + level);
    }

    public void StartButton()
    {
        // Show Main Menu
        options.SetActive(false);
        credits.SetActive(false);
        main.SetActive(false);
        customize.SetActive(false);
        difficulty.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(DifficultyFirstOption);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        options.SetActive(false);
        credits.SetActive(false);
        difficulty.SetActive(false);
        customize.SetActive(false);
        main.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstOption);

    }

    public void OptionsButton()
    {
        // Show Options
        main.SetActive(false);
        credits.SetActive(false);
        difficulty.SetActive(false);
        customize.SetActive(false);
        options.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(OptionsFirstOption);
    }

    public void CustomizeButton()
    {
        // Show Customization Options
        main.SetActive(false);
        credits.SetActive(false);
        difficulty.SetActive(false);
        options.SetActive(false);
        customize.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(CustomizeFirstOption);

    }

    public void CreditsButton()
    {
        // Show Credits Menu
        main.SetActive(false);
        options.SetActive(false);
        difficulty.SetActive(false);
        customize.SetActive(false);
        credits.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(CreditsFirstObject);
    }
}