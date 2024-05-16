using PlayFab;
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

    private PhotonLoginManager loginManager;

    protected override void Init() {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.SrvConnectBtn).gameObject.BindEvent(OnSrvConnectBtnClicked);
        GetText((int)Texts.NickName_Text).text = PlayFabManager.Instance.NickName + "님 환영합니다!";
    }

    private void Start() {
        loginManager = PhotonLoginManager.Instance;
        Init();
    }

    private void OnSrvConnectBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("Connect");
        loginManager.Connect();
    }
}
