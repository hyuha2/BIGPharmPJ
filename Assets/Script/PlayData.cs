using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayData : MonoBehaviour
{
    public static PlayData playData;

    private string UserName { get; set; }
    private long Company_Money { get; set; }
    private long Invest_Money { get; set; }
    public static List<Product> ProductInformation { get; set; } = new(); // 성분명, 브랜드명
    public static List<Product> Find_NewDrugMaterials { get; set; } = new(); // 찾은 신규 물질, 당장의 제품 개발에 돌입 할 수 없을 수 있기 때문에 후보물질에 대한 데이터 베이스는 확보해둬야 함.
    public static List<ProductDevelopProcess> newDrug_development_list = new(); // 개발 진행중인 물질들 
    //private int Employee_Total { get; set; }
    private List<int> HiredList { get; set; }
    private EmployeeManager EPM { get; set; }
    private List<Dictionary<int, GameObject>> MailBox = new List<Dictionary<int, GameObject>>();
    public List<EmailButtonPrefab> MailPrefabList { get; set; } = new();
    public List<string> approved_commercial_licence = new();

    private Time GameCurrentTime { get; set; }

    private void Awake()
    {
        Debug.Log("Awake() is called in PlayData");
        if (playData == null)
        {
            playData = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    public void SetMailBox(int key, GameObject value_obj)
    {
        Debug.Log("SETMAILBox Called. Key =" + key + ", GameObject =" + value_obj);
        Dictionary<int, GameObject> newmail = new();
        newmail[key] = value_obj;
        MailBox.Add(newmail);
        Debug.Log("List<Dictonary<int, GameObject> added");
        foreach (Dictionary<int, GameObject> dic in MailBox)
            foreach(var conf in dic)
                Debug.Log($"{conf.Key}, {conf.Value.name}");
    }

    public void DelMailBox(int key)
    {
        for(int i = MailBox.Count - 1; i>=0; i--)
        {
            if (MailBox[i].ContainsKey(key))
            {
                MailBox.RemoveAt(i);
            }
        }
    }

    public List<Dictionary<int, GameObject>> GetMailBox()
    {
        return MailBox;
    }

    public void SetCompany_Money(long value)
    {
        Company_Money = value;
    }

    public long GetCompany_Money()
    {
        return Company_Money;
    }

    public long GetInvest_Money()
    {
        return Invest_Money;
    }

    public int Get_DSdurationDay_inProductInformation(string mfg_brand_name)
    {
        int duration_day = 0;
        for (int i = 0; i < ProductInformation.Count; i++)
        {
            if (mfg_brand_name == ProductInformation[i].BrandName)
            {
                duration_day = ProductInformation[i].manufacture_ds_duration_day;
            }
        }
        return duration_day;
    }
}
