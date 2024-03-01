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

    public void SetMailProperty()
    {
        Text title = GameObject.Find("txt_mail_receive_title").GetComponent<Text>();
        Text content = GameObject.Find("txt_mail_receive_content").GetComponent<Text>();
        title.text = txt_mail_title;
        content.text = txt_mail_receive_content;
    }


}
