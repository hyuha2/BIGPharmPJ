using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 
using System;

public class CSVController : MonoBehaviour
{

    // Update is called once per frame
    public string[,] CSVRead(string dbfilename)
    {
        StreamReader sr = new StreamReader(Path.Combine(Application.dataPath, "Resource", dbfilename));
        string data = sr.ReadToEnd();
        string[] row = data.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
        string[] column = row[0].Split(',');
        string [,] datalist = new string[row.Length, column.Length];

        for(int i = 0; i < row.Length; i++)
        {
            string[] emptycolumn = row[i].Split(',');
            for(int j = 0; j < emptycolumn.Length; j++)
            {
                datalist[i, j] = emptycolumn[j];
            }
        }
        return datalist;   
    }
}
