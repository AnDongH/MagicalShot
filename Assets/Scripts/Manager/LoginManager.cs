using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("ȸ�� ���� UI")]
    [SerializeField] private GameObject registerUI;
    [SerializeField] private InputField r_emailInput;
    [SerializeField] private InputField r_passwordInput;
    [SerializeField] private InputField r_accountNameInput;

    [Header("�α��� UI")]
    [SerializeField] private GameObject loginUI;
    [SerializeField] private InputField l_emailInput;
    [SerializeField] private InputField l_passwordInput;

    [Header("�г��� UI")]
    [SerializeField] private GameObject nickNameUI;
    [SerializeField] private InputField nickNameInput;

    [Header("���� UI")]
    [SerializeField] private GameObject connectUI;
    [SerializeField] private Text nickNameText;
    public string  nickName { get; private set; }

    // �α���
    public void LoginBtn() {
        var request = new LoginWithEmailAddressRequest { 
            Email = l_emailInput.text, 
            Password = l_passwordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }


    // ȸ�� ����
    public void RegisterBtn() {
        var request = new RegisterPlayFabUserRequest { Email = r_emailInput.text, Password = r_passwordInput.text, Username = r_accountNameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);

    }

    // ȸ������ â Ŵ
    public void OnRegisterUIOn() {
        registerUI.SetActive(true);
    }

    // ȸ������ â ��
    public void OnRegisterUIOff() {
        registerUI.SetActive(false);
    }


    // �α��� ���� �ݹ�
    private void OnLoginSuccess(LoginResult result) {
        print("�α��� ����");
        loginUI.SetActive(false);
        string name = result.InfoResultPayload.PlayerProfile.DisplayName;

        if (name == null) {
            nickNameUI.SetActive(true);
        }
        else {
            connectUI.SetActive(true);
            nickNameText.text = name + "�� ȯ���մϴ�!";
            nickName = name;
        }
    }

    // �α��� ���� �ݹ�
    private void OnLoginFailure(PlayFabError error) => print("�α��� ����");

    // ȸ������ ���� �ݹ�
    private void OnRegisterSuccess(RegisterPlayFabUserResult result) {
        print("ȸ�� ���� ����");
        OnRegisterUIOff();
    }

    // ȸ������ ���� �ݹ�
    private void OnRegisterFailure(PlayFabError error) => print("ȸ������ ����");

    // �г��� ����
    public void UpdateDisplayName() {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName = nickNameInput.text
        }, result => {
            Debug.Log("The player's display name is now: " + result.DisplayName);
            nickNameUI.SetActive(false);
            connectUI.SetActive(true);
            nickNameText.text = result.DisplayName + "�� ȯ���մϴ�!";
            nickName = result.DisplayName;
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

}
