using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventEngineManager : MonoBehaviour
{
    private string[,] eventdatalist;
    public string Gamemode {get; set;}
    public static EventEngine engine; // SelectDB()에서 모드 엔진 참고 인스턴트. 
    public CEOEventEngine ceoengine;
    //public static Queue<object> sendmailwaitList = new();
    public static Queue<Dictionary<string, object>> sendmailwaitlist = new(); 
    public List<Dictionary<string, object>> tmpeventlist = new(); // 임시적으로 해당 키워드 이벤트 내용들 저장.
    public List<Dictionary<string, object>> releaseevent; // 그 중 메일 몇 개 보낼지 랜덤 결정해서 개수만큼 저장. 
    public Dictionary<string, object> slicedeventlist; // 위 List에서 꺼내 딕셔너리로 키값쌍으로 저장. 
    public Queue<string> ReportHost {get; set;}
    public Queue<string> MailSubject {get; set;}
    public Queue<string> EventContents {get; set;}
    public Queue<int> EventNo {get; set;}
    public GameObject maillist;


    void Start()
    {
        Debug.Log("EventManager Called");
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
        Debug.Log("newgame????? " + newgame);
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
            ceoengine = GameObject.Find("EventEngineManager").AddComponent<CEOEventEngine>();
            ceoengine = GetComponent<CEOEventEngine>();
            engine = ceoengine;
            Debug.Log("engine = " + ceoengine.ToString()); 
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
        Debug.Log("Add Event List called");
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
        slicedeventlist = new();
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
                    Debug.LogFormat("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                }
                else if(kvp.Key =="ReportHost")
                {
                    a = kvp.Value;
                    ReportHost.Enqueue((string)a);
                    Debug.LogFormat("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                }
                else if(kvp.Key == "MailSubject")
                {
                    a = kvp.Value;
                    MailSubject.Enqueue((string)a);
                    Debug.LogFormat("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                }
                else if(kvp.Key == "EventContents")
                {
                    a = kvp.Value;
                    EventContents.Enqueue((string)a);
                    Debug.LogFormat("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
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

        Debug.Log(emailtitle);

        StartCoroutine(SendMail(no, sender, emailtitle, eventcontents));
    }

    IEnumerator SendMail(int no, string sender, string emailtitle, string eventcontents)
    {
        GameObject obj = Instantiate(maillist);
        Transform rct = GameObject.Find("Content").transform;
        Text email_title = GameObject.Find("email_title").GetComponent<Text>(); // 다른 방법을 써야함 중복되서 이전 버튼이 초기화됨 이하 마찬가지 .
        email_title.text = emailtitle;
        Text email_content = GameObject.Find("email_content").GetComponent<Text>();
        email_content.text = eventcontents.Substring(0,20) + "...";
        obj.transform.SetParent(rct);
        float randomtime = UnityEngine.Random.value;
        Debug.Log("randomtime = " + randomtime);
        yield return new WaitForSeconds(randomtime);
    }

    public void DelEventList()
    {
        tmpeventlist.Clear();
    }

    public void AddQueue()
    {
        Debug.Log("tempventlist count = " + tmpeventlist.Count);
        for(int i=0; i<tmpeventlist.Count; i++)
        {
            sendmailwaitlist.Enqueue(tmpeventlist[i]);
            Debug.Log("Whait is Enqueue?? + " + tmpeventlist[i]);
        }
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
    
}
