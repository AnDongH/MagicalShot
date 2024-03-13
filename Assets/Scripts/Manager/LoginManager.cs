using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("회원 가입 UI")]
    [SerializeField] private GameObject registerUI;
    [SerializeField] private InputField r_emailInput;
    [SerializeField] private InputField r_passwordInput;
    [SerializeField] private InputField r_accountNameInput;

    [Header("로그인 UI")]
    [SerializeField] private GameObject loginUI;
    [SerializeField] private InputField l_emailInput;
    [SerializeField] private InputField l_passwordInput;

    [Header("닉네임 UI")]
    [SerializeField] private GameObject nickNameUI;
    [SerializeField] private InputField nickNameInput;

    [Header("접속 UI")]
    [SerializeField] private GameObject connectUI;
    [SerializeField] private Text nickNameText;
    public string  nickName { get; private set; }

    // 로그인
    public void LoginBtn() {
        var request = new LoginWithEmailAddressRequest { 
            Email = l_emailInput.text, 
            Password = l_passwordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }


    // 회원 가입
    public void RegisterBtn() {
        var request = new RegisterPlayFabUserRequest { Email = r_emailInput.text, Password = r_passwordInput.text, Username = r_accountNameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);

    }

    // 회원가입 창 킴
    public void OnRegisterUIOn() {
        registerUI.SetActive(true);
    }

    // 회원가입 창 끔
    public void OnRegisterUIOff() {
        registerUI.SetActive(false);
    }


    // 로그인 성공 콜백
    private void OnLoginSuccess(LoginResult result) {
        print("로그인 성공");
        loginUI.SetActive(false);
        string name = result.InfoResultPayload.PlayerProfile.DisplayName;

        if (name == null) {
            nickNameUI.SetActive(true);
        }
        else {
            connectUI.SetActive(true);
            nickNameText.text = name + "님 환영합니다!";
            nickName = name;
        }
    }

    // 로그인 실패 콜백
    private void OnLoginFailure(PlayFabError error) => print("로그인 실패");

    // 회원가입 성공 콜백
    private void OnRegisterSuccess(RegisterPlayFabUserResult result) {
        print("회원 가입 성공");
        OnRegisterUIOff();
    }

    // 회원가입 실패 콜백
    private void OnRegisterFailure(PlayFabError error) => print("회원가입 실패");

    // 닉네임 설정
    public void UpdateDisplayName() {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName = nickNameInput.text
        }, result => {
            Debug.Log("The player's display name is now: " + result.DisplayName);
            nickNameUI.SetActive(false);
            connectUI.SetActive(true);
            nickNameText.text = result.DisplayName + "님 환영합니다!";
            nickName = result.DisplayName;
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

}
