using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabManager : Singleton<PlayFabManager>
{
    public string  nickName { get; private set; }

    // �α���
    public void Login(InputField email, InputField pw) {
        var request = new LoginWithEmailAddressRequest { 
            Email = email.text, 
            Password = pw.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }


    // ȸ�� ����
    public void Register(InputField email, InputField pw, InputField userName) {
        var request = new RegisterPlayFabUserRequest { 
            Email = email.text, 
            Password = pw.text, 
            Username = userName.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }


    // �α��� ���� �ݹ�
    private void OnLoginSuccess(LoginResult result) {
        print("�α��� ����");
        UI_Manager.Instance.ClosePopupUI();
        nickName = result.InfoResultPayload.PlayerProfile.DisplayName;

        if (nickName == null) {
            UI_Manager.Instance.ShowPopupUI<UI_PopUp>("NickNameCanvas");
        }
        else {
            UI_Manager.Instance.ShowPopupUI<UI_PopUp>("ConnectCanvas");
        }
    }

    // �α��� ���� �ݹ�
    private void OnLoginFailure(PlayFabError error) => print("�α��� ����");

    // ȸ������ ���� �ݹ�
    private void OnRegisterSuccess(RegisterPlayFabUserResult result) {
        print("ȸ�� ���� ����");
        UI_Manager.Instance.ClosePopupUI();
    }

    // ȸ������ ���� �ݹ�
    private void OnRegisterFailure(PlayFabError error) => print("ȸ������ ����");

    // �г��� ����
    public void UpdateDisplayName(InputField name) {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName = name.text
        }, result => {
            Debug.Log("The player's display name is now: " + result.DisplayName);
            nickName = result.DisplayName;
            UI_Manager.Instance.ClosePopupUI();
            UI_Manager.Instance.ShowPopupUI<UI_PopUp>("ConnectCanvas");

        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

}
