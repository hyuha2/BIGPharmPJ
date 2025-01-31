using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gm = null;
    private static string username;
    private static string time;
    private static bool newgame = false;
    private float interest_rate { get; set; } = 3.5f;
    public string CurrentGameDataFile {get; set;}
    public SaveData saveData;

    private void Awake()
    {
        Debug.Log("Awake() is called in GameManager");
        if(gm == null)
        {
            gm = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("gm is null");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("gm is not null");
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadInitailized()
    {
        Awake();
    }

     public void SetInputUserName(string name)
    {
        username = name;
        Debug.Log(username);
    }

    public string GetUserName()
    {
        return username;
    }

    public void SetInputNewGame(string btnname)
    {
        newgame = btnname.Equals("btn_nexttoselectmode"); // 새 게임 시작시 최초 시작시에만 만나게되는 btn_nexttoselectmode 버튼의 버튼 이름이 맞는 경우 bool newgame에 true를 넘기므로 신규게임인지 아닌지 필터링 역할.
        Debug.Log("is New Game???" + newgame);
    }

    public bool GetNewGame(){
        return newgame;
    }

    public void SetNewGame(bool b)
    {
        newgame = b;
    }

    public string GetInterest_Rate()
    {
        string rate = interest_rate.ToString();
        return rate;
    }
    
    public float GetInterest_Rate_Float()
    {
        return interest_rate;
    }

    public void Save()
    {
        SaveData saveData = SaveMaterial();
        XmlManager xm = new XmlManager();

        if(CurrentGameDataFile == null)
        {
            string fileName = "BigPharma_" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".xml";
            string path = Application.persistentDataPath + "/" + fileName;
            try
            {
                XmlManager.XmlSave<SaveData>(saveData, path);
            }
            catch (DirectoryNotFoundException ex)
            {
                Debug.Log(ex.Message);
            }
            PanelController.PC.GenerationPopupMessage($"{path}와 같이 저장 되었습니다.");
            PanelController.PC.DisadbleGameObject("SavePanel");
            CurrentGameDataFile = path;
        }
        else
        {
            try
            {
                XmlManager.XmlSave<SaveData>(saveData, CurrentGameDataFile);
            }
            catch (DirectoryNotFoundException ex)
            {
                Debug.Log(ex.Message);
            }
            PanelController.PC.GenerationPopupMessage($"현재 파일에 저장 되었습니다.");
            PanelController.PC.DisadbleGameObject("SavePanel");
        }
        
        
    }
    public void SaveAs()
    {
        SaveData saveData = SaveMaterial();
        XmlManager xm = new XmlManager();
        string fileName = GameObject.Find("txt_save_as_filename").GetComponent<Text>().text;
        string path = Application.persistentDataPath + "/" + fileName + ".xml";
        try
        {
            XmlManager.XmlSave<SaveData>(saveData, path);
        }
        catch (DirectoryNotFoundException ex)
        {
            Debug.Log(ex.Message);
        }
        PanelController.PC.GenerationPopupMessage($"{path}와 같이 저장 되었습니다.");
        PanelController.PC.DisadbleGameObject("SavePanel");
        CurrentGameDataFile = path;
    }

    public SaveData SaveMaterial()
    {
        SaveData saveData = new SaveData();
        SerializableDictionary<String, object> sd = new();

        saveData.UserName = username;
        saveData.GameCurrentTime = TimeController.tc.DateTimeParseFromString(TimeController.tc.GameTime);
        saveData.Company_Money = PlayData.playData.GetCompany_Money();
        saveData.HiredList = EmployeeManager.epm.hired_person;
        saveData.GameMode = EventEngineManager.em.Gamemode;
        saveData.selectmode = SceneController.sc.GetSelectedMode();
        saveData.EmployeeDataList = FlattenArray(EmployeeManager.epm.employees);
        saveData.EventDataList = FlattenArray(EventEngineManager.em.GetEventDatalist());
        saveData.MailPrefap = GetMailPrefab();  
        Debug.Log("Check of EventDataList Length = " + saveData.EventDataList.Length);
        //saveData.general_instance_investor = Investor.investor;
        //saveData.EPM = EmployeeManager.epm;
        //saveData.Invest_Money = Investor.investor.total_InverstedMoney;
        saveData.companyInformation = sd.SetCompanyInformation(); // 딕셔너리를 SerializableDictionary에 풀어내는 것이 필요.
        saveData.companyNo = EventEngineManager.companyNo;
        saveData.ProductInformation = PlayData.ProductInformation;
        return saveData;
    }

    public List<EmailButtonDataForSave> GetMailPrefab()
    {
        GameObject content = GameObject.Find("Content");
        EmailButtonPrefab[] mail = content.GetComponentsInChildren<EmailButtonPrefab>();
        List<EmailButtonDataForSave> mails = new();
        for (int i =0; i<mail.Length; i++)
        {
            EmailButtonDataForSave email = new();
            email.prefabInstanceID = mail[i].prefabInstanceID;
            email.mailContentScriptID = mail[i].mailContentScriptID;
            email.no = mail[i].no;
            email.txt_mail_title = mail[i].txt_mail_title;
            email.txt_mail_receive_sender = mail[i].txt_mail_receive_sender;
            email.txt_mail_receive_content = mail[i].txt_mail_receive_content;
            email.txt_mail_receive_time = mail[i].txt_mail_receive_time;
            email.buttonaction1 = mail[i].buttonaction1;
            email.buttonaction2 = mail[i].buttonaction2;
            email.buttonaction3 = mail[i].buttonaction3;
            email.facepath = mail[i].facepath;
            mails.Add(email);
        }
        Debug.Log(mail.Length + "/" + mails.Count);
        return mails;
    }

    public T[] FlattenArray<T>(T[,] multiArray)
    {
        int rows = multiArray.GetLength(0);
        int cols = multiArray.GetLength(1);
        T[] flatArray = new T[rows * cols];

        for(int r = 0; r <rows; r++)
        {
            for(int c = 0; c<cols; c++)
            {
                flatArray[r * cols + c] = multiArray[r, c];
            }
        }

        return flatArray;
    }

    public static T[,] ExpandArray<T>(T[] flatArray, int rows, int cols)
    {
        if (flatArray.Length != rows * cols)
            throw new InvalidOperationException("InvalidOperatrionException");

        T[,] multiArray = new T[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int c = 0; c < cols; c++)
            {
                multiArray[i, c] = flatArray[i * cols + c];
            }
        }
        return multiArray;
    }
    /*
    public void Load(string fileName)
    {
        saveData = new();
        XmlManager xml = new();
        string path = fileName;
        try
        {
            saveData = XmlManager.XmlLoad<SaveData>(path);
            SceneController.sc.SetSelectedMode(saveData.selectmode);
            SceneManager.LoadScene(3);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.isLoaded)
        {
            LoadMaterial(saveData);
            Button conti = GameObject.Find("btn_continue").GetComponent<Button>();
            Button menubar = GameObject.Find("btn_menubar").GetComponent<Button>();
            if(conti!=null)
            {
                conti.onClick.RemoveAllListeners();
                conti.onClick.AddListener(EventEngineManager.em.RemainEventCehck);
            }
            if (menubar != null)
            {
                menubar.onClick.RemoveAllListeners();
                menubar.onClick.AddListener(PanelController.PC.OnclickMenuButton);
            }
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadMaterial(SaveData saveData)
    {
        SceneController.sc.SetSelectedMode(saveData.selectmode);
        username = saveData.UserName;
        DateTime time = saveData.GameCurrentTime;
        TimeController.tc.SetCurrentTime(time);
        EventEngineManager.em.Gamemode = saveData.GameMode;
        if (saveData.EventDataList == null)
            throw new InvalidOperationException("EventDataList is null or empty.");
        EmployeeManager.epm.employees = ChangeDemensionList(13, saveData.EmployeeDataList);
        EventEngineManager.em.SetEventDataList(ChangeDemensionList(20, saveData.EventDataList));
        PlayData.playData.SetCompany_Money(saveData.Company_Money);
        foreach (int person in saveData.HiredList)
            EmployeeManager.epm.hired_person.Add(person);
        SetMailBox();
    }

    public void LogButtonListeners(string context)
    {
        Button[] buttons = GameObject.Find("Canvas").transform.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            Debug.Log($"{context} - Button: " + button.name);
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            {
                var target = button.onClick.GetPersistentTarget(i);
                var methodName = button.onClick.GetPersistentMethodName(i);
                Debug.Log($"{context} - Listener {i}: target={target}, method={methodName}");
            }
        }
    }*/

    public string[,] ChangeDemensionList(int expectedElements, string[] list)
    {
        int rows = list.Length / expectedElements;
        if (list.Length % expectedElements != 0)
            throw new InvalidOperationException("EventDataList length is not a multiple of expected row length.");

        string[,] datalist = ExpandArray(list, list.Length / expectedElements, expectedElements);
        return datalist;
    }

    /*public void SetMailBox()
    {
        foreach (EmailButtonDataForSave load in saveData.MailPrefap)
        {
            StartCoroutine(EventEngineManager.em.SendMail(load.no, load.txt_mail_receive_sender, load.txt_mail_title, load.txt_mail_receive_content, load.buttonaction1, load.buttonaction2, load.buttonaction3, load.facepath, load.txt_mail_receive_time));
        }
    }*/

}