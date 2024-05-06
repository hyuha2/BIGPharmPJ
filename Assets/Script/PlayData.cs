using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayData : MonoBehaviour
{
    public static PlayData playData;

    private string UserName { get; set; }
    private long Company_Money { get; set; }
    private int Employee_Total { get; set; }
    private List<int> HiredList { get; set; }
    private EmployeeManager EPM { get; set; }
    private OrderedDictionary MailBox = new OrderedDictionary();

    private Time GameCurrentTime { get; set; }

    private void Awake()
    {
        if (playData == null)
        {
            playData = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetOrderDictionary(int key, GameObject value_obj)
    {
        MailBox.Add(key, value_obj);
    }

    public void DelOrderDicionary(int key)
    {
        MailBox.Remove(key);
    }

    public void SetCompany_Money(long value)
    {
        Company_Money = value;
    }
}
