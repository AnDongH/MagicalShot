using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PlayFabManager : DontDestroySingleton<PlayFabManager> {

    #region delegate
    public delegate void OnRankGetHandler(List<PlayerLeaderboardEntry> playerList);
    public delegate void OnLoginFailedHandler(string error);
    public delegate void OnRegisterFailedHandler(string error);
    public delegate void OnFindSuccessHandler(string error);
    public delegate void OnFindFailedHandler(string error);
    #endregion

    #region event
    public static event OnRankGetHandler OnRankGet;
    public static event OnLoginFailedHandler OnLoginFailed;
    public static event OnRegisterFailedHandler OnRegisterFailed;
    public static event OnFindSuccessHandler OnFindSuccess;
    public static event OnFindFailedHandler OnFindFailed;
    #endregion

    public string NickName { get; private set; }
    public string PlayFabID { get; private set; }

    // �α���
    public void Login(InputField email, InputField pw) {
        var request = new LoginWithEmailAddressRequest {
            Email = email.text,
            Password = pw.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
        };
        GameManager.ShowLoadingUI();
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);

    }

    public void Logout() {
        PlayFabClientAPI.ForgetAllCredentials();
    }

    // ȸ�� ����
    public void Register(InputField email, InputField pw, InputField userName) {
        var request = new RegisterPlayFabUserRequest {
            Email = email.text,
            Password = pw.text,
            Username = userName.text
        };
        GameManager.ShowLoadingUI();
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    public void FindPassWordOnEmail(InputField email) {
        var request = new SendAccountRecoveryEmailRequest {
            Email = email.text,
            TitleId = PlayFabSettings.TitleId
        };
        GameManager.ShowLoadingUI();
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnSendAccountRecoveryEmailSuccess, OnSendAccountRecoveryEmailFail);
    }

    private void OnSendAccountRecoveryEmailSuccess(SendAccountRecoveryEmailResult result) {
        GameManager.CloseLoadingUI();
        OnFindSuccess?.Invoke("�̸��Ͽ� �����ؼ� Ȯ�����ּ���");
    }

    private void OnSendAccountRecoveryEmailFail(PlayFabError error) {
        GameManager.CloseLoadingUI();
        OnFindFailed?.Invoke("�ùٸ� �̸����� �ۼ����ּ���");
    }

    // �α��� ���� �ݹ�
    private void OnLoginSuccess(LoginResult result) {
        print("�α��� ����");
        GameManager.CloseLoadingUI();
        UI_Manager.Instance.ClosePopupUI();
        PlayFabID = result.PlayFabId;
        if (result.InfoResultPayload.PlayerProfile.DisplayName == null) {
            UI_Manager.Instance.ShowPopupUI<UI_PopUp>("NickNameCanvas");
        }
        else {
            NickName = result.InfoResultPayload.PlayerProfile.DisplayName;
            UI_Manager.Instance.ShowPopupUI<UI_PopUp>("ConnectCanvas");
        }
        DataManager.Instance.LoadData(result.PlayFabId);
    }

    // �α��� ���� �ݹ�
    private void OnLoginFailure(PlayFabError error) {
        GameManager.CloseLoadingUI();
        OnLoginFailed?.Invoke("�̸��� Ȥ�� ��й�ȣ�� ���� �ʽ��ϴ�.");
    }

    // ȸ������ ���� �ݹ�
    private void OnRegisterSuccess(RegisterPlayFabUserResult result) {
        print("ȸ�� ���� ����");
        GameManager.CloseLoadingUI();
        UI_Manager.Instance.ClosePopupUI();
        DataManager.Instance.SaveData();
    }

    // ȸ������ ���� �ݹ�
    private void OnRegisterFailure(PlayFabError error) {

        GameManager.CloseLoadingUI();

        string target = error.GenerateErrorReport();
        string result = "ȸ������ ����";

        if (target.Contains("Email address is not valid.")) result = "�̸��� ������ ������ּ���";
        else if (target.Contains("Password must be between 6 and 100 characters.")) result = "��� ��ȣ�� 6~100�� ���̷� ���ּ���";
        else if (target.Contains("Username must be between 3 and 20 characters.")) result = "���� �̸��� 3~20�� ���̷� ���ּ���";
        else if (target.Contains("Email address already exists. ")) result = "�ش� �̸��Ϸ� ���� ������ �̹� �����մϴ�.";

        OnRegisterFailed?.Invoke(result);
        print(target);

    }

    // �г��� ����
    public void UpdateDisplayName(InputField name) {
        GameManager.ShowLoadingUI();
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName = name.text
        }, result => {
            GameManager.CloseLoadingUI();
            NickName = result.DisplayName;
            Debug.Log("The player's display name is now: " + result.DisplayName);
            UI_Manager.Instance.ClosePopupUI();
            UI_Manager.Instance.ShowPopupUI<UI_PopUp>("ConnectCanvas");

        }, error => {
            GameManager.CloseLoadingUI();
            Debug.LogError(error.GenerateErrorReport()); 
        } );


    }

    // ����ǥ
    public void SendStatisticToServer(int score, string name) {
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = name,
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnStatisticUpdateSuccess, error => { print("��� ���� ����"); });
    }

    private void OnStatisticUpdateSuccess(UpdatePlayerStatisticsResult result) {
        print("���������� ����");
    }


    public void GetLeaderboard() {
        var request = new GetLeaderboardRequest {
            StatisticName = "WinScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        GameManager.ShowLoadingUI();
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGetSuccess, error => { GameManager.CloseLoadingUI(); print("�������� ��û ����"); } );
    }

    private void OnLeaderboardGetSuccess(GetLeaderboardResult result) {
        GameManager.CloseLoadingUI();
        OnRankGet?.Invoke(result.Leaderboard);
    }

    public void SetUserData(Dictionary<string, string> data, Action<UpdateUserDataResult> result = null) {

        if (!PlayFabClientAPI.IsClientLoggedIn()) return;

        var request = new UpdateUserDataRequest{ 
            Data = data, 
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, result, OnGetOrSetUserDataFailed);
    }

    public void GetUserData(string playfabId) {
        var request = new GetUserDataRequest() { PlayFabId = playfabId };
        GameManager.ShowLoadingUI();
        PlayFabClientAPI.GetUserData(request, (result) => OnGetUserDataSuccess(result), OnGetOrSetUserDataFailed);
    }

    private void OnGetUserDataSuccess(GetUserDataResult result) {

        GameManager.CloseLoadingUI();

        if (!PlayFabClientAPI.IsClientLoggedIn()) return;

        if (!result.Data.ContainsKey("UserData"))
            DataManager.Instance.SaveData();
        else {
            foreach (var eachData in result.Data) {
                string key = eachData.Key;

                if (key.Contains("UserData")) {
                    DataManager.Instance.userData = JsonUtility.FromJson<UserData>(eachData.Value.Value);
                }
            }
        }
    }

    private void OnGetOrSetUserDataFailed(PlayFabError error) {
        GameManager.CloseLoadingUI();
        Debug.LogError("error : " + error.GenerateErrorReport());
    }

}
