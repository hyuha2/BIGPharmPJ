using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Department {OwnerSecretary,CEOSecretary,RD,Manufacture,QC,QA,Engineering,Adminstrator,Sales,MaterialsManagement,Marketing,RA}
public enum AdminstratorTeam {HR,IR,Finance,Purchase}

public class EmployeeManager : MonoBehaviour
{

    public EmployeeManager epm;
    public string [,] employees;
    public List<int> hired_person = new List<int>();

    
    void Awake()
    {
        if(epm == null)
        {
            epm = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (employees == null)
        {
            CSVController csv = GameObject.Find("CSVController").GetComponent<CSVController>();
            employees = csv.CSVRead("Employee_list.csv");

        }

        if (GameManager.gm.GetNewGame())
        {
            Debug.Log("직원추가 대");
            ArrangeDepartment(1, Department.OwnerSecretary.ToString(), UnityEngine.Random.Range(22000000, 50000000));
            ArrangeDepartment(5, Department.RD.ToString(), UnityEngine.Random.Range(22000000, 50000000));
            ArrangeDepartment(8, Department.Manufacture.ToString(), UnityEngine.Random.Range(22000000, 50000000));
            ArrangeDepartment(6, Department.QC.ToString(), UnityEngine.Random.Range(22000000, 50000000));
            ArrangeDepartment(4, Department.QA.ToString(), UnityEngine.Random.Range(22000000, 50000000));
            ArrangeDepartment(1, Department.Adminstrator.ToString() + "_" + AdminstratorTeam.HR.ToString(), UnityEngine.Random.Range(22000000, 50000000));
            ArrangeDepartment(1, Department.Adminstrator.ToString() + "_" + AdminstratorTeam.IR.ToString(), UnityEngine.Random.Range(22000000, 50000000));
            ArrangeDepartment(1, Department.Adminstrator.ToString() + "_" + AdminstratorTeam.Finance.ToString(), UnityEngine.Random.Range(22000000, 50000000));
            ArrangeDepartment(1, Department.Adminstrator.ToString() + "_" + AdminstratorTeam.Purchase.ToString(), UnityEngine.Random.Range(22000000, 50000000));
            ArrangeDepartment(1, Department.CEOSecretary.ToString(), UnityEngine.Random.Range(22000000, 50000000));
        }
    }

    void Start()
    {

    }

    public void ArrangeDepartment(int assign_personal, string department, int salary)
    {
        for(int i =0; i < assign_personal; i++)
        {
            int randomEmpNo = UnityEngine.Random.Range(1, employees.GetLength(0)+1);
            if(employees[randomEmpNo, 11].Contains("FALSE"))
            {
                Debug.Log("직원번호 " + randomEmpNo + "가 고용되지 않았음을 확인하여 지금 부터 변경 합니다 ");
                employees[randomEmpNo, 11] = "true";
                employees[randomEmpNo, 7] = department;
                employees[randomEmpNo, 9] = salary.ToString();
                hired_person.Add(randomEmpNo);
                Debug.Log("추가되었습니다. 현재 고용 인원은 " + hired_person.Count + "명 입니다 .");
            }
            else
            {
                i -= 1;
                Debug.Log(randomEmpNo + " 는 채용 된 직원 입니다 . 확인 바랍니다 . ");
            }

        }
    }

    public List<int> FindEmployee_department(string department)
    {
        List<int> findEmpNo = new List<int>();
        for(int i =0; i<hired_person.Count; i++)
        {
            int no = hired_person[i];
            Debug.Log("직원번호 " + no + "에 대해" + department +"소속인지 조회합니다 ");
            if(employees[no, 7] == department)
            {
                findEmpNo.Add(hired_person[i]);
                Debug.Log("직원번호 " + hired_person[i] + "는 소속부서가 " + department + "입니다 .");
            }
        }
        return findEmpNo;
    }

    public int EmailSender_Random(string reporthost) // remain mail 로직 확인 . 메일은 1개가 와있지만 , 리스트에는 2개가 있어서 이미 로직이 돌아간 상태 .
    {
        Debug.Log("report host = " + reporthost);
        int findEmpNo = 0;
        switch (reporthost)
        {
            case "Bank":
                findEmpNo = 1001;
                break;

            default:
                List<int> SearchEmpNo = new List<int>();
                SearchEmpNo = FindEmployee_department(reporthost);
                Debug.Log("SearchEMpNo = " + SearchEmpNo.Count);
                int random_index = UnityEngine.Random.Range(0, SearchEmpNo.Count + 1);
                try
                {
                    findEmpNo = SearchEmpNo[random_index];
                }
                catch (ArgumentOutOfRangeException e)
                {
                    random_index = random_index - 1;
                    try
                    {
                        findEmpNo = SearchEmpNo[random_index];
                    }
                    catch (ArgumentOutOfRangeException e_one)
                    {
                        findEmpNo = 0;
                    }
                }
                Debug.Log("배열에 대한 Random index no = " + random_index);
                break;
        }        
        return findEmpNo;
    }

    public string[] Employee_information(int empNo)
    {
        string[] employee_info = new string[employees.GetLength(1)];
        switch (empNo)
        {
            case 1001:
                employee_info = new string[]{"1001", "Bank", "Male", "KOR", "39", "default", "null", "null", "null", "0", "100", "TRUE", "null"};
                break;

            default:
                for (int i = 0; i < employees.GetLength(1); i++)
                {
                    employee_info.SetValue(employees[empNo + 1, i], i);
                }
                break;
        }
        return employee_info;
    }




}
