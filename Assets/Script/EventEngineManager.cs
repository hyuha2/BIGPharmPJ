using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class EventEngineManager : MonoBehaviour
{
    public static EventEngineManager em;
    private static string[,] eventdatalist;
    public string[,] pharmaceutical_list;
    public string Gamemode {get; set;}
    public static EventEngine engine; // SelectDB()에서 모드 엔진 참고 인스턴트.
    public static IEventEngineChoice iec;
    public static CSVController csvc;
    public static MFGScheduler mfg_scheduler;
    public static QualityScheduler quality_scheduler;
    public MarketReserachReport mrr;
    public CEOEventEngine ceoengine;
    public TimeController timectr;
    public MailInstanceIDHub hub;
    public EventFromButtonOne one;
    public EventFromButtonTwo two;

    public static int MFG_Order;
    public static int qc_order_seq_no;

    public long globalPopulation = 8161972572; // 세계 인구

    public GameObject maillist;

    public List<Dictionary<string, object>> tmpeventlist = new(); // 임시적으로 해당 키워드 이벤트 내용들 저장.
    public List<Dictionary<string, object>> releaseevent; // 그 중 메일 몇 개 보낼지 랜덤 결정해서 개수만큼 저장.
    public static List<int> companyNo = new();
    public static List<int> controlledEventNo = new();
    public static List<Deviation> deviations_list = new();

    public static Queue<Dictionary<string, object>> sendmailwaitlist = new(); // release event 결정되기전 큐에 대기.
    public Queue<string> EventType1 { get; set; } = new();
    public Queue<string> ReportHost { get; set; } = new();
    public Queue<string> MailSubject { get; set; } = new();
    public Queue<string> EventContents { get; set; } = new();
    public Queue<int> EventNo { get; set; } = new();
    public Queue<string> ButtonAction1 { get; set; } = new();
    public Queue<string> ButtonAction2 { get; set; } = new();
    public Queue<string> ButtonAction3 { get; set; } = new();

    private Dictionary<int, GameObject> prefabInstanceID = new Dictionary<int, GameObject>(); // 메일 관리 차원에서 만들었던 것 같은... 
    public static Dictionary<string, Company> companyInformation = new();

    public int lastePrefabInstanceID;
    public string selected_mode_engine_name;

    private void Awake()
    {
        Debug.Log("Awake() called in EventEngineManager");
        if (em==null)
        {
            em = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            StackTrace stackTrace = new();
            for(int i=1; i<stackTrace.FrameCount; i++)
            {
                StackFrame stf = stackTrace.GetFrame(i);
                if (stf.GetMethod() != null && stf.GetMethod().DeclaringType != null)
                {
                    Debug.Log($"{stf.GetMethod().DeclaringType.FullName}.{stf.GetMethod().Name}");
                }
            }
            //Destroy(gameObject);
        }

        csvc = GameObject.Find("CSVController").GetComponent<CSVController>();
    }

    void Start()
    {
        string dbname = SelectDB();
        Debug.Log("DBNAME SELECT SUCCESS");
        ReceiveDB(dbname);
        Debug.Log("DB RECEVIED SUCCESS");
        timectr = GameObject.Find("TimeController").GetComponent<TimeController>();
        pharmaceutical_list = EventEngineManager.csvc.CSVRead("Bio_pharmaceutical_data.csv");
        mfg_scheduler = GameObject.Find("MFGScheduler").GetComponent<MFGScheduler>();
        one = new();
        two = new();

        if (NewGameCheck())
        {
            PlayData.playData.SetCompany_Money(1000000000);
            Product product = new Product()
            {
                pharmaceutical_data_no = 3000,
                BrandName = "자가맙",
                IngredientName = "Infliximab"
            };
            PlayData.ProductInformation.Add(product);
            switch(selected_mode_engine_name) //origin : engine.ToString()
            {
                case "EventEngineManager (CEOEventEngine)":
                    ceoengine.InitialEventContents(eventdatalist);
                break;
            }
            MFG_Order = 1; //생산 오더에 대한 키 값으로 사용 예정
            qc_order_seq_no = 1;
        }
       
    }
    public bool NewGameCheck()
    {
        bool newgame = GameManager.gm.GetNewGame();
        return newgame;
    }

    public void SetEventDataList(string[,] EventDataList)
    {
        eventdatalist = EventDataList;
    }

    public string[,] GetEventDataList()
    {
        return eventdatalist;
    }

    private string SelectDB()
    {
        SceneController sc = GameObject.Find("SceneController").GetComponent<SceneController>();
        string btnname = sc.GetSelectedMode();

        switch(btnname)
        {
            case "btn_ceomode":
                btnname = "EventDB_CEO.csv";
                Gamemode = "CEO";
                ceoengine = GameObject.Find("EventEngineManager").AddComponent<CEOEventEngine>().GetComponent<CEOEventEngine>();
                selected_mode_engine_name = ceoengine.ToString();
                engine = new EventEngine();
                iec = new CEOEventEngine();
            break;
        }
        
        return btnname;
    }
    
    void ReceiveDB(string dbname)
    {
        CSVController csv = GameObject.Find("CSVController").GetComponent<CSVController>();
        eventdatalist = csv.CSVRead(dbname);     
    }

    public void AddEventList(string [,] filtered)
    {
        for(int i=1; i<filtered.GetLength(0); i++)
        {
            Dictionary<string, object> event_contents = new();
            for (int j=0; j<filtered.GetLength(1); j++)
            {
                event_contents[filtered[0,j]]= filtered[i,j];
            }
            tmpeventlist.Add(event_contents);
        }

    }
    public void DicBindingInList(int count)
    {
        releaseevent = new();
        for(int i=0; i<count; i++)
        {
            releaseevent.Add(sendmailwaitlist.Dequeue());
        }
    }

    public void SliceDicinQueForEmailListDataSet()
    {
        foreach (Dictionary<string, object> tmp in releaseevent)
            foreach (KeyValuePair<string, object> kvp in tmp)
                if(kvp.Key == "No")
                {
                    EventNo.Enqueue(Convert.ToInt32(kvp.Value));
                }
                else if(kvp.Key =="ReportHost")
                {
                    ReportHost.Enqueue((string)kvp.Value);
                }
                else if(kvp.Key == "MailSubject")
                {
                    MailSubject.Enqueue((string)kvp.Value);
                }
                else if(kvp.Key == "EventContents")
                {
                    EventContents.Enqueue((string)kvp.Value);
                }
                else if(kvp.Key == "ButtonAction1")
                {
                    ButtonAction1.Enqueue((string)kvp.Value);
                }
                else if(kvp.Key == "ButtonAction2")
                {
                    ButtonAction2.Enqueue((string)kvp.Value);
                }
                else if(kvp.Key == "ButtonAction3")
                {
                    ButtonAction3.Enqueue((string)kvp.Value);
                }
                else
                {
                    continue;
                }
    }

    public void PrefabEmailListDequeAfterDataSet()
    {
        int no = EventNo.Dequeue();
        string sender = ReportHost.Dequeue();
        EmployeeManager em = GameObject.Find("EmployeeManager").GetComponent<EmployeeManager>();
        int empNo = em.EmailSender_Random(sender);
        string[] profile = em.Employee_information(empNo);
        sender = profile[1];
        string faceURL = profile[12];
        string emailtitle = MailSubject.Dequeue();
        string eventcontents = EventContents.Dequeue();
        string buttonaction1 = ButtonAction1.Dequeue();
        string buttonaction2 = ButtonAction2.Dequeue();
        string buttonaction3 = ButtonAction3.Dequeue();
        string time = TimeController.tc.TimeGeneration();

        StartCoroutine(SendMail(no, sender, emailtitle, eventcontents, buttonaction1, buttonaction2, buttonaction3, faceURL, time));
    }

    public IEnumerator SendMail(int no, string sender, string emailtitle, string eventcontents, string buttonaction1, string buttonaction2, string buttonaction3, string faceURL, string time) // 이 부분에서 이벤트 트랙커를 만들고 버튼도 구분 할 수 있게 해야하겠음 .
    {
        GameObject obj = Instantiate(maillist);
        lastePrefabInstanceID = obj.GetInstanceID();
        Debug.Log("Prefab Isntance ID = " + lastePrefabInstanceID);
        Transform rct = GameObject.Find("Content").transform;
        Text[] texts = obj.GetComponentsInChildren<Text>();
        //Image profile = obj.GetComponent<Image>();
        EmailButtonPrefab mailist_controller = obj.transform.Find("MailListController").GetComponent<EmailButtonPrefab>();
        mailist_controller.buttonaction1 = buttonaction1;
        mailist_controller.buttonaction2 = buttonaction2;
        mailist_controller.buttonaction3 = buttonaction3;
        mailist_controller.txt_mail_receive_sender = sender;
        mailist_controller.prefabInstanceID = lastePrefabInstanceID;
        prefabInstanceID.Add(lastePrefabInstanceID, obj);
        foreach (Text text in texts)
        {
            switch (text.name)
            {
                case "email_title":
                    text.text = emailtitle;
                    mailist_controller.txt_mail_title = emailtitle;
                    break;

                case "email_content":
                    if (no == 1)
                    {
                        //text.text = GameManager.gm.GetUserName() + eventcontents.Substring(0, 30) + "...";
                        text.text = GameManager.gm.GetUserName() + eventcontents;
                        mailist_controller.txt_mail_receive_content = text.text;
                    }
                    else
                    {
                        //text.text = eventcontents.Substring(0, 30) + "...";
                        text.text = eventcontents;
                        mailist_controller.txt_mail_receive_content = eventcontents;
                    }
                    break;

                case "email_receive_time":
                    text.text = time;
                    mailist_controller.txt_mail_receive_time = text.text;
                    break;
            }
        }
        AddForControlledEventNo(no, lastePrefabInstanceID); // 이벤트 관리를 위해 'EventType1' 로 지정된 이벤트에 대해 발생 이벤트 번호 추가 
        obj.transform.SetParent(rct);
        yield return new WaitForSeconds(1f);
    }
    public void AddForControlledEventNo(int evetNo, int prefabInstanceID)
    {
        try
        {
            if(eventdatalist[evetNo, 1] == "Play")
            {
                controlledEventNo.Add(prefabInstanceID);

                if(eventdatalist[evetNo, 2] == "Deviation")
                {
                    if(eventdatalist[evetNo, 3] == "Manufacture")
                    {
                        string devScenario = new Deviation().Generation_Deviation_Scenario(prefabInstanceID, "Manufacture");
                    }
                }
            }
        }
        catch(Exception e)
        {

        }
    }

    public void DelTmpEventList()
    {
        tmpeventlist.Clear();
    }

    public void AddQueue()
    {
        for(int i=0; i<tmpeventlist.Count; i++)
        {
            sendmailwaitlist.Enqueue(tmpeventlist[i]);
        }
        DelTmpEventList();
    }

    public int ReleaseEventCount()
    {
        int quecount= sendmailwaitlist.Count;
        //int releaseevnetcount = engine.RandomNumber(quecount);
        int releaseevnetcount = engine.RandomNumber(quecount);
        Debug.Log("ReleaseEventCount = " + releaseevnetcount);
        return releaseevnetcount;
    }

    public string[,] GetEventDatalist()
    {
        return eventdatalist;
    }

    public void RemainEventCehck()
    {
        if(sendmailwaitlist.Count == 0)
        {
            engine.EventGeneration();
        }
        else
        {
            int waitlistcount = sendmailwaitlist.Count;
            Debug.Log("snedmialwaitlist의 잔여 대기 메일수는 " + waitlistcount + " 입니다.");
            int releaseeventcount = 0;
            if (waitlistcount == 1)
            {
                releaseeventcount = 1;
            }
            else
            {
                releaseeventcount = engine.RandomNumber(waitlistcount);
            }
            DicBindingInList(releaseeventcount); // 결정된 개수만큼 큐에서 꺼내 릴리즈 이벤트 리스트에 딕셔너리 타입으로 다시 담음
            SliceDicinQueForEmailListDataSet(); // 메일 리스트에 필요한 데이터 각 요소를 큐로 각 요소별로 담음
            for (int i = 0; i < releaseeventcount; i++)
            {
                PrefabEmailListDequeAfterDataSet();
            }
        }
    }

    public EventEngine GetEventEngine() 
    {
        return engine;
    }

    public void OnClickDelEmail()
    {
        if (hub == null)
        {
            Debug.Log("HUB 생성");
            hub = GameObject.Find("Mail_Content_Panel").GetComponent<MailInstanceIDHub>();
        }

        if (prefabInstanceID.TryGetValue(hub.mailInstanceID, out GameObject instance))
        {
            Debug.Log("삭제를 위한 instanceID 확인 = " + hub.mailInstanceID);
            Debug.Log("out instance 값 존재 여부 확인 = " + instance);
            if (hub.mcsi.TryGetValue(hub.mailContetnScriptID, out EmailButtonPrefab ebp))
            {
                Debug.Log("out ebp 값 존재 여부 확인 = " + ebp);
                Debug.Log("out ebp 값 button 값 결과 확인 = " + ebp.buttonaction1 + "\t" + ebp.buttonaction2 + "\t" + ebp.buttonaction3);
                if (!ebp.buttonaction1.Contains("N/A"))
                {
                    Transform tr = GameObject.Find("Canvas").transform.Find("Del_YesOrNo_Panel").transform;
                    tr.gameObject.SetActive(true);
                    EventConfirmPanel ecp = GameObject.Find("Del_YesOrNo_Panel").GetComponent<EventConfirmPanel>();
                    ecp.SetConfirmMessage("삭제하시겠습니까?");
                    ecp.SetButton("예", "아니오");
                    ecp.DeinteractableButtoninMCPPanel();
                }
                else
                {
                    Destroy(instance);
                    ebp.mcp.SetActive(false);
                    prefabInstanceID.Remove(hub.mailInstanceID);
                    //PlayData.playData.DelMailBox(hub.mailInstanceID);
                    hub.mcsi.Remove(hub.mailContetnScriptID);
                    Destroy(ebp);
                }

            }
        }
        
    }

    public void OnClickDelEmailFromConfirmPanel()
    {
        if (hub == null)
        {
            Debug.Log("HUB 생성");
            hub = GameObject.Find("Mail_Content_Panel").GetComponent<MailInstanceIDHub>();
        }
        else
        {
            Debug.Log("HUB 이미 생성되어 있다 ");
        }
        EventConfirmPanel ecp = GameObject.Find("Del_YesOrNo_Panel").GetComponent<EventConfirmPanel>();
        ecp.gameObject.SetActive(false);
        GameObject mcp = GameObject.Find("Mail_Content_Panel");
        mcp.gameObject.SetActive(false);

        if (prefabInstanceID.TryGetValue(hub.mailInstanceID, out GameObject instance))
        {
            Destroy(instance);
            prefabInstanceID.Remove(hub.mailInstanceID);
            //PlayData.playData.DelMailBox(hub.mailInstanceID);
            
            if (hub.mcsi.TryGetValue(hub.mailContetnScriptID, out EmailButtonPrefab ebp))
            {
                Destroy(ebp);
                hub.mcsi.Remove(hub.mailContetnScriptID);
            }
        }
    }

    public void OnClickActionButtonOne() // EventFromButton 에서 모든 이벤트를 핸들링 . 
    {
        if (hub == null)
        {
            hub = GameObject.Find("Mail_Content_Panel").GetComponent<MailInstanceIDHub>();
        }
        one.Arrange(hub.buttonaction_name_one, hub.mailInstanceID);
    }

    public void OnClickActionButtonTwo() // EventFromButton 에서 모든 이벤트를 핸들링 . 
    {
        if (hub == null)
        {
            hub = GameObject.Find("Mail_Content_Panel").GetComponent<MailInstanceIDHub>();
        }
        two.Arrange(hub.buttonaction_name_two, hub.mailInstanceID);
    }

    public string GetPharmaceutical_Name(int no)
    {
        return pharmaceutical_list[no-3000, 1];
    }
}