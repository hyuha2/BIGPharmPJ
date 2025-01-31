using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEngine : EventEngineManager
{
    
    public static EventEngineManager eem;
    public float finding_new_drug_material_interval = 1900f;
    public float product_development_process_check_interval = 300f;
    public float product_development_process_check_parameter_interval = 5f;

    private void Start()
    {
        if(eem==null)
        {
            eem = GameObject.Find("EventEngineManager").GetComponent<EventEngineManager>();
        }

        if(EventEngineManager.companyInformation.Count == 0)
        {
            GeneratedCompnay();
        }

        FindingNewDrugMaterial();
    }

    public IEnumerator FindingNewDrugMaterial()
    {
        while (true)
        {
            yield return new WaitForSeconds(finding_new_drug_material_interval);
            if (TimeController.tc.lastMonth != DateTime.Now.Month) //월마다 신규 물질을 자동으로 찾을 수 있도록 함. 
            {
                Find_NPC_NewDrugMaterial();
                Find_Player_NewDrugMaterial();
                TimeController.tc.lastMonth = DateTime.Now.Month; // 물질 발견월을 갱신하기 위해 만든 것 
            }
        }
    }

    public IEnumerator ProductDevelopmetProcessCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(product_development_process_check_interval);
            if(PlayData.newDrug_development_list.Count != 0)
            {
                for(int i=0; i<PlayData.newDrug_development_list.Count; i++)
                {
                    ProductDevelopmetProcessCheck_CellLineDevelopment(i);
                    if (PlayData.newDrug_development_list[i].cellLineDevelopment_completed == true)
                    {
                        PlayData.newDrug_development_list[i].SetManufacturedDevelopomentCompletedTime();
                    }

                    ProductDevelopmetProcessCheck_ManufactureDevelopmentProcess(i);
                    if (PlayData.newDrug_development_list[i].manufacturedDevelopment_completed)
                    {
                        PlayData.newDrug_development_list[i].manufactureProcessValidation_eng = true;
                        ProductDevelopmetProcessCheck_ManufactureDevelopmentProcess_ENG(i);
                    }

                    if (PlayData.newDrug_development_list[i].manufacturedDevelopment_completed == false)
                    {
                        PlayData.newDrug_development_list[i].manufacturedDevelopment_completed = TimeController.tc.Compare_A_and_B_Time((DateTime)PlayData.newDrug_development_list[i].manufacturedDevelopmentCompletedTime);
                        if (PlayData.newDrug_development_list[i].manufacturedDevelopment_completed)
                        {
                            PlayData.newDrug_development_list[i].manufactureProcessValidation_eng = true;
                            MFGOrder order = new MFGOrder { mfg_order_no = MFG_Order,
                                                            requestOrder_ds = true,
                                                            requestOrder_dp = false,
                                                            forProductDevelopment = true,
                                                            forProcessValidation = false,
                                                            forGMPbatch = false,
                                                            mfg_brandName = PlayData.newDrug_development_list[i].developBrandName,
                                                            mfg_batch_amount = 1,
                                                            mfg_campaign = "ENG"};
                            mfg_scheduler.Received_MFG_Order(order, (int)PlayData.newDrug_development_list[i].manufacturedDevelopmentDurationTime / 60);
                            PlayData.newDrug_development_list[i].SetQualityCheckTime();
                            quality_scheduler.Schedule_Direct_Assignment((DateTime)PlayData.newDrug_development_list[i].qualityCheckCompletedTimeAsPerBatch);
                        }
                    }

                    if (PlayData.newDrug_development_list[i].qualityCheck_end == false)
                    {
                        PlayData.newDrug_development_list[i].qualityCheck_end = TimeController.tc.Compare_A_and_B_Time((DateTime)PlayData.newDrug_development_list[i].qualityCheckCompletedTimeAsPerBatch);
                        if (PlayData.newDrug_development_list[i].qualityCheck_end)
                        {
                            PlayData.newDrug_development_list[i].qualityCheck_end = false;
                            if(PlayData.newDrug_development_list[i].preClinicalStart == false)
                            {
                                PlayData.newDrug_development_list[i].preClinicalStart = true;
                                PlayData.newDrug_development_list[i].SetPreclinicalCompletedTime();
                                PlayData.newDrug_development_list[i].manufactureProcessValidation_start = true;
                                MFGOrder order = new MFGOrder
                                {
                                    mfg_order_no = MFG_Order,
                                    requestOrder_ds = true,
                                    requestOrder_dp = false,
                                    forProductDevelopment = true,
                                    forProcessValidation = true,
                                    forGMPbatch = false,
                                    mfg_brandName = PlayData.newDrug_development_list[i].developBrandName,
                                    mfg_batch_amount = 1,
                                    mfg_campaign = "PV"
                                };
                                mfg_scheduler.Received_MFG_Order(order, PlayData.playData.Get_DSdurationDay_inProductInformation(order.mfg_brandName));
                                PlayData.newDrug_development_list[i].SetQualityCheckTime();
                            }    
                        }
                    }

                    if (PlayData.newDrug_development_list[i].preClinicalEnd == false)
                    {
                        PlayData.newDrug_development_list[i].preClinicalEnd = TimeController.tc.Compare_A_and_B_Time((DateTime)PlayData.newDrug_development_list[i].preClinicalCompletedTime);
                        if (PlayData.newDrug_development_list[i].preClinicalEnd)
                        {
                            PlayData.newDrug_development_list[i].qualityCheck_end = false;
                            PlayData.newDrug_development_list[i].clinical_phase1_start = true;
                            PlayData.newDrug_development_list[i].SetClinicalCompletedTime();
                        }
                    }

                }
            }
        }
        
    }

    public IEnumerator ProductDevelopmetProcessCheck_CellLineDevelopment(int indexno)
    {
        while(PlayData.newDrug_development_list[indexno].cellLineDevelopment_completed != true)
        {
            yield return new WaitForSeconds(product_development_process_check_parameter_interval);
            PlayData.newDrug_development_list[indexno].cellLineDevelopment_completed = TimeController.tc.Compare_A_and_B_Time((DateTime)PlayData.newDrug_development_list[indexno].cellLineDevelpomentCompletedTime);   
        }
    }

    public IEnumerator ProductDevelopmetProcessCheck_ManufactureDevelopmentProcess(int indexno)
    {
        while (PlayData.newDrug_development_list[indexno].cellLineDevelopment_completed && PlayData.newDrug_development_list[indexno].manufacturedDevelopment_completed != true)
        {
            yield return new WaitForSeconds(product_development_process_check_parameter_interval);
            PlayData.newDrug_development_list[indexno].manufacturedDevelopment_completed = TimeController.tc.Compare_A_and_B_Time((DateTime)PlayData.newDrug_development_list[indexno].manufacturedDevelopmentCompletedTime);
        }
    }

    public void ProductDevelopmetProcessCheck_ManufactureDevelopmentProcess_ENG(int indexno)
    {
        MFGOrder order = new MFGOrder
        {
            mfg_order_no = MFG_Order,
            requestOrder_ds = true,
            requestOrder_dp = false,
            forProductDevelopment = true,
            forENGbatch = true,
            forProcessValidation = false,
            forGMPbatch = false,
            mfg_brandName = PlayData.newDrug_development_list[indexno].developBrandName,
            mfg_batch_amount = 1,
            mfg_campaign = "ENG",
            indexno = indexno,

        };
        mfg_scheduler.Received_MFG_Order(order, (int)PlayData.newDrug_development_list[indexno].manufacture_ds_duration_day);
        RequestOrder_QualityCheck_ProcessDevelopmentSample(order, indexno);
    }

    public void RequestOrder_QualityCheck_ProcessDevelopmentSample(MFGOrder order, int indexno)
    {
        QualityOrder qcOrder = new QualityOrder
        {
            MFG_order_no = order.mfg_order_no,
            batch_no = order.batchNo,
            dp = false,
            ds = true,
            ipc = true,
            investgiationSample = false,
            emSample = false,
            utilitySample = false,
            validationSample = order.forProcessValidation
        };
        order.qc_request_testing_orderNo = qcOrder.qc_order_no; // mfg_order가 저장이 되는건가?? 확인 필요.
        quality_scheduler.Received_Quality_Test_Order(qcOrder, order.mfg_batch_amount, (int)(PlayData.newDrug_development_list[indexno].qualityConfirmDurtationTime / 60) / 60 * 24);
    }

    private void Update()
    {

    }

    public int RandomNumber(int count)
    {
        int randomnumber = (int)UnityEngine.Random.Range(1, count+1);
        return randomnumber;
    }

    public int[] RandomEventNo(int ev_generation_count)
    {
        int[] eventno = new int[ev_generation_count];
        for (int i = 0; i < eventno.Length; i++)
        {
            int no;
            do
            {
                no = UnityEngine.Random.Range(4, GetEventDatalist().GetLength(0));
                no = FilterForGetRandomEventNo(no);
            }
            while (no == -1);
            eventno.SetValue(no, i);

            for (int j = 0; j < i; j++)
            {
                while (eventno[j] == eventno[i]) // evetno가 중복으로 생성 될 경우 재생성을 위해 반복문 들어감.
                {
                    Debug.Log("While statement entered");
                    do
                    {
                        no = UnityEngine.Random.Range(4, GetEventDatalist().GetLength(0));
                        no = FilterForGetRandomEventNo(no);
                    }
                    while (no == -1);
                    eventno.SetValue(no, i);//값을 인덱스에 set. SetValue(Value, index no) 
                    Debug.Log("Whiele statment end point");
                }
            }
        }
        Array.Sort(eventno);
        return eventno;
    }

    public int FilterForGetRandomEventNo(int no)
    {//EventCondition_Money,EventCondition_StaffCount,EventCondition_Licence,EventCondition_Time,EventCondition_TimeInterval_Week
        string[,] datalist = GetEventDatalist();
        bool eventConditionMoney = PlayData.playData.GetCompany_Money() >= long.Parse(datalist[no, 4]);
        Debug.Log("long.Parse(datalist[no, 4] = " + long.Parse(datalist[no, 4]) + "/" + eventConditionMoney);
        bool eventConditionStaffCount = EmployeeManager.epm.hired_person.Count >= int.Parse(datalist[no, 5]);
        Debug.Log("int.Parse(datalist[no, 5]" + int.Parse(datalist[no, 5]) + "/" + eventConditionStaffCount);
        //bool eventConditionLicence = PlayData.playData.approved_commercial_licence.Count >= int.Parse(datalist[no, 6]); //BLA 구현후 if 문에 반영 필요 
        bool eventConditionTime = TimeController.tc.GetGameStartTime().Year >= int.Parse(datalist[no, 7]);
        Debug.Log("int.Parse(datalist[no, 7]" + int.Parse(datalist[no, 7]) + "/" + eventConditionTime);

        if (eventConditionMoney && eventConditionStaffCount && eventConditionTime)
        {
            Debug.Log("이벤트 발생 조건이 모두 적합합니다");
            return no;
        }

        return -1;
    }

    public void GeneratedCompnay()
    {
        string[,] companyData = EventEngineManager.csvc.CSVRead("company_information.csv");
        for(int i=1; i<companyData.GetLength(0); i++)
        {
            for(int j=0; j<companyData.GetLength(1); j++)
            {
                if (!EventEngineManager.companyInformation.ContainsKey(companyData[i, 1]))
                {
                    Product newProduct = new Product
                    {
                        BrandName = companyData[i, 4],
                        IngredientName = companyData[i, 3]
                    };
                    EventEngineManager.companyInformation[companyData[i, 1]] = new Company { ProductInformation = new List<Product> { newProduct }, Name = companyData[i, 1], Nationality = companyData[i,2]};
                    EventEngineManager.companyInformation[companyData[i, 1]].SetFixedExpenditure();
                    EventEngineManager.companyNo.Add(int.Parse(companyData[i, 0]));
                    Debug.Log(companyData[i, 1]);
                }
            }
        }
        
    }

    public int SpecificDecisionSendEmailCount(string eventtype1) //play evet시 몇 개 보낼지 셀거 
    {
        string[,] eventdata = eem.GetEventDatalist();
        int rowcolcount = eventdata.GetUpperBound(0)+1;
        List<bool> filter = new List<bool>();
        for(int i=0; i<rowcolcount; i++)
        {
           if(eventtype1.Equals(eventdata.GetValue(i,2)))
                filter.Add(true);
        }        
        int decision_sendEmail_count = filter.Count;
        return decision_sendEmail_count;
    }
    
    public object EventDataTransferForEmail(string[,] eventdatalist, int indexfrom, int indexto)
    {
        int  count = (indexto - indexfrom)+1;
        for(int i=0; i<count+1; i++)
        {
            var a = new {sender = eventdatalist.GetValue(indexfrom-1,10), emailSubject = eventdatalist.GetValue(indexfrom-1,13) };
            //from++;
        }
        return eventdatalist;
    }

    public int InitailEventDataTransferCountForEmail(int joincount) // 신규 게임시 적용
    {
        int decision_sendemail_count = 0;
        while(decision_sendemail_count==0)
        {
            decision_sendemail_count = RandomNumber(joincount);
        }
        Debug.Log("decision_sendemail_count =" + decision_sendemail_count); 
        return decision_sendemail_count;
    }

    public string[,] EventFilter(string[,] eventdatalist, string keyword)
    {
        int joincount = KeywordCount(eventdatalist, keyword);
        int rowpoint = 1;
        string[,] filter = new string[joincount+1, eventdatalist.GetLength(1)];
        for(int i =0; i<eventdatalist.GetLength(1); i++) //컬럼명 먼저 저장 
        {
            filter[0,i] = (string)eventdatalist.GetValue(0,i);
        }
        for(int i =0; i < eventdatalist.GetLength(0); i++)
        {
            for(int j=0; j<eventdatalist.GetLength(1); j++)
            {
                if(eventdatalist.GetValue(i,j).Equals(keyword))
                {
                    for(int a=0; a < eventdatalist.GetLength(1); a++)
                    {
                        filter[rowpoint, a] = (string)eventdatalist.GetValue(i,a);
                    }
                     rowpoint++;
                }
            }
        }
        return filter;
    }

    public string[,] EventFilterUsingEvetNo(int[] eventno)
    {
        string[,] eventdatalist = eem.GetEventDatalist();
        int rowlength = eventdatalist.GetLength(1);
        string[,] contents = new string[eventno.Length+1, rowlength];
        for(int i=0; i<eventno.Length; i++)
        {
            for(int j=0; j<rowlength; j++)
            {
                if(i==0)
                {
                    for(int c=0; c<rowlength; c++)
                    {
                        contents[i, c] = eventdatalist[0, c];
                    }    
                }
                contents[i+1, j] = eventdatalist[eventno[i], j];
            }
        }
        return contents;
    }
    
    public void InitialEventContents(string[,] eventdatalist)
    {
        eem = GameObject.Find("EventEngineManager").GetComponent<EventEngineManager>();
        string[,] filter = EventFilter(eventdatalist, "Join"); //Join으로 분류된 이벤트 리스트를 가져옴.
        eem.AddEventList(filter); // 분류된 모든 이벤트 키워드 내용 리스트에 딕셔너리 타입으로 저장 
        eem.AddQueue(); // 위 리스트를 큐에 모두 담음 (굳이???? 빼내오기 쉬울라고..쓴거)
        int releaseevnetcount = eem.ReleaseEventCount(); // 그 중 몇 개의 초기 이벤트를 방출할건지 개수를 결정
        eem.DicBindingInList(releaseevnetcount); // 결정된 개수만큼 큐에서 꺼내 릴리즈 이벤트 리스트에 딕셔너리 타입으로 다시 담음
        eem.SliceDicinQueForEmailListDataSet(); // 메일 리스트에 필요한 데이터 각 요소를 큐로 각 요소별로 담음
        for (int i = 0; i < releaseevnetcount; i++)
        {
            eem.PrefabEmailListDequeAfterDataSet();
        }
        GameManager.gm.SetNewGame(false); //현재 로직 흐름상 로드 되면 게임 초기화 되는걸 방지하기 위해 값 변경.
    }

    public void EventGeneration()
    {
        Debug.Log("EvnetGeneration()호출");
        int event_generation_decison_count = (int)UnityEngine.Random.Range(1, 4);
        int[] eventno = RandomEventNo(event_generation_decison_count);
        string[,] filter = EventFilterUsingEvetNo(eventno);
        WokrFlowListandDicandQueueandPrefabEmail(filter);
        //engine.EventGeneration(event_generation_decison_count, eventno);
    }

    public void WokrFlowListandDicandQueueandPrefabEmail(string[,] filter) // 이니셜 이벤트 가져다 붙였더니 초기 이벤트가 도출되니 reowpoint 쪽에 아마 수정 필.
    {
        eem.AddEventList(filter); //tempeventlist에 등록.
        eem.AddQueue(); //sendmailwaitlist 큐에 등록.
        int releaseevnetcount = eem.ReleaseEventCount(); // 그 중 몇 개의 초기 이벤트를 방출할건지 개수를 결정
        eem.DicBindingInList(releaseevnetcount); // 결정된 개수만큼 큐에서 꺼내 릴리즈 이벤트 리스트에 딕셔너리 타입으로 다시 담음
        eem.SliceDicinQueForEmailListDataSet(); // 메일 리스트에 필요한 데이터 각 요소를 큐로 각 요소별로 담음
        for (int i = 0; i < releaseevnetcount; i++)
        {
            eem.PrefabEmailListDequeAfterDataSet();
        }
    }

    public int KeywordCount(string[,] datalist, string keyword)
    {
        int count = 0;
        for(int i =0; i < datalist.GetLength(0); i++)
        {
            for(int j=0; j<datalist.GetLength(1); j++)
            {
                if(datalist.GetValue(i,j).Equals(keyword))
                {
                     count++;
                }
            }
        }
        return count;
    }

    public void AddForSendingEmail(int no, string sender, string title, string contentmsg, string buttonaction1, string buttonaction2, string buttonaction3)
    {
        Dictionary<string, object> addmail = new();
        addmail.Add("No", no);
        addmail.Add("ReportHost", sender);
        addmail.Add("MailSubject", title);
        addmail.Add("EventContents", contentmsg);
        addmail.Add("ButtonAction1", buttonaction1);
        addmail.Add("ButtonAction2", buttonaction2);
        addmail.Add("ButtonAction3", buttonaction3);
        EventEngineManager.em.tmpeventlist.Add(addmail);
    }

    public void Find_NPC_NewDrugMaterial()
    {
        int minRangeValue = 1;
        int npcMaxRangeValue = 10001;
        int fixedValue = 0;

        foreach(KeyValuePair<string, Company> kvp in EventEngineManager.companyInformation)
        {
            fixedValue = UnityEngine.Random.Range(minRangeValue, npcMaxRangeValue);
            int employeeCount = kvp.Value.EmployeeCount;
            float rdRatio = kvp.Value.RDEmployeeRatio;
            int tolerance = (int)(employeeCount * rdRatio) / 2;
            int high_tolerance = fixedValue + (tolerance / 2); // 그래서 fixed Value 기준으로 더 높은 허용 범위를 만들고 
            int low_tolerance = fixedValue - (tolerance / 2); // 더 낮은 허용 범위를 만들고 
            if (fixedValue >= low_tolerance && fixedValue <= high_tolerance)
            {
                int ingredeintNo = UnityEngine.Random.Range(1, pharmaceutical_list.GetLength(0));
                for(int i=0; i<pharmaceutical_list.GetLength(0); i++)
                {
                    try
                    {
                        if (kvp.Value.ProductInformation[i].IngredientName != pharmaceutical_list[i, 1])
                        {
                            if (pharmaceutical_list[i, 1] == pharmaceutical_list[ingredeintNo, 1])
                            {
                                kvp.Value.Find_NewDrugMaterials.Add(new Product() { pharmaceutical_data_no = Int32.Parse(pharmaceutical_list[ingredeintNo, 0]),
                                                                                    IngredientName = pharmaceutical_list[ingredeintNo, 1] });
                            }
                        }
                    }
                    catch(NullReferenceException e)
                    {
                        PanelController.PC.EnadbleGameObject("PopupPanel");
                        PanelController.PC.GenerationPopupMessage("From NPC Find New Drug Material Logic, " + e.Message);
                    }
                    
                }
            }
        }
    }

    public void Find_Player_NewDrugMaterial()
    {
        int minRangeValue = 1;
        int playerMaxRangeValue = 100001;
        int fixedValue = UnityEngine.Random.Range(minRangeValue, playerMaxRangeValue); // 1~100000을 랜덤하게 생성해서 
        int sumRDEmployeeAbility = 0;// 연구소 직원 능력치에 대해 보정계수로 사용할 변수를 만들어 놓고 
        List<int> RDEmployee = EmployeeManager.epm.FindEmployee_department(Department.RD.ToString()); // 모든 연구소 직원의 데이터 번호를 가져온 다음에 
        string[] RDEmployeeInformation = new string[RDEmployee.Count]; // 문자열 배열을 연구소 직원의 수만큼 생성하고 

        foreach(int a in RDEmployee)
        {
            RDEmployeeInformation = EmployeeManager.epm.Employee_information(a); // 연구소 직원의 모든 정보를 담은 다음 
            sumRDEmployeeAbility += int.Parse(RDEmployeeInformation[10]); // 연구소 직원의 능력치를 모두 더한 다음 
        }

        int tolerance = sumRDEmployeeAbility / 2; // 연구소 직원 능력치 합을 2로 나눈 결과를 물질 발견 성공에 대한 허용 범위로 만들 것임
        int high_tolerance = fixedValue + (tolerance / 2); // 그래서 fixed Value 기준으로 더 높은 허용 범위를 만들고 
        int low_tolerance = fixedValue - (tolerance / 2); // 더 낮은 허용 범위를 만들고 
        if(fixedValue <= high_tolerance && fixedValue >= low_tolerance) // 허용 범위가 랜덤 번호보다 크거나 작다면 
        {
            int ingredeintNo = UnityEngine.Random.Range(1, pharmaceutical_list.GetLength(0)); // 등록된 물질 번호를 랜덤하게 하나 가져오고 
            for(int i=0; i<pharmaceutical_list.GetLength(0); i++)  // 그런데 등록된 물질 개수 만큼 반복문을 돌릴거야 
            {
                try
                {
                    if (PlayData.ProductInformation[i].IngredientName != pharmaceutical_list[i, 1]) // 만약 PlayData.ProductInformation 리스트와 등록된 물질 번호가 동일하지 않다면 없던 후보물질이므로 
                    {
                        if (pharmaceutical_list[i, 1] == pharmaceutical_list[ingredeintNo, 1]) // csv에 등록된 동일 물질과 랜덤으로 가져온 등록된 동일 물질이 동일해질 때 찾은 물질로 등록하고  
                        {
                            PlayData.Find_NewDrugMaterials.Add(new Product() {pharmaceutical_data_no = Int32.Parse(pharmaceutical_list[ingredeintNo, 0]),
                                                                              IngredientName = pharmaceutical_list[ingredeintNo, 1]}); // 새로 찾게된 신규물질로 등록하고 
                            int empNo = EmployeeManager.epm.EmailSender_Random(Department.RD.ToString()); // 보고할 보고자를 랜덤하게 1명 생성한 다음 
                            string sender = EmployeeManager.epm.Get_Specificity_Employee_Information(empNo, 1); // 보고자의 이름을 가져오고 
                            string content = $"{pharmaceutical_list[i, 1]}에 대해 제품 개발 가능하여 보고 드립니다. 해당 성분은 {pharmaceutical_list[i, 2]}에 사용 될 수 있습니다."; // 메일 문구를 생성한 다음 
                            //string sender, string title, string contentmsg, string buttonaction1, string buttonaction2, string buttonaction3)
                            AddForSendingEmail(Int32.Parse(pharmaceutical_list[ingredeintNo, 0]), sender, $"{pharmaceutical_list[i, 1]} 성분에 대한 제품 개발 가능 보고", content, "시장조사", "개발진행", "N/A"); //메일 발신 리스트에 넣음
                            finding_new_drug_material_interval = 1900f;
                        }
                    }
                }
                catch(NullReferenceException e)
                {
                    PanelController.PC.EnadbleGameObject("PopupPanel");
                    PanelController.PC.GenerationPopupMessage(e.Message);
                }
            }

        }
    }
}
