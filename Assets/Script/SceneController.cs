using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SceneController : MonoBehaviour
{
    private static string selectedmode;
    public static SceneController sc;

    public Button btn_ceomode;
    public Button btn_backtomenu;
    public Button btn_nexttoselectmode;
    public Button btn_newgame;
    public Button btn_loadgame;
    public Button btn_configuration;

    public MainButtonController mbc;

    private void Awake()
    {
        Debug.Log("Awake() is called in SceneController");
        if (sc == null)
        {
            sc = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGameButtonActiviation()
    {
        if(btn_ceomode == null)
        {
            btn_ceomode = GameObject.Find("btn_ceomode").GetComponent<Button>();
            btn_ceomode.onClick.AddListener(OnClickGameMode);
        }
    }

    public void SceneTwoButtonActiviation()
    {
        if (btn_backtomenu == null)
        {
            btn_ceomode = GameObject.Find("btn_backtomenu").GetComponent<Button>();
            btn_ceomode.onClick.AddListener(OnMainScene);
        }

        if(btn_nexttoselectmode == null)
        {
            btn_nexttoselectmode = GameObject.Find("btn_nexttoselectmode").GetComponent<Button>();
            btn_nexttoselectmode.onClick.AddListener(OnMailList);
        }
    }

    public void MainMenuButtonActiviation()
    {
        if(btn_newgame == null)
        {
            btn_newgame = GameObject.Find("btn_newgame").GetComponent<Button>();
            btn_newgame.onClick.AddListener(OnClickNewGame);
        }
        if(btn_loadgame == null)
        {
            if(mbc == null)
            {
                mbc = GameObject.Find("MainButtonController").GetComponent<MainButtonController>();
            }
            btn_loadgame = GameObject.Find("btn_loadgame").GetComponent<Button>();
            btn_loadgame.onClick.AddListener(mbc.OnClickLoadButton);
        }
    }

    public void OnMainSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.isLoaded)
        {
            MainMenuButtonActiviation();
        }
        SceneManager.sceneLoaded -= OnMainSceneLoaded;
    }

    public void OnSceneLoaded_One(Scene scene, LoadSceneMode mode)
    {
        if (scene.isLoaded)
        {
            NewGameButtonActiviation();
        }
        SceneManager.sceneLoaded -= OnSceneLoaded_One;
        btn_ceomode.onClick.RemoveListener(OnClickNewGame);
    }

    public void OnSceneLoaded_Two(Scene scene, LoadSceneMode mode)
    {
        if (scene.isLoaded)
        {
            SceneTwoButtonActiviation();
        }
        SceneManager.sceneLoaded -= OnSceneLoaded_Two;
    }

    public void OnMainScene()
    {
        SceneManager.LoadScene(0);
        SceneManager.sceneLoaded += OnMainSceneLoaded;
        try
        {
            btn_backtomenu.onClick.RemoveListener(OnMainScene);
        }
        catch (NullReferenceException e)
        {
            return;
        }
        
    }

    public void OnClickNewGame()
    {
        SceneManager.LoadScene(1);
        SceneManager.sceneLoaded += OnSceneLoaded_One;
    }

    public void OnClickGameMode()
    {
        selectedmode = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(selectedmode);
        SceneManager.LoadScene(2);
        SceneManager.sceneLoaded += OnSceneLoaded_Two;
    }

    public void OnMailList()
    {
        InputField ipt = GameObject.Find("inputf_username").GetComponent<InputField>();
        string name = ipt.text;
        string btnname = EventSystem.current.currentSelectedGameObject.name;
        GameManager.gm.SetInputUserName(name);
        GameManager.gm.SetInputNewGame(btnname);
        SceneManager.LoadScene(3);
        btn_nexttoselectmode.onClick.RemoveListener(OnMailList);
    }

    public string GetSelectedMode()
    {
        Debug.Log("selectedmode = " + selectedmode);
        return selectedmode;
    }

    public void SetSelectedMode(string selectmode)
    {
        selectedmode = selectmode;
    }

}
