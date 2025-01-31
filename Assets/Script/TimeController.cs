using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : GameManager
{
    public static TimeController tc = null;
    public int lastMonth = 0; //신규 물질 찾는 함수 호출 조건식을 위해 만든 변수
    public int toDay_inGame;
    private DateTime gameStartTime = DateTime.Now; // 게임 시작 시간. 이벤트 발생 조건 요소
    private DateTime CurrentTime { get; set; } = DateTime.Now; //play시 게임내 현재의 가상시간으로 사용할 변수.
    public string GameTime { get; set; } // 위 CurrentTime을 문자열로 받아줄 변수.
    public float timescale = 1000.0f;


    private void Awake()
    {
        if (tc == null)
        {
            tc = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    private void Update()
    {
        CurrentTime = CurrentTime.AddSeconds(Time.deltaTime * timescale);
        if(CurrentTime.Day != toDay_inGame)
        {
            EventEngineManager.MFG_Order = 0;
        }
        else
        {
            toDay_inGame = CurrentTime.Day;
        }
        
    }

    void Start()
    {
        Debug.Log("TimeControll의 Start() call " + GameTime);
        lastMonth = DateTime.Now.Month;
    }

    public string TimeGeneration()
    {
        DateTime newtime = new(); // CurrentTime 보다 미래의 시간을 받아줄 변수 
        float a = UnityEngine.Random.Range(5,15);
        double random_value = (double)a;
        newtime = CurrentTime.AddMinutes(random_value); // 현재 게임내 가상시간에 랜덤하게 '분' 단위를 더해줌. 
        newtime = CompareToOldTimeandNewTime(newtime);
        Debug.Log("After compare, newtime vlaue = " + newtime);
        CurrentTime = newtime;
        Debug.Log("Changed currenttime = " + CurrentTime);
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
        else if(result == 0)
        {
            TimeGeneration();
        }
        else
        {
            TimeGeneration();
            Debug.Log("else statment called, new and new time vlaue : " + newtime);
        }

        return newtime;
    }

    public bool Compare_A_and_B_Time(DateTime time_t2)
    {
        int result = DateTime.Compare(CurrentTime, time_t2);
        if (result < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public DateTime DateTimeParseFromString(string datetime)
    {
        DateTime var = DateTime.Parse(datetime);
        return var;
    }

    public string DateTimeParseFromDateTime(DateTime dateTime)
    {
        string dateAndTime = CurrentTime.ToString();
        return dateAndTime;
    }

    public void SetCurrentTime(DateTime time)
    {
        CurrentTime = time;
    }

    public DateTime GetCurrentTime()
    {
        return CurrentTime;
    }

    public DateTime GetGameStartTime()
    {
        return gameStartTime;
    }


}
