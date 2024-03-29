using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventEngineManager : MonoBehaviour
{
    public static EventEngineManager em;
    private static string[,] eventdatalist;
    public string Gamemode {get; set;}
    public static EventEngine engine; // SelectDB()에서 모드 엔진 참고 인스턴트.
    public static IEventEngineChoice iec;
    public CEOEventEngine ceoengine;
    public TimeController timectr;   
    public static Queue<Dictionary<string, object>> sendmailwaitlist = new(); // release event 결정되기전 큐에 대기.

    public List<Dictionary<string, object>> tmpeventlist = new(); // 임시적으로 해당 키워드 이벤트 내용들 저장.
    public List<Dictionary<string, object>> releaseevent; // 그 중 메일 몇 개 보낼지 랜덤 결정해서 개수만큼 저장.

    public Queue<string> ReportHost { get; set; } = new();
    public Queue<string> MailSubject { get; set; } = new();
    public Queue<string> EventContents { get; set; } = new();
    public Queue<int> EventNo { get; set; } = new();
    public Queue<string> ButtonAction1 { get; set; } = new();
    public Queue<string> ButtonAction2 { get; set; } = new();
    public Queue<string> ButtonAction3 { get; set; } = new();

    public GameObject maillist;

    private Dictionary<int, GameObject> prefabInstanceID = new Dictionary<int, GameObject>();

    public int lastePrefabInstanceID;


    private void Awake()
    {
        if(em==null)
        {
            em = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        timectr = GameObject.Find("TimeController").GetComponent<TimeController>();
    }

    void Start()
    {
        string dbname = SelectDB();
        Debug.Log("DBNAME SELECT SUCCESS");
        ReceiveDB(dbname);
        Debug.Log("DB RECEVIED SUCCESS");
        if(NewGameCheck())
        {
            switch(engine.ToString())
            {
                case "EventEngineManager (CEOEventEngine)":
                ceoengine.InitialEventContents(eventdatalist);
                break;
            } 
        }    
    }
    public bool NewGameCheck()
    {
        bool newgame = GameObject.Find("GameManager").GetComponent<GameManager>().GetNewGame();
        return newgame;
    }

    // Update is called once per frame
    void Update()
    {
        
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
            engine = ceoengine;
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
        string emailtitle = MailSubject.Dequeue();
        string eventcontents = EventContents.Dequeue();
        string buttonaction1 = ButtonAction1.Dequeue();
        string buttonaction2 = ButtonAction2.Dequeue();
        string buttonaction3 = ButtonAction3.Dequeue();

        StartCoroutine(SendMail(no, sender, emailtitle, eventcontents, buttonaction1, buttonaction2, buttonaction3));
    }

    IEnumerator SendMail(int no, string sender, string emailtitle, string eventcontents, string buttonaction1, string buttonaction2, string buttonaction3) // 이 부분에서 이벤트 트랙커를 만들고 버튼도 구분 할 수 있게 해야하겠음 .
    {
        GameObject obj = Instantiate(maillist);
        lastePrefabInstanceID = obj.GetInstanceID();
        Debug.Log("Prefab Isntance ID = " + lastePrefabInstanceID);
        prefabInstanceID.Add(lastePrefabInstanceID, obj);
        Transform rct = GameObject.Find("Content").transform;
        Text[] texts = obj.GetComponentsInChildren<Text>();
        if (timectr == null)
        {
            timectr = new();
        }
        EmailButtonPrefab mailist_controller = obj.transform.Find("MailListController").GetComponent<EmailButtonPrefab>();
        mailist_controller.buttonaction1 = buttonaction1;
        mailist_controller.buttonaction2 = buttonaction2;
        mailist_controller.buttonaction3 = buttonaction3;
        mailist_controller.prefabInstanceID = lastePrefabInstanceID;
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
                    text.text = timectr.TimeGeneration();
                    mailist_controller.txt_mail_receive_time = text.text;
                    break;
            }
        }
        obj.transform.SetParent(rct);
        float randomtime = UnityEngine.Random.value;
        yield return new WaitForSeconds(randomtime);
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
            int releaseeventcount = engine.RandomNumber(waitlistcount);
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

    public void OnClickDelEmail() // 메일 삭제 구현해야 함. 
    {
        EmailButtonPrefab epb = GameObject.Find("MailListController").GetComponent<EmailButtonPrefab>();
        Debug.Log("Instance ID in EmailPrefabClass =" + epb.prefabInstanceID);
        if (epb.btn_decision_action1 && epb.btn_decision_action2 && epb.btn_decision_action3 != null)
        {
            Debug.Log("epb button cehck = " + epb.buttonaction1 + epb.buttonaction2 + epb.buttonaction3);
            Transform tr = GameObject.Find("Canvas").transform.Find("Del_YesOrNo_Panel").transform;
            gameObject.SetActive(tr); // 이 부분 검토 .
            EventControllPanel ecp = GameObject.Find("Del_YesOrNo_Panel").GetComponent<EventControllPanel>();
            ecp.SetConfirmMessage("삭제하시겠습니까?");

            Debug.Log("삭제하시겠습니까?");
        }
        else
        {
            if(prefabInstanceID.TryGetValue(epb.prefabInstanceID, out GameObject instance))
            {
                Destroy(instance);
                prefabInstanceID.Remove(epb.prefabInstanceID);
                epb.mcp.SetActive(false);
                Destroy(epb);
            }
            else
            {
                Debug.Log("실패인?");
            }
        }
    }
}