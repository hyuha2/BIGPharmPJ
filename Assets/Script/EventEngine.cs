using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEngine : EventEngineManager
{
    
    public static EventEngineManager eem;

    private void Start()
    {
        
        if(eem==null)
        {
            eem = GameObject.Find("EventEngineManager").GetComponent<EventEngineManager>();
        }
        
    }

    public int RandomNumber(int count)
    {
        int randomnumber = (int)UnityEngine.Random.Range(1, count+1);
        return randomnumber;
    }

    public int[] RandomEventNo(int ev_generation_count)
    {
        int[] eventno = new int[ev_generation_count];
        for(int i=0; i<eventno.Length; i++)
        {
            int no = UnityEngine.Random.Range(4,GetEventDatalist().GetLength(0));
            eventno.SetValue(no, i);//값을 인덱스에 set. SetValue(Value, index no) 

            for(int j=0; j<i; j++)
            {
                while(eventno[j]==eventno[i]) // evetno가 중복으로 생성 될 경우 재생성을 위해 반복문 들어감.
                {
                    Debug.Log("While statement entered");
                    int temp_no = UnityEngine.Random.Range(4, GetEventDatalist().GetLength(0));
                    eventno.SetValue(temp_no, i);
                    Debug.Log("Whiele statment end point");
                }
            }
        }
        Array.Sort(eventno);
        return eventno;
    }

    public int SpecificDecisionSendEmailCount(string eventtype1) //play evet시 몇 개 보낼지 셀거 
    {
        string[,] eventdata = eem.GetEventDatalist();
        int rowcolcount = eventdata.GetUpperBound(0)+1;
        List<bool> filter = new List<bool>();
        for(int i=0; i<rowcolcount; i++)
        {
           if(eventtype1.Equals(eventdata.GetValue(i,2)))
                filter.Add(true);
        }        
        int decision_sendEmail_count = filter.Count;
        return decision_sendEmail_count;
    }
    
    public object EventDataTransferForEmail(string[,] eventdatalist, int indexfrom, int indexto)
    {
        int  count = (indexto - indexfrom)+1;
        for(int i=0; i<count+1; i++)
        {
            var a = new {sender = eventdatalist.GetValue(indexfrom-1,10), emailSubject = eventdatalist.GetValue(indexfrom-1,13) };
            //from++;
        }
        return eventdatalist;
    }

    public int InitailEventDataTransferCountForEmail(int joincount) // 신규 게임시 적용
    {
        int decision_sendemail_count = 0;
        while(decision_sendemail_count==0)
        {
            decision_sendemail_count = RandomNumber(joincount);
        }
        Debug.Log("decision_sendemail_count =" + decision_sendemail_count); 
        return decision_sendemail_count;
    }

    public string[,] EventFilter(string[,] eventdatalist, string keyword)
    {
        int joincount = KeywordCount(eventdatalist, keyword);
        int rowpoint = 1;
        string[,] filter = new string[joincount+1, eventdatalist.GetLength(1)];
        for(int i =0; i<eventdatalist.GetLength(1); i++) //컬럼명 먼저 저장 
        {
            filter[0,i] = (string)eventdatalist.GetValue(0,i);
        }
        for(int i =0; i < eventdatalist.GetLength(0); i++)
        {
            for(int j=0; j<eventdatalist.GetLength(1); j++)
            {
                if(eventdatalist.GetValue(i,j).Equals(keyword))
                {
                    for(int a=0; a < eventdatalist.GetLength(1); a++)
                    {
                        filter[rowpoint, a] = (string)eventdatalist.GetValue(i,a);
                    }
                     rowpoint++;
                }
            }
        }
        return filter;
    }

    public string[,] EventFilterUsingEvetNo(int[] eventno)
    {
        string[,] eventdatalist = eem.GetEventDatalist();
        int rowlength = eventdatalist.GetLength(1);
        Debug.Log("rowlength = " + rowlength);
        string[,] contents = new string[eventno.Length+1, rowlength];
        for(int i=0; i<eventno.Length; i++)
        {
            for(int j=0; j<rowlength; j++)
            {
                if(i==0)
                {
                    for(int c=0; c<rowlength; c++)
                    {
                        contents[i, c] = eventdatalist[0, c];
                    }    
                }
                contents[i+1, j] = eventdatalist[eventno[i], j];
            }
        }
        return contents;
    }
    
    public void InitialEventContents(string[,] eventdatalist)
    {
        eem = GameObject.Find("EventEngineManager").GetComponent<EventEngineManager>();
        string[,] filter = EventFilter(eventdatalist, "Join"); //Join으로 분류된 이벤트 리스트를 가져옴.
        eem.AddEventList(filter); // 분류된 모든 이벤트 키워드 내용 리스트에 딕셔너리 타입으로 저장 
        eem.AddQueue(); // 위 리스트를 큐에 모두 담음 (굳이???? 빼내오기 쉬울라고..쓴거)
        int releaseevnetcount = eem.ReleaseEventCount(); // 그 중 몇 개의 초기 이벤트를 방출할건지 개수를 결정
        eem.DicBindingInList(releaseevnetcount); // 결정된 개수만큼 큐에서 꺼내 릴리즈 이벤트 리스트에 딕셔너리 타입으로 다시 담음
        eem.SliceDicinQueForEmailListDataSet(); // 메일 리스트에 필요한 데이터 각 요소를 큐로 각 요소별로 담음
        for (int i = 0; i < releaseevnetcount; i++)
        {
            eem.PrefabEmailListDequeAfterDataSet();
        }
    }

    public void EventGeneration()
    {
        Debug.Log("EvnetGeneration()호출");
        int event_generation_decison_count = (int)UnityEngine.Random.Range(1, 4);
        int[] eventno = RandomEventNo(event_generation_decison_count);
        string[,] filter = EventFilterUsingEvetNo(eventno);
        WokrFlowListandDicandQueueandPrefabEmail(filter);
        //engine.EventGeneration(event_generation_decison_count, eventno);
    }

    public void WokrFlowListandDicandQueueandPrefabEmail(string[,] filter) // 이니셜 이벤트 가져다 붙였더니 초기 이벤트가 도출되니 reowpoint 쪽에 아마 수정 필.
    {
        eem.AddEventList(filter); //tempeventlist에 등록.
        eem.AddQueue(); //sendmailwaitlist 큐에 등록.
        int releaseevnetcount = eem.ReleaseEventCount(); // 그 중 몇 개의 초기 이벤트를 방출할건지 개수를 결정
        eem.DicBindingInList(releaseevnetcount); // 결정된 개수만큼 큐에서 꺼내 릴리즈 이벤트 리스트에 딕셔너리 타입으로 다시 담음
        eem.SliceDicinQueForEmailListDataSet(); // 메일 리스트에 필요한 데이터 각 요소를 큐로 각 요소별로 담음
        for (int i = 0; i < releaseevnetcount; i++)
        {
            eem.PrefabEmailListDequeAfterDataSet();
        }
    }

    public int KeywordCount(string[,] datalist, string keyword)
    {
        int count = 0;
        for(int i =0; i < datalist.GetLength(0); i++)
        {
            for(int j=0; j<datalist.GetLength(1); j++)
            {
                if(datalist.GetValue(i,j).Equals(keyword))
                {
                     count++;
                }
            }
        }
        return count;
    }
}
