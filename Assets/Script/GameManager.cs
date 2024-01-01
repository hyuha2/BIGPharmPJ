using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static string username;
    private static string time;
    private static bool newgame = false;
    public static GameManager gm = null;
    private void Awake()
    {
        if(gm == null)
        {
            gm = this;
        }
    }

    void Start()
    {        
        if(newgame)
        {
            GenerationTime();
        }    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public void SetInputUserName(string name)
    {
        username = name;
        Debug.Log(username);
    }

    public void SetInputNewGame(string btnname)
    {
        newgame = btnname.Equals("btn_nexttoselectmode"); // 새 게임 시작시 최초 시작시에만 만나게되는 btn_nexttoselectmode 버튼의 버튼 이름이 맞는 경우 bool newgame에 true를 넘기므로 신규게임인지 아닌지 필터링 역할.
        Debug.Log("is New Game???" + newgame);
    }

    public bool GetNewGame(){
        return newgame;
    }

    void GenerationTime()
    {
        DateTime crurrentDate = DateTime.Now;
        time = crurrentDate.ToString();
        Debug.Log("시간은? : " + time);

    }
}