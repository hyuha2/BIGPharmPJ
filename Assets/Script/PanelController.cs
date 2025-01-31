using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Globalization;

public class PanelController : MonoBehaviour
{

    public static PanelController PC;
    public int tempObjId;
    public int requestCount_from_SpecificMenu = 0;
    public bool invest_fix = false;
    public string[,] development;

    public GameObject fileGoButtonPrefab;
    public GameObject empInforButtonPrefab;
    public Transform contentInScrollView;
    public Dropdown dropdown;
    public Dropdown brandName;
    public Dropdown mfg_campaign;
    public InputField batch_wo_count;

    CultureInfo koreanCulture = new("ko-KR");
    CSVController csvcr;

    private void Awake()
    {
        Debug.Log("Awake() is called in PanelController");
        if (PC == null)
        {
            PC = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
        csvcr = GameObject.Find("CSVController").GetComponent<CSVController>();
    }

    private void Start()
    {
        dropdown.onValueChanged.AddListener(delegate { Orz_DropdownItemSelected(dropdown); });
    }

    public void EnadbleGameObject(string enableGameObjectName)
    {
        Transform ptr = GameObject.Find("Canvas").transform;
        Transform tr = ptr.Find(enableGameObjectName).transform;
        tr.gameObject.SetActive(true);
    }

    public void DisadbleGameObject(string disableGameObjectName)
    {
        GameObject.Find(disableGameObjectName).SetActive(false);
    }

    public void InteratableMCPPanel()
    {
        try
        {
            Button[] buttoninmcp = GameObject.Find("Mail_Content_Panel").GetComponentsInChildren<Button>();
            foreach (Button btn in buttoninmcp)
            {
                Text txt = GameObject.Find(btn.name).GetComponentInChildren<Text>();
                if(txt.text == "N/A")
                {
                    btn.interactable = false;
                }
                else
                {
                    btn.interactable = true;
                }
            }
        }
        catch(NullReferenceException e)
        {
            GameObject canvas = GameObject.Find("Canvas");
            List<GameObject> activeObj = new List<GameObject>();
            List<string> obj_name = new();
            foreach (Transform i in canvas.transform)
            {
                if (i.gameObject.name != "inputf_searchfiled" && i.name != "btn_search" && i.name != "btn_continue" && i.name != "btn_menubar" && i.name != "Mail_Scroll View")
                {
                    if (i.gameObject.activeInHierarchy)
                    {
                        obj_name.Add(i.gameObject.name);
                    }
                }
            }
            string interable_obj_name;
            if(obj_name.Count == 1)
            {
                interable_obj_name = obj_name[0];
                Button[] buttoninmcp = GameObject.Find(interable_obj_name).GetComponentsInChildren<Button>();
                foreach (Button btn in buttoninmcp)
                {
                    btn.interactable = true;
                }
            }
            else
            {
                Debug.Log($"obj_name의 개수는 {obj_name.Count} 입니다 .");
            }

        }
    }

    public void DeInteratablePanel(string panel_name)
    {
        Button[] buttoninmcp = GameObject.Find(panel_name).GetComponentsInChildren<Button>();
        foreach (Button btn in buttoninmcp)
        {
            if(btn.name != "btn_close" || btn.name != "btn_delete")
            {
                btn.interactable = false;
            }
            else
            {
                btn.interactable = true;
            }    
        }
    }

    public void OnclickMenuButton()
    {
        GameObject menu;
        try
        {
            menu = GameObject.Find("Menu");
            if (menu == null)
            {
                EnadbleGameObject("Menu");
            }
            else
            {
                DisadbleGameObject("Menu");
            }
        }
        catch (NullReferenceException e)
        {
            //비활성화 상태
        }       
    }

    public void GenerationPopupMessage(string msg)
    {
        try
        {
            Text text = GameObject.Find("txt_popupMessage").GetComponent<Text>();
            text.text = msg;
        }
        catch (NullReferenceException e)
        {
            EnadbleGameObject("PopupPanel");
            Text text = GameObject.Find("txt_popupMessage").GetComponent<Text>();
            text.text = msg;
        }
    }

    public void OnClickSpecificMenu()
    {
        EventEngineManager.iec.SpecificMenu();
        DisadbleGameObject("Menu");
    }

    //저장 및 불러오기
    public void OnClickSaveButton()
    {
        EnadbleGameObject("SavePanel");
    }

    public void OnClickMainButton()
    {
        SceneController.sc.OnMainScene();
        if(GameManager.gm.CurrentGameDataFile != null)
        {
            GameManager.gm.CurrentGameDataFile = null;
        }
    }

    public List<string> SaveFileList()
    {
        List<string> saveFileList = new List<string>();

        try
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.xml", SearchOption.AllDirectories);
            saveFileList.AddRange(files);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return saveFileList;
    }


    //CEO Mode 관련 판넬
    public void OnClickRequestFinanceReport()
    {
        CEOEventEngine.ceoengine.OnClickReportFinance();
        DeInteratablePanel("CEOSpecificPanel");
        GenerationPopupMessage("유관부서에 전자 보고 요청 하였습니다.");

    }

    public void BankLoan()
    {
        DeInteratablePanel("CEOSpecificPanel");
        LoanBankPanel(0);
        InteratableMCPPanel();
    }

    public void InvestorCalled()
    {
        if (requestCount_from_SpecificMenu == 0)
        {
            GenerationPopupMessage("유관부서에 투자자 모집 요청 하겠습니다");
            Investor.investor.ResultInvestorCalled();
            requestCount_from_SpecificMenu += 1;
            StartCoroutine(RequestInvestDelay(1000));

        }
        else
        {
            GenerationPopupMessage("최근 투자유치가 진행되어 불가합니다.");
        }

    }

    public void FixedInvest()
    {
        if (invest_fix == false)
        {
            Investor.investor.FixedInvest();
            GenerationPopupMessage("투자금이 입금 되었습니다.");
            if (GameObject.Find("txt_decision_action1").activeInHierarchy)
            {
                GameObject.Find("txt_decision_action1").GetComponent<Text>().text = "N/A";
            }
            if (GameObject.Find("txt_decision_action2").activeInHierarchy)
            {
                GameObject.Find("txt_decision_action2").GetComponent<Text>().text = "N/A";
            }
            if (GameObject.Find("txt_decision_action3").activeInHierarchy)
            {
                GameObject.Find("txt_decision_action3").GetComponent<Text>().text = "N/A";
            }
        }
        else
        {
            GenerationPopupMessage("투자 확정 하셨습니다.");
        }

    }

    IEnumerator RequestInvestDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        requestCount_from_SpecificMenu = 0;
    }

    public void LoanBankPanel(int objID)
    {
        EnadbleGameObject("ReqeustLoanOnBank");
        Text[] texts = GameObject.Find("ReqeustLoanOnBank").GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            switch (text.name)
            {
                case "txt_CurrentRate":
                    string rate = GameManager.gm.GetInterest_Rate();
                    text.text = "현재 금리는 " + rate + "% 입니다.";
                    break;

                case "txt_CurrentDate":
                    string date = TimeController.tc.GameTime;
                    text.text = "현재 시간은 " + date + " 입니다.";
                    break;
            }
        }
        tempObjId = objID;
    }

    public void OnClickLoanBankCancelButton()
    {
        DisadbleGameObject("ReqeustLoanOnBank");
        EventConfirmPanel ecp = new EventConfirmPanel();
        try
        {
            if (GameObject.Find("CEOSpecificPanel").activeInHierarchy)
            {
                ecp.InteractableButtonCEOSpecificPanel();
            }
            else
            {
                ecp.InteractableButtoninMCPPanel();
            }
        }
        catch(NullReferenceException e)
        {
            ecp.InteractableButtoninMCPPanel();
        }
    }

    public void OnClickLoanBankApproveButton()
    {
        Bank.bank.SendMessage("ForApproveLoanBank", tempObjId, SendMessageOptions.RequireReceiver);
    }

    //save Pannel
    public void OnClickSaveLogic()
    {
        GameManager.gm.Save();
    }

    public void OnClickSaveAsLogic()
    {
        GameManager.gm.SaveAs();
    }

    //직원 관련 판넬
    public void OnClickRequestEmployeelist()
    {
        List<string> org_list = new List<string>();
        org_list.Add("전체");
        foreach (var a in Enum.GetValues(typeof(Department)))
        {
            org_list.Add(a.ToString());
        }
            
        foreach (var a in Enum.GetValues(typeof(AdminstratorTeam)))
        {
            org_list.Add(a.ToString());
        }

        EnadbleGameObject("EmployeeListSummaryPanel");
        RemoveInScrollView("Employee_Content");
        Dropdown org_Category = GameObject.Find("dropdown_organizationCategory").GetComponent<Dropdown>();
        org_Category.options.Clear();
        foreach (string item in org_list)
        {
            Dropdown.OptionData newOption = new Dropdown.OptionData(item);
            org_Category.options.Add(newOption);
        }
        org_Category.value = 0;
        List<string[]> employeeInfo = GetEmployeeInformation();
        PreparationEmpInforButton(employeeInfo);
    }

    public List<string[]> GetEmployeeInformation()
    {
        List<string[]> employee_infor = new();
        for(int i =0; i < EmployeeManager.epm.hired_person.Count; i++)
        {
            string[] employee = new string[EmployeeManager.epm.employees.GetLength(1)];
            employee = EmployeeManager.epm.Employee_information(EmployeeManager.epm.hired_person[i]);
            employee_infor.Add(employee);
        }
        return employee_infor;
    }

    public List<string[]> GetEmployeeInformation(string dep)
    {
        List<string[]> employee_infor = new();
        for (int i = 0; i < EmployeeManager.epm.hired_person.Count; i++)
        {
            string[] employee = new string[EmployeeManager.epm.employees.GetLength(1)];
            employee = EmployeeManager.epm.Employee_information(EmployeeManager.epm.hired_person[i]);
            if(employee[7] == dep)
            {
                employee_infor.Add(employee);
            }
        }
        return employee_infor;
    }

    public void PreparationEmpInforButton(List<string[]> emp)
    {
        int i = 0;
        foreach(var a in emp)
        {
            string name = a[1];
            string team = a[6] + " / " + a[7];
            string salary = a[9];
            string ability = a[10];
            string facePath = a[12];

            StartCoroutine(GenerationEmpInfoButton(i, name, team, salary, ability, facePath));
            
            i++;
        }
    }

    public IEnumerator GenerationEmpInfoButton(int seq_no, string emp_name, string emp_team, string emp_salary, string emp_ability, string emp_facePath)
    {
        GameObject obj = Instantiate(empInforButtonPrefab);
        Text no = obj.transform.Find("txt_emp_no").GetComponent<Text>();
        Text name = obj.transform.Find("txt_name").GetComponent<Text>();
        Text team = obj.transform.Find("txt_team").GetComponent<Text>();
        Text salary = obj.transform.Find("txt_salary").GetComponent<Text>();
        Text ability = obj.transform.Find("txt_ability").GetComponent<Text>();
        Image profile = obj.transform.Find("Img_profileImg").GetComponent<Image>();
        long parse_salary = long.Parse(emp_salary);

        int empNo = seq_no + 1;
        no.text = empNo.ToString();
        name.text = emp_name;
        team.text = emp_team;
        salary.text = string.Format(koreanCulture, "{0:N0}", parse_salary) + " 원";
        ability.text = emp_ability;

        Sprite prImage = Resources.Load<Sprite>(emp_facePath);
        profile.sprite = prImage;

        Button btn_empinfo = obj.GetComponent<Button>();
        btn_empinfo.onClick.AddListener(() => OnClickEmpInfoButton(no.text, name.text, team.text, salary.text, ability.text, profile.sprite));

        Transform rct = GameObject.Find("Employee_Content").transform;
        obj.transform.SetParent(rct);

        yield return null;

    }

    public void RemoveInScrollView(string sv_name)
    {
        Transform scrollview = GameObject.Find(sv_name).transform;
        Transform[] childs = scrollview.GetComponentsInChildren<Transform>();
        Debug.Log(scrollview.name);
        Debug.Log("scrollview son count : " + childs.Length + "one index name = " + childs[0].name);
        try
        {
            for (int i = 1; i < childs.Length; i++)
            {
                Debug.Log("childs name = " + childs[i].name);
                Destroy(childs[i].gameObject);
            }
        }
        catch (NullReferenceException e)
        {
            return;
        }
    }
    

    public void Orz_DropdownItemSelected(Dropdown dropdown)
    {
        RemoveInScrollView("Employee_Content");
        int index = dropdown.value;
        if(index == 0)
        {
            List<string[]> employeeInfo = GetEmployeeInformation();
            PreparationEmpInforButton(employeeInfo);
            return;
        }
        string selectedItem = dropdown.options[index].text;
        List<string[]> employee_infor = GetEmployeeInformation(selectedItem);
        PreparationEmpInforButton(employee_infor);
        //이벤트 리스너 제거 함수 추가 
    }

    public void OnClickEmpInfoButton(string no, string name, string team, string salary, string ability, Sprite profile)
    {
        EnadbleGameObject("EmployeeInformationPanel");
        Text txt_eip_name = GameObject.Find("txt_eip_name").GetComponent<Text>();
        Text txt_eip_sex = GameObject.Find("txt_eip_sex").GetComponent<Text>();
        Text txt_eip_national = GameObject.Find("txt_eip_national").GetComponent<Text>();
        Text txt_eip_age = GameObject.Find("txt_eip_age").GetComponent<Text>();
        Text txt_eip_workplace = GameObject.Find("txt_eip_workplace").GetComponent<Text>();
        Text txt_eip_management = GameObject.Find("txt_eip_management").GetComponent<Text>();
        Text txt_eip_department = GameObject.Find("txt_eip_department").GetComponent<Text>();
        Text txt_eip_position = GameObject.Find("txt_eip_position").GetComponent<Text>();
        Text txt_eip_salary = GameObject.Find("txt_eip_salary").GetComponent<Text>();
        Text txt_eip_ability = GameObject.Find("txt_eip_ability").GetComponent<Text>();

        int db_no = int.Parse(no);
        txt_eip_name.text = "이름 : " + name;
        txt_eip_sex.text = "성별 : " + EmployeeManager.epm.Get_Specificity_Employee_Information(db_no, 2);
        txt_eip_national.text = "국적 : " + EmployeeManager.epm.Get_Specificity_Employee_Information(db_no, 3);
        txt_eip_age.text = "나이 : " + EmployeeManager.epm.Get_Specificity_Employee_Information(db_no, 4);
        txt_eip_workplace.text = "근무지 : " + EmployeeManager.epm.Get_Specificity_Employee_Information(db_no, 5);
        txt_eip_management.text = "임원 : " + EmployeeManager.epm.Get_Specificity_Employee_Information(db_no, 6);
        txt_eip_department.text = "부서 : " + team;
        txt_eip_position.text = "보직 : " + EmployeeManager.epm.Get_Specificity_Employee_Information(db_no, 8);
        txt_eip_salary.text = "연봉 : " + salary;
        txt_eip_ability.text = "능력치 : " + ability;

    }

    public IEnumerator OnClick_Request_Product_Develepoment()
    {
        string[,] developmentList = DataBaseDown("Bio_pharmaceutical_data");
        int[] plan_no = new int[3];
        for(int i =0; i<plan_no.Length; i++)
        {
            int randomNumber = UnityEngine.Random.Range(1, 119);
            plan_no[i] = randomNumber;
        }

        yield return new WaitForSeconds(300f);

        string msg = "";
        string material_name;
        string can_apply;
        string average_prescribe;
        string ratio;

        for(int i=0; i<plan_no.Length; i++)
        {
            material_name = developmentList[plan_no[i], 1];
            can_apply = developmentList[plan_no[i], 2];
            average_prescribe = developmentList[plan_no[i], 3];
            ratio = developmentList[plan_no[i], 4];
            msg += $"{i}. 성분명 : {material_name} / 적응증 : {can_apply} / 연간 처방 건수 : {average_prescribe} / 처방 인구 비율 : {ratio}\n";
        }
        msg += "위와 같이 후보군을 선출하였습니다. 어떤 부분을 제품화 하는게 좋을지 확인 부탁 드립니다.";

        EventEngineManager.em.SendMail(119, "StragetyTF", "개발 제품 후보군 확인 부탁 드립니다", msg, "1번", "2번", "3", "", TimeController.tc.TimeGeneration());
    }

    public string[,] DataBaseDown(string filename)
    {
        development = csvcr.CSVRead("Bio_pharmaceutical_data");
        return development;
    }

    public void OnclickCompanyButtion()
    {
        EnadbleGameObject("CompanyInformationPanel");
    }

    public void OnClickMFGWorkOrder()
    {
        EnadbleGameObject("MFG_Order_Panel");
        DisadbleGameObject("CEOSpecificPanel");
        MFGWorkOrderPanel_Init();
    }

    public void MFGWorkOrderPanel_Init()
    {
        brandName = GameObject.Find("Dropdown_brandName").GetComponent<Dropdown>();
        mfg_campaign = GameObject.Find("Dropdown_mfg_campaign").GetComponent<Dropdown>();
        brandName.options.Clear();
        mfg_campaign.options.Clear();
        List<string> receivedBrandName = new();
        List<string> received_mfg_campaign = new() { "ENG", "GMP", "PV"};

        if (PlayData.ProductInformation != null)
        {
            for (int i = 0; i < PlayData.ProductInformation.Count; i++)
            {
                receivedBrandName.Add(PlayData.ProductInformation[i].BrandName);
                if (PlayData.ProductInformation[i].GetResultBLA())
                {
                    received_mfg_campaign.Add("Commercial");
                }
            }
        }
        else
        {
            receivedBrandName.Add("생산불가");
        }

        brandName.AddOptions(receivedBrandName);
        mfg_campaign.AddOptions(received_mfg_campaign);
    }

    public void MFG_Order()
    {
        batch_wo_count = GameObject.Find("InputField_mfg_batch").GetComponent<InputField>();
        string batch_count = batch_wo_count.text.Trim();
        try
        {
            Debug.Log("try statement join");
            Debug.Log(brandName.options[brandName.value].text);
            Debug.Log(this.mfg_campaign.options[this.mfg_campaign.value].text);
            int parse_batch_count = Int32.Parse(batch_count);
            MFGOrder mfg_order = new MFGOrder()
            {
                mfg_brandName = brandName.options[brandName.value].text,
                mfg_campaign = this.mfg_campaign.options[this.mfg_campaign.value].text,
                mfg_batch_amount = parse_batch_count,
                mfg_order_no = EventEngineManager.MFG_Order
            };
            EventEngineManager.mfg_scheduler.Received_MFG_Order(mfg_order, PlayData.playData.Get_DSdurationDay_inProductInformation(mfg_order.mfg_brandName));
        }
        catch
        {
            batch_wo_count.text = "";
            return;
        }
    }

    public void OnClickProductDevelpmentOrder()
    {
        EventEngineManager.engine.finding_new_drug_material_interval = 950f;
        EnadbleGameObject("PopupPanel");
        GenerationPopupMessage("유관부서에 제품 개발을 지시 하였습니다.");
    }

    public void OnClickNewDrugMaterial_Market_Research_Report_Generation(int newDrugMaterialNo)
    {
        string newDrugMaterialName = EventEngineManager.em.GetPharmaceutical_Name(newDrugMaterialNo);
        EventEngineManager.em.mrr.MarketResearchForNewDrugMaterial(newDrugMaterialName, newDrugMaterialNo-3000);
    }

    void OnDestroy()
    {
        dropdown.onValueChanged.RemoveAllListeners();
    }

}
