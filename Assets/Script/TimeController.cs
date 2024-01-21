using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : GameManager
{
    public static TimeController tc = null;
    private DateTime CurrentTime { get; set; } = DateTime.Now; //play시 게임내 현재의 가상시간으로 사용할 변수.
    public string GameTime { get; set; } // 위 CurrentTime을 문자열로 받아줄 변수.

    private void Awake()
    {
        if (tc == null)
        {
            tc = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("TimeControll의 Start() call " + GameTime);
    }

    public string TimeGeneration()
    {
        DateTime newtime = new(); // CurrentTime 보다 미래의 시간을 받아줄 변수 
        float a = UnityEngine.Random.Range(1,10);
        double random_value = (double)a;
        newtime = CurrentTime.AddMinutes(random_value); // 현재 게임내 가상시간에 랜덤하게 '분' 단위를 더해줌. 
        Debug.Log("Newtime of TimeGeneration = " + newtime); //1450
        Debug.Log("CurrentTime of Filed = " + CurrentTime); // 1449
        newtime = CompareToOldTimeandNewTime(newtime);
        Debug.Log("After compare, newtime vlaue = " + newtime);
        CurrentTime = newtime;
        Debug.Log("Currenttime = newtime : " + CurrentTime);
        GameTime = CurrentTime.ToString();
        return GameTime;
    }

    public DateTime CompareToOldTimeandNewTime(DateTime newtime)
    {
        int result = DateTime.Compare(CurrentTime, newtime);
        if(result < 0)
        {
            Debug.Log("if called, new time later  than current time");
            return newtime;
        }
        else
        {
            //newtime = newtime.AddMinutes((double)UnityEngine.Random.Range(1,10));
            TimeGeneration();
            Debug.Log("else statment called, new and new time vlaue : " + newtime);

        }

        return newtime;
    }


}
