using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventFromButtonOne : EventEngine // 모드별 이벤트 엔진에 함수 정의해서 뭔가를 할 수 있게 되었다 .
{
    int objIDforTemprary = 0; // ButtonCorrectiondProcessFromXtoNA 메서드에서 임시로 사용하기 위해 만든 변수 
    public void Arrange(string btn_name, int objId)
    {
        objIDforTemprary = objId;
        switch (btn_name)
        {            
            case "은행대출":
                PanelController.PC.DeInteratablePanel("Mail_Content_Panel");
                PanelController.PC.SendMessage("LoanBankPanel", objId, SendMessageOptions.RequireReceiver);
                break;

            case "투자확정":
                PanelController.PC.DeInteratablePanel("Mail_Content_Panel");
                PanelController.PC.FixedInvest();
                Transform[] obj = GameObject.Find("Content").GetComponentsInChildren<Transform>();
                for (int i = 0; i < obj.Length; i++)
                {
                    if (obj[i].gameObject.GetInstanceID() == objId)
                    {
                        Debug.Log("InstanceID가 동일함을 확인하였습니다. object 이름은 -> " + obj[i].name);

                        EmailButtonPrefab ebp = obj[i].Find("MailListController").GetComponent<EmailButtonPrefab>();
                        ebp.buttonaction1 = "N/A";
                        ebp.buttonaction2 = "N/A";
                        ebp.buttonaction3 = "N/A";

                        if (EventEngineManager.em.hub.mcsi.ContainsKey(objId))
                        {
                            EventEngineManager.em.hub.mcsi[objId].buttonaction1 = "N/A";
                            EventEngineManager.em.hub.mcsi[objId].buttonaction2 = "N/A";
                            EventEngineManager.em.hub.mcsi[objId].buttonaction3 = "N/A";
                        }
                    }
                }
                PanelController.PC.invest_fix = false;
                EventEngineManager.controlledEventNo.Remove(objId);
                break;

            case "시장조사":
                int newDrugMaterilNo = GameObject.Find("Mail_Content_Panel").GetComponent<MailInstanceIDHub>().eventNo;
                PanelController.PC.OnClickNewDrugMaterial_Market_Research_Report_Generation(newDrugMaterilNo);
                PanelController.PC.DeInteratablePanel("Mail_Content_Panel");
                PanelController.PC.EnadbleGameObject("PopupPanel");
                PanelController.PC.GenerationPopupMessage("유관부서에 시장 조사를 지시를 하였습니다.");
                break;
        }
    }

    public void ButtonCorrectiondProcessFromXtoNA()
    {
        Transform[] obj = GameObject.Find("Content").GetComponentsInChildren<Transform>();

        for (int i = 0; i < obj.Length; i++)
        {
            if (obj[i].gameObject.GetInstanceID() == objIDforTemprary)
            {
                EmailButtonPrefab ebp = obj[i].Find("MailListController").GetComponent<EmailButtonPrefab>();
                ebp.buttonaction1 = "N/A";

                if (EventEngineManager.em.hub.mcsi.ContainsKey(objIDforTemprary))
                {
                    EventEngineManager.em.hub.mcsi[objIDforTemprary].buttonaction1 = "N/A";
                }
            }
        }
    }
}
