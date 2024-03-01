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

    public Queue<string> ReportHost {get; set;}
    public Queue<string> MailSubject {get; set;}
    public Queue<string> EventContents {get; set;}
    public Queue<int> EventNo {get; set;}

    public GameObject maillist;


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
            for(int j=0; j<filtered.GetLength(1); j++)
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
        object a = null; // 딕셔너리 값 임시로 담음. 
        EventNo = new();
        ReportHost = new();
        MailSubject = new();
        EventContents = new();
        foreach(Dictionary<string, object> tmp in releaseevent)
            foreach(KeyValuePair<string, object> kvp in tmp)
                if(kvp.Key == "No")
                {
                    a = kvp.Value;
                    EventNo.Enqueue(Convert.ToInt32(a));
                }
                else if(kvp.Key =="ReportHost")
                {
                    a = kvp.Value;
                    ReportHost.Enqueue((string)a);
                }
                else if(kvp.Key == "MailSubject")
                {
                    a = kvp.Value;
                    MailSubject.Enqueue((string)a);
                }
                else if(kvp.Key == "EventContents")
                {
                    a = kvp.Value;
                    EventContents.Enqueue((string)a);
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

        StartCoroutine(SendMail(no, sender, emailtitle, eventcontents));
    }

    IEnumerator SendMail(int no, string sender, string emailtitle, string eventcontents)
    {
        GameObject obj = Instantiate(maillist);
        Transform rct = GameObject.Find("Content").transform;
        Text[] texts = obj.GetComponentsInChildren<Text>();
        if (timectr == null)
        {
            timectr = new();
        }
        EmailButtonPrefab mailist_controller = obj.transform.Find("MailListController").GetComponent<EmailButtonPrefab>();
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

}
