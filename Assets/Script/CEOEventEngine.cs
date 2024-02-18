using System;
using System.Collections.Generic;
using UnityEngine;

public enum DBcolumnNo {No,EventType1,EventType2,EventType3,EventCondition_Money,EventCondition_StaffCount,EventCondition_Licence,EventCondition_Time,EventCondition_TimeInterval_Week,EventOccuredHost,ReportHost,EventTracker,MailSubject,EventContents,GetMoney,SetMoney,ButtonActivation,ButtonAction1,ButtonAction2,ButtonAction3};
public enum EventCategory { Deviation,Attracting_Investment,Sales,Complain,HR,Regulatory,Finance};
public class CEOEventEngine : EventEngine, IEventEngineChoice
{    
    public static new CEOEventEngine ceoengine;

    public string[] dbkey = {"No","EventType1","EventType2","EventType3","EventCondition_Money","EventCondition_StaffCount","EventCondition_Licence","EventCondition_Time","EventCondition_TimeInterval_Week","ReportHost","EventTracker","MailSubject","EventContents","GetMoney","SetMoney","ButtonActivation","ButtonAction1","ButtonAction2","ButtonAction3"};

    private void Awake()
    {
        if(ceoengine==null)
        {
            ceoengine = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public new void InitialEventContents(string[,] eventdatalist)
    {
        base.InitialEventContents(eventdatalist);
    }

    public string[,] EventGeneration(int ev_generation_decision_count, int eventno)
    {
        string[,]eventcontent = new string[ev_generation_decision_count,dbkey.Length];


        return eventcontent;
    }

    public void EngineChoice()
    {
        CEOEventEngine ec = new CEOEventEngine();
        iec = ec;
    }








}
