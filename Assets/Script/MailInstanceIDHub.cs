using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailInstanceIDHub : MonoBehaviour
{
    public int mailInstanceID { get; set; }
    public int mailContetnScriptID { get; set; }

    public Dictionary<int, EmailButtonPrefab> mcsi;

    private void Awake()
    {
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
