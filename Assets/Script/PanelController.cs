using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{

    public static PanelController PC;
    public int tempObjId;

    private void Awake()
    {
        if (PC == null)
        {
            PC = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnadbleGameObject(string enableGameObjectName)
    {
        Transform ptr = GameObject.Find("Canvas").transform;
        Transform tr = ptr.Find(enableGameObjectName).transform;
        tr.gameObject.SetActive(true);
    }

    public void DisadbleGameObject(string disableGameObjectName)
    {
        GameObject.Find(disableGameObjectName).SetActive(false);
    }

    public void InteratableMCPPanel()
    {
        Button[] buttoninmcp = GameObject.Find("Mail_Content_Panel").GetComponentsInChildren<Button>();
        foreach (Button btn in buttoninmcp)
        {
            btn.interactable = true;
        }
    }

    public void DeInteratablePanel(string panel_name)
    {
        Button[] buttoninmcp = GameObject.Find(panel_name).GetComponentsInChildren<Button>();
        foreach (Button btn in buttoninmcp)
        {
            btn.interactable = false;
        }
    }

    public void OnclickMenuButton()
    {
        GameObject menu;
        try
        {
            menu = GameObject.Find("Menu");
            if (menu == null)
            {
                EnadbleGameObject("Menu");
            }
            else
            {
                DisadbleGameObject("Menu");
            }
        }
        catch (NullReferenceException e)
        {
            //비활성화 상태
        }       
    }

    public void UserInputPanel(string text)
    {

    }

    public void LoanBankPanel(int objID)
    {
        EnadbleGameObject("ReqeustLoanOnBank");
        Text[] texts = GameObject.Find("ReqeustLoanOnBank").GetComponentsInChildren<Text>();
        foreach(Text text in texts)
        {
            switch (text.name)
            {
                case "txt_CurrentRate":
                    string rate = GameManager.gm.GetInterest_Rate();
                    text.text = "현재 금리는 " + rate + "% 입니다.";
                    break;

                case "txt_CurrentDate":
                    string date = TimeController.tc.GameTime;
                    text.text = "현재 시간은 " + date + " 입니다.";
                    break;
            }
        }
        tempObjId = objID;
    }

    public void OnClickLoanBankCancelButton()
    {
        DisadbleGameObject("ReqeustLoanOnBank");
        EventConfirmPanel ecp = new EventConfirmPanel();
        ecp.InteractableButtoninMCPPanel();
    }

    public void OnClickLoanBankApproveButton()
    {
        EventEngineManager.em.SendMessage("ForApproveLoanBank", tempObjId, SendMessageOptions.RequireReceiver);
    }

    public void GenerationPopupMessage(string msg)
    {
        Text text = GameObject.Find("txt_popupMessage").GetComponent<Text>();
        text.text = msg;
    }

}
