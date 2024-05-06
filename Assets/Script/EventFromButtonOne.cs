using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFromButtonOne : EventEngine // 모드별 이벤트 엔진에 함수 정의해서 뭔가를 할 수 있게 되었다 .
{
    public void Arrange(string btn_name, int objId)
    {
        Debug.Log("호출된 btn_name과 ojtID를 확인합니다." + btn_name + "," + objId);
        switch (btn_name)
        {            
            case "은행대출":
                PanelController.PC.DeInteratablePanel("Mail_Content_Panel");
                PanelController.PC.SendMessage("LoanBankPanel", objId, SendMessageOptions.RequireReceiver);
                break;
        }
    }
    


}
