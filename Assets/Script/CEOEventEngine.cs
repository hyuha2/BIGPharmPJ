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
    CultureInfo koreanCulture = new("ko-KR");

    private void Awake()
    {
        Debug.Log("Awake() called in CEOEventEngine");
        if(ceoengine==null)
        {
            ceoengine = this;
            ///DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
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
    
    public void SpecificMenu()
    {
        PanelController.PC.EnadbleGameObject("CEOSpecificPanel");
    }

    public long FixedEmployeeCost()
    {
        long empcost = 0;
        int[] hired_empno = new int[EmployeeManager.epm.hired_person.Count];
        for(int i =0; i<EmployeeManager.epm.hired_person.Count; i++)
        {
            hired_empno[i] = EmployeeManager.epm.hired_person[i];
        }
        for(int i=0; i<hired_empno.Length; i++)
        {
            string salary = EmployeeManager.epm.employees[hired_empno[i], 9];
            empcost += long.Parse(salary);
        }
        return empcost;
    }

    public void OnClickReportFinance()
    {
        long company_pure_money = PlayData.playData.GetCompany_Money(); //현재 자본 
        long company_loan_money = Bank.bank.AllLoanMoney(); // 총 대출 받은 금액
        long company_fixed_emp_money = FixedEmployeeCost(); //고정 인건비
        long invested_total_money = Investor.total_InverstedMoney; // 투자자 유치금
        string loan_money = string.Format(koreanCulture, "{0:N0}", company_loan_money);
        string pure_money = string.Format(koreanCulture, "{0:N0}", company_pure_money);
        string fixed_emp_money = string.Format(koreanCulture, "{0:N0}", company_fixed_emp_money);
        string total_invested_money = string.Format(koreanCulture, "{0:N0}", invested_total_money);
        //고정 자재비 필요

        string title = "회계보고 드립니다.";
        string msg = $"자본 : {pure_money}원\n" +
                     $"투자자 유치금 : {total_invested_money}원\n" +   
                     $"총 대출 금액 : {loan_money}원\n" +
                     $"고정비(인건비) : {fixed_emp_money}원\n";

        //int sender_emp_no = EmployeeManager.epm.EmailSender_Random("Finance");
        //string[] sender_information = EmployeeManager.epm.Employee_information(sender_emp_no);
        string sender = "Adminstrator_Finance";
        AddForSendingEmail(1002, sender, title, msg, "N/A", "N/A", "N/A");
    }

    public void BankLoan()
    {

    }
}
