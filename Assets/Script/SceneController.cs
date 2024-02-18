using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    private static string selectedmode;
    public static SceneController sc = null;

    private void Awake()
    {
        if (sc==null)
        {
            sc = this;
            DontDestroyOnLoad(gameObject);
        }
        
    }

    public void OnMainScene()
    {
        SceneManager.LoadScene(0);
    }

    public void OnClickNewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClickGameMode()
    {
       Debug.Log("Called OnClickGameMode()");
       selectedmode = EventSystem.current.currentSelectedGameObject.name;
       Debug.Log(selectedmode);
       SceneManager.LoadScene(2);
    }

    public void OnMailList()
    {
        InputField ipt = GameObject.Find("inputf_username").GetComponent<InputField>();
        string name = ipt.text;
        string btnname = EventSystem.current.currentSelectedGameObject.name;
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.SetInputUserName(name);
        gm.SetInputNewGame(btnname);
        SceneManager.LoadScene(3);
        Destroy(gm);
    }

    public string GetSelectedMode()
    {
        return selectedmode;
    }

}
