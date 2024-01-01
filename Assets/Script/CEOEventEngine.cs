using System;
using System.Collections.Generic;
using UnityEngine;

public enum DBcolumnNo {No,EventType1,EventType2,EventType3,EventCondition_Money,EventCondition_StaffCount,EventCondition_Licence,EventCondition_Time,EventCondition_TimeInterval_Week,EventOccuredHost,ReportHost,EventTracker,MailSubject,EventContents,GetMoney,SetMoney,ButtonActivation,ButtonAction1,ButtonAction2,ButtonAction3};
public class CEOEventEngine : EventEngine
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



    

    
         
}
