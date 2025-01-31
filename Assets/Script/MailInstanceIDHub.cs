using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailInstanceIDHub : MonoBehaviour
{
    public int mailInstanceID { get; set; }
    public int mailContetnScriptID { get; set; }
    public int eventNo { get; set; }

    public string buttonaction_name_one { get; set; }
    public string buttonaction_name_two { get; set; }
    public string buttonaction_name_three { get; set; }

    public Dictionary<int, EmailButtonPrefab> mcsi; // 저장 할 때 가져와 저장해야 할 것.

    private void Awake()
    {
        Debug.Log("Awake() is called in MailInstanceIDHub");
        if (mcsi == null)
        {
            mcsi = new Dictionary<int, EmailButtonPrefab>();
        }
        Debug.Log("MailInstancIDHUB Scripit 준비 완료 했습니다 ");
    }

    public void mailInstanceIDPrint()
    {
        Debug.Log("mailInstanceID in mailInstanceIDHUB = " + mailInstanceID);
    }

    public void AddContentMailIDScript(int key, EmailButtonPrefab epb)
    {
        if (!mcsi.ContainsKey(key))
        {
            mcsi.Add(key, epb);
        }
        
    }
}
