using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.UI;

public class Bank : MonoBehaviour
{
    public static Bank bank;

    private long bank_pure_money { get; set; } = 99999999999999;//99조
    private long bank_debt { get; set; } = 0;
    public float bank_book_rate { get; set; } = 0.03f;
    private float bank_having_safety_rate { get; set; } = 0.1f;
    public float bank_having_safety_rate_once { get; set; } = 0.005f;

    public long request_money { get; set; }
    public float request_rate { get; set; }
    public DateTime request_repay_date { get; set; }

    public float approved_rate { get; set; }

    public List<(long loan_money, float loan_rate, DateTime repay_date, long loan_debt)> loan_history = new();

    CultureInfo koreanCulture = new("ko-KR");

    public void Awake()
    {
        Debug.Log("Awake() is called in Bank");
        if (bank == null)
        {
            bank = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    public long RandomMoney(long money)
    {
        float temp_rate = UnityEngine.Random.Range(bank_having_safety_rate_once / 10, bank_having_safety_rate_once *2);
        return (long)Math.Round(money * temp_rate);
    }

    public float RandomRate(int year)
    {
        float temp_rate = UnityEngine.Random.Range((GameManager.gm.GetInterest_Rate_Float() * (float)(5 / year)/5), GameManager.gm.GetInterest_Rate_Float());
        temp_rate += bank_book_rate;
        return temp_rate;
    }

    public void ForApproveLoanBank(int objID)
    {
        InputField ipt = GameObject.Find("ipt_RequestMoney").GetComponent<InputField>();
        string input_money = ipt.text;
        ipt = GameObject.Find("ipt_RequestRate").GetComponent<InputField>();
        string input_rate = ipt.text;
        long request_money = long.Parse(input_money);
        float request_rate = float.Parse(input_rate);
        Dropdown drop = GameObject.Find("Dropdown_repayDate").GetComponent<Dropdown>();
        int repay_date = int.Parse(drop.options[drop.value].text);
        Bank.bank.LoanApprovedProcessing(request_money, request_rate, repay_date);
    }

    public void LoanApprovedProcessing(long request_money, float request_rate, int request_repay_date)
    {
        Debug.Log("대출승인을 위한 프로세싱을 시작 합니다 . ");
        if(request_money <= (long)bank_pure_money * bank_having_safety_rate_once && bank_pure_money >= (long)bank_pure_money * bank_having_safety_rate)
        {
            Debug.Log("은행 잔고는 대출을 위해 적합합니다. 이자율 책정 하겠습니다.");
            if(request_rate >= GameManager.gm.GetInterest_Rate_Float())
            {
                approved_rate = GameManager.gm.GetInterest_Rate_Float() + bank_book_rate; 
            }
            else
            {
                approved_rate = RandomRate(request_repay_date);
            }
            string currentTime = TimeController.tc.GameTime;
            DateTime counting_repay_date = DateTime.Parse(currentTime);
            counting_repay_date = counting_repay_date.AddYears(request_repay_date);
            string repay_date = counting_repay_date.ToString("yyyy-MM-dd");
            PanelController.PC.DisadbleGameObject("ReqeustLoanOnBank");
            string message = "승인요청 되었습니다. 검토 후 연락 드리겠습니다.";
            EventEngineManager.em.one.ButtonCorrectiondProcessFromXtoNA();
            PanelController.PC.EnadbleGameObject("PopupPanel");
            PanelController.PC.GenerationPopupMessage(message);
            loan_history.Add((loan_money: request_money, loan_rate: approved_rate, repay_date: counting_repay_date, loan_debt: request_money));
            PlayData.playData.SetCompany_Money(request_money);
            AddForSendingEmail("대출 승인 되었습니다", string.Format(koreanCulture, "{0:N0}", request_money) + "원 / 이자율 " + approved_rate + "% 로 상환기간 " + request_repay_date +"년 으로 승인 되었습니다. 상환일은 " + repay_date + "일 입니다.", "N/A", "N/A", "N/A");
            try
            {
                if (GameObject.Find("txt_decision_action1").activeInHierarchy)
                {
                    GameObject.Find("txt_decision_action1").GetComponent<Text>().text = "N/A";
                }
            }
            catch
            {

            }
        }
        else
        {
            PanelController.PC.DisadbleGameObject("ReqeustLoanOnBank");
            double loan_max_once = bank_pure_money * bank_having_safety_rate_once;
            string msg = "대출 신청 기준에 부적합하여 신청 거절 당하였습니다. 1회 신청 가능 최대 금액은 현재 " + string.Format(koreanCulture, "{0:N0}", loan_max_once) + "원 입니다.";
            PanelController.PC.EnadbleGameObject("PopupPanel");
            PanelController.PC.GenerationPopupMessage(msg);
        }

    }
    public void AddForSendingEmail(string title, string contentmsg, string buttonaction1, string buttonaction2, string buttonaction3)
    {
        Dictionary<string, object> frombank = new();
        frombank.Add("No", 1001);
        frombank.Add("ReportHost", "Bank");
        frombank.Add("MailSubject", title);
        frombank.Add("EventContents", contentmsg);
        frombank.Add("ButtonAction1", buttonaction1);
        frombank.Add("ButtonAction2", buttonaction2);
        frombank.Add("ButtonAction3", buttonaction3);
        EventEngineManager.em.tmpeventlist.Add(frombank);
    }

    public long AllLoanMoney()
    {
        long total = 0;
        foreach (var a in loan_history)
        {
            total += a.loan_money;
        }
        return total;
    }

}
