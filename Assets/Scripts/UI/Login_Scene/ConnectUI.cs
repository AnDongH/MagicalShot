using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConnectUI : UI_PopUp
{
    enum Buttons {
        SrvConnectBtn
    }

    enum Texts {
        NickName_Text
    }
    protected override void Init() {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.SrvConnectBtn).gameObject.BindEvent(OnSrvConnectBtnClicked);
        GetText((int)Texts.NickName_Text).text = PlayFabManager.Instance.nickName + "�� ȯ���մϴ�!";
    }

    private void Start() {
        Init();
    }

    private void OnSrvConnectBtnClicked(PointerEventData data) {
        PhotonManager.Instance.Connect();
    }
}
