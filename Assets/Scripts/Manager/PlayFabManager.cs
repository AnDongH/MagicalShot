using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PlayFabManager : DontDestroySingleton<PlayFabManager> {

    #region rank UI delegate
    public delegate void OnRankGetHandler(List<PlayerLeaderboardEntry> playerList);
    #endregion

    #region rank UI event
    public static event OnRankGetHandler OnRankGet;
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
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }


    // �α��� ���� �ݹ�
    private void OnLoginSuccess(LoginResult result) {
        print("�α��� ����");
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
    private void OnLoginFailure(PlayFabError error) => print("�α��� ����");

    // ȸ������ ���� �ݹ�
    private void OnRegisterSuccess(RegisterPlayFabUserResult result) {
        print("ȸ�� ���� ����");
        UI_Manager.Instance.ClosePopupUI();
        DataManager.Instance.SaveData();
    }

    // ȸ������ ���� �ݹ�
    private void OnRegisterFailure(PlayFabError error) => print("ȸ������ ����");

    // �г��� ����
    public void UpdateDisplayName(InputField name) {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName = name.text
        }, result => {
            NickName = result.DisplayName;
            Debug.Log("The player's display name is now: " + result.DisplayName);
            UI_Manager.Instance.ClosePopupUI();
            UI_Manager.Instance.ShowPopupUI<UI_PopUp>("ConnectCanvas");

        }, error => Debug.LogError(error.GenerateErrorReport()));
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
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGetSuccess, error => { print("�������� ��û ����"); } );
    }

    private void OnLeaderboardGetSuccess(GetLeaderboardResult result) {
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
        PlayFabClientAPI.GetUserData(request, (result) => OnGetUserDataSuccess(result), OnGetOrSetUserDataFailed);
    }

    private void OnGetUserDataSuccess(GetUserDataResult result) {

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

    private void OnGetOrSetUserDataFailed(PlayFabError error) => Debug.LogError("error : " + error.GenerateErrorReport());

}
