using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailButtonPrefab : MonoBehaviour
{
    public GameObject mcp;
    public int prefabInstanceID;
    public int mailContentScriptID;
    public int no;
    public string txt_mail_title;
    public string txt_mail_receive_sender;
    public string txt_mail_receive_content;
    public string txt_mail_receive_time;
    public string buttonaction1;
    public string buttonaction2;
    public string buttonaction3;
    public string facepath;

    public GameObject btn_decision_action1;
    public GameObject btn_decision_action2;
    public GameObject btn_decision_action3;


    public void Awake()
    {
        Debug.Log("Awake() is called in EmailButtonPrefab");
        mcp = GetMCP(); //초기값 비활성화 상태이기 때문에 가져옴.
    }

    public void Start()
    {
        if(mcp!=null)
        {
            mcp.SetActive(false);
        }
        else
        {
            mcp = GetMCP();
            mcp.SetActive(false);
        }
    }


    public void ButtonPrefabOnClick()
    {
        if (mcp!=null)
        {
            mcp.SetActive(true);
        }
        else
        {
            mcp = GetMCP();
            mcp.SetActive(true);
        }
        btn_decision_action1 = GameObject.Find("btn_decision_action1");
        btn_decision_action2 = GameObject.Find("btn_decision_action2");
        btn_decision_action3 = GameObject.Find("btn_decision_action3");
        SetMailProperty();
    }

    public GameObject GetMCP()
    {
        Debug.Log("GET MCSV 호출");
        GameObject pcanvas = GameObject.Find("Canvas");
        Transform mcsvtr = pcanvas.transform.Find("Mail_Content_Panel");
        Transform[] childObject = mcsvtr.GetComponentsInChildren<Transform>();
        foreach(Transform pr in childObject)
            if(pr.gameObject.name == "Mail_Content_Panel")
            {
                mcp = pr.gameObject;
            }
        return mcp;
    }

    public void SetMailProperty() // 
    {
        Text title = GameObject.Find("txt_mail_receive_title").GetComponent<Text>();
        Text content = GameObject.Find("txt_mail_receive_content").GetComponent<Text>();
        Text sender = GameObject.Find("txt_mail_receive_sender").GetComponent<Text>();
        Text time = GameObject.Find("txt_mail_receive_time").GetComponent<Text>();
        title.text = txt_mail_title;
        content.text = txt_mail_receive_content;
        sender.text = txt_mail_receive_sender;
        time.text = txt_mail_receive_time;

        if (buttonaction1.Contains("N/A"))
        {
            if(btn_decision_action1 != null)
            {
                btn_decision_action1.SetActive(false);
            }
        }
        else
        {
            DisableGameObjectChangeEnable("Mail_Content_Panel", "btn_decision_action1");
            try
            {
                Text txt = btn_decision_action1.GetComponentInChildren<Text>();
                txt.text = buttonaction1;
            }
            catch (NullReferenceException e)
            {
                btn_decision_action1 = GameObject.Find("btn_decision_action1");
                Text txt = btn_decision_action1.GetComponentInChildren<Text>();
                txt.text = buttonaction1;
            }
            btn_decision_action1.SetActive(true);
            btn_decision_action1.GetComponent<Button>().interactable = true;
        }

        if (buttonaction2.Contains("N/A"))
        {
            if (btn_decision_action2 != null)
            {
                btn_decision_action2.SetActive(false);
            }
        }
        else
        {
            DisableGameObjectChangeEnable("Mail_Content_Panel", "btn_decision_action2");
            try
            {
                Text txt = btn_decision_action2.GetComponentInChildren<Text>();
                txt.text = buttonaction2;
            }
            catch (NullReferenceException e)
            {
                btn_decision_action2 = GameObject.Find("btn_decision_action2");
                Text txt = btn_decision_action2.GetComponentInChildren<Text>();
                txt.text = buttonaction2;
            }
            btn_decision_action2.GetComponent<Button>().interactable = true;

        }

        if (buttonaction3.Contains("N/A") || buttonaction3.Contains("N/A\n"))
        {
            if (btn_decision_action3 != null)
            {
                btn_decision_action3.SetActive(false);
            }
        }
        else
        {
            DisableGameObjectChangeEnable("Mail_Content_Panel", "btn_decision_action3");
            try
            {
                Text txt = btn_decision_action3.GetComponentInChildren<Text>();
                txt.text = buttonaction3;
            }
            catch (NullReferenceException e)
            {
                btn_decision_action3 = GameObject.Find("btn_decision_action3");
                Text txt = btn_decision_action3.GetComponentInChildren<Text>();
                txt.text = buttonaction3;
            }
            btn_decision_action3.GetComponent<Button>().interactable = true;
        }
        mailContentScriptID = gameObject.GetInstanceID();
        MailInstanceIDHub instanceIDHub = GameObject.Find("Mail_Content_Panel").GetComponent<MailInstanceIDHub>();
        instanceIDHub.mailInstanceID = prefabInstanceID;
        instanceIDHub.mailContetnScriptID = mailContentScriptID;
        instanceIDHub.eventNo = no;
        instanceIDHub.buttonaction_name_one = buttonaction1;
        instanceIDHub.buttonaction_name_two = buttonaction2;
        instanceIDHub.buttonaction_name_three = buttonaction3;
        Debug.Log("prfabInstaceID of SetMailProperty = " + prefabInstanceID);
        Debug.Log("mailContentScriptID of SetMailProperty = " + mailContentScriptID);
        instanceIDHub.mailInstanceIDPrint();
        instanceIDHub.AddContentMailIDScript(mailContentScriptID, this);
    }

    public void DisableGameObjectChangeEnable(string gameObjectParentStrName, string enableGameObjectName)
    {
        Transform can = GameObject.Find(gameObjectParentStrName).transform;
        Debug.Log(can.name);
        Transform tr = can.Find(enableGameObjectName).transform;
        tr.gameObject.SetActive(true);
    }
}
