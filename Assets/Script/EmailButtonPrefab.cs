using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailButtonPrefab : MonoBehaviour
{
    public GameObject mcp;
    public string txt_mail_title;
    public string txt_mail_receive_sender;
    public string txt_mail_receive_content;
    public string txt_mail_receive_time;
    public string buttonaction1;
    public string buttonaction2;
    public string buttonaction3;

    public GameObject btn_decision_action1;
    public GameObject btn_decision_action2;
    public GameObject btn_decision_action3;


    public void Awake()
    {
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
        if(mcp!=null)
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
        title.text = txt_mail_title;
        content.text = txt_mail_receive_content;
        Debug.Log(buttonaction1 + buttonaction2 + buttonaction3);

        if (buttonaction1.Contains("N/A"))
        {
            try
            {
                btn_decision_action1.SetActive(false);
            }
            catch
            {
                return;
            }
        }
        else
        {
            btn_decision_action1 = DisableGameObjectChangeEnable("Mail_Content_Panel", "btn_decision_action1");
            Text txt = btn_decision_action1.GetComponentInChildren<Text>();
            txt.text = buttonaction1;
            btn_decision_action1.SetActive(true);
        }

        if (buttonaction2.Contains("N/A"))
        {
            try
            {
                btn_decision_action2.SetActive(false);
            }
            catch
            {
                return;
            }
        }
        else
        {
            btn_decision_action2 = DisableGameObjectChangeEnable("Mail_Content_Panel", "btn_decision_action2");
            Text txt = btn_decision_action2.GetComponentInChildren<Text>();
            txt.text = buttonaction2;
            btn_decision_action2.SetActive(true);
        }

        if (buttonaction3.Contains("N/A") || buttonaction3.Contains("N/A\n"))
        {
            try
            {
                btn_decision_action3.SetActive(false);
            }
            catch
            {
                return;
            }
        }
        else
        {
            btn_decision_action3 = DisableGameObjectChangeEnable("Mail_Content_Panel", "btn_decision_action3");
            Text txt = btn_decision_action3.GetComponentInChildren<Text>();
            txt.text = buttonaction3;
            btn_decision_action3.SetActive(true);
        }
    }

    public GameObject DisableGameObjectChangeEnable(string gameObjectParentStrName, string enableGameObjectName)
    {
        GameObject obj = new GameObject();
        Transform can = GameObject.Find(gameObjectParentStrName).transform;
        Debug.Log(can.name);
        Transform[] tr = can.Find(enableGameObjectName).GetComponentsInChildren<Transform>();
        foreach(Transform enableGameObject in tr)
        {
            Debug.Log(enableGameObject.gameObject.name + "=" + enableGameObjectName);
            if (enableGameObject.gameObject.name == enableGameObjectName)
            {
                obj = enableGameObject.gameObject;
            }
        }
        return obj;
    }

}
