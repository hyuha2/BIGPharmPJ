using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventConfirmPanel : MonoBehaviour //예, 아니오 팝업창에 대한 클래스. 
{
    public Button btn_answer1 { get; set; }
    public Button btn_answer2 { get; set; }

    public Text txt_confirmMessage { get; set; }
    public Text txt_answer1 { get; set; }
    public Text txt_answer2 { get; set; }

    public int prefabInstanceID { get; set; }

    public void SetConfirmMessage(string message)
    {
        txt_confirmMessage = GameObject.Find("txt_confirmMessage").GetComponent<Text>();
        txt_confirmMessage.text = message;  
    }

    public void SetButton(string left, string right)
    {
        txt_answer1 = GameObject.Find("txt_answer1").GetComponent<Text>();
        txt_answer1.text = left;
        txt_answer2 = GameObject.Find("txt_answer2").GetComponent<Text>();
        txt_answer2.text = right;
    }

    public void DeinteractableButtoninMCPPanel()
    {
        Button[] buttoninmcp = GameObject.Find("Mail_Content_Panel").GetComponentsInChildren<Button>();
        foreach (Button btn in buttoninmcp)
        {
            btn.interactable = false;
        }
    }

    public void InteractableButtoninMCPPanel()
    {
        Button[] buttoninmcp = GameObject.Find("Mail_Content_Panel").GetComponentsInChildren<Button>();
        foreach (Button btn in buttoninmcp)
        {
            btn.interactable = true;
        }
    }

    public void InteractableButtonCEOSpecificPanel()
    {
        Button[] buttoninmcp = GameObject.Find("CEOSpecificPanel").GetComponentsInChildren<Button>();
        foreach (Button btn in buttoninmcp)
        {
            btn.interactable = true;
        }
    }

    public void OnClickRight()
    {
        gameObject.SetActive(false);
        InteractableButtoninMCPPanel();
    }

    public void OnLeftClick()
    {
        if (txt_confirmMessage.text.Contains("삭제"))
        {
            InteractableButtoninMCPPanel();
            EventEngineManager em = GameObject.Find("EventEngineManager").GetComponent<EventEngineManager>();
            em.OnClickDelEmailFromConfirmPanel();
        }
    }
}
