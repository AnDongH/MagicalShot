using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabManager : Singleton<PlayFabManager>
{
    public string  nickName { get; private set; }

    // 로그인
    public void Login(InputField email, InputField pw) {
        var request = new LoginWithEmailAddressRequest { 
            Email = email.text, 
            Password = pw.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }


    // 회원 가입
    public void Register(InputField email, InputField pw, InputField userName) {
        var request = new RegisterPlayFabUserRequest { 
            Email = email.text, 
            Password = pw.text, 
            Username = userName.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }


    // 로그인 성공 콜백
    private void OnLoginSuccess(LoginResult result) {
        print("로그인 성공");
        UI_Manager.Instance.ClosePopupUI();
        nickName = result.InfoResultPayload.PlayerProfile.DisplayName;

        if (nickName == null) {
            UI_Manager.Instance.ShowPopupUI<UI_PopUp>("NickNameCanvas");
        }
        else {
            UI_Manager.Instance.ShowPopupUI<UI_PopUp>("ConnectCanvas");
        }
    }

    // 로그인 실패 콜백
    private void OnLoginFailure(PlayFabError error) => print("로그인 실패");

    // 회원가입 성공 콜백
    private void OnRegisterSuccess(RegisterPlayFabUserResult result) {
        print("회원 가입 성공");
        UI_Manager.Instance.ClosePopupUI();
    }

    // 회원가입 실패 콜백
    private void OnRegisterFailure(PlayFabError error) => print("회원가입 실패");

    // 닉네임 설정
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
