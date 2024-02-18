using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailButtonPrefab : MonoBehaviour
{
    public GameObject mcsv;
    public GameObject msv;
    public GameObject msvInstanceCopy;
    public SceneController sc;

    public void Awake()
    {
        mcsv = GameObject.Find("Canvas/Mail_Content_Scroll View");
        msv = GameObject.Find("Canvas/Mail_Scroll View");
        Debug.Log("EmailPrefab_Awake_mcsv =" + mcsv);
        Debug.Log("EmailPrefab_Awake_msv =" + msv);
    }

    public void Start()
    {
        if(mcsv!=null)
        {
            mcsv.SetActive(false);
        }
        else
        {
            mcsv = GetMCSV();
        }
    }


    public void ButtonPrefabOnClick()
    {
        if(msv!=null)
        {
            msv.SetActive(false);
        }
        else
        {
            msv = GetMSV();
            msv.SetActive(false);
        }

        if(mcsv!=null)
        {
            mcsv.SetActive(true);
        }
        else
        {
            mcsv = GetMCSV();
            mcsv.SetActive(true);
        }
    }

    public GameObject GetMCSV()
    {
        GameObject parentCanvas = GameObject.Find("Canvas").gameObject;
        Transform[] childObject = parentCanvas.GetComponentsInChildren<Transform>();
        foreach(Transform pr in childObject)
            if(pr.gameObject.name == "Mail_Content_Scroll View")
            {
                mcsv = pr.gameObject;
            }
        //mcsv = tmcsv.gameObject;
        return mcsv;
    }

    public GameObject GetMSV()
    {
        GameObject parentCanvas = GameObject.Find("Canvas").gameObject;
        Transform[] childObject = parentCanvas.GetComponentsInChildren<Transform>();
        foreach (Transform pr in childObject)
            if (pr.gameObject.name == "Mail_Scroll View")
            {
                msv = pr.gameObject;
            }
        //mcsv = tmcsv.gameObject;
        return msv;
    }

    public void ButtonCloseOnClick()
    {
        mcsv.SetActive(false);
        msv.SetActive(true);
    }


}
