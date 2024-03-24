using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventControllPanel : MonoBehaviour
{
    public Button btn_answer1 { get; set; }
    public Button btn_answer2 { get; set; }

    public Text txt_confirmMessage { get; set; }
    public Text txt_answer1 { get; set; }
    public Text txt_answer2 { get; set; }

    public string confirmMessage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetConfirmMessage(string message)
    {
        txt_confirmMessage.text = message;
    }
}
