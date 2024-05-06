using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

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

    public void ForApproveLoanBank(int objID)
    {
        Debug.Log("은행 심사 준비중 입니다 .");
        InputField ipt = GameObject.Find("ipt_RequestMoney").GetComponent<InputField>();
        string input_money = ipt.text;
        ipt = GameObject.Find("ipt_RequestRate").GetComponent<InputField>();
        string input_rate = ipt.text;
        long request_money = long.Parse(input_money);
        float request_rate = float.Parse(input_rate);
        Dropdown drop = GameObject.Find("Dropdown_repayDate").GetComponent<Dropdown>();
        int repay_date = int.Parse(drop.options[drop.value].text);
        Debug.Log("값이 잘 들어왔는지 확인합니다 . " + request_money + "," + request_rate +"," +repay_date);
        Bank.bank.LoanApprovedProcessing(request_money, request_rate, repay_date);
    }








}
