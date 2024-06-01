using PlayFab.ClientModels;
using PlayFab.EconomyModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : DontDestroySingleton<DataManager> {

    #region Notice UI delegate
    public delegate void OnNoticeDataGetHandler(string notice);
    #endregion

    #region Notice UI event
    public static event OnNoticeDataGetHandler OnNoticeDataGet;
    #endregion

    public UserData userData;
    public int mapIndex { get; set; }

    private string cryptoFilePath;

    [field: SerializeField] public ResourceSO Resource { get; private set; }

    public readonly string noticeInfoURL = "https://docs.google.com/document/d/1CdYdTts1HVUbZjSOrLS5o_bJDZnsnulGj1XcLPUsNto/export?format=txt";
    public readonly string sheetURL = "https://docs.google.com/spreadsheets/d/1T3KSbYhJyFOvNvwhQ4lzWU6QVlIGxxpKeEhmQgYuA10";

    public readonly string marbleDataRange = "A2:F";
    public readonly string marbleDataSheetId = "0";

    public readonly string commonMarbleDataRange = "A2:D";
    public readonly string commonMarbleDataSheetId = "1732538507";

    public readonly string runeDataRange = "A2:I";
    public readonly string runeDataSheetId = "2022585265";

    public readonly string mapDataRange = "A2:B";
    public readonly string mapDataSheetId = "431664827";

    public string NoticeString { get; private set; }

    private Dictionary<Type, string> sheetDatas = new Dictionary<Type, string>();

    private string jsonData;

    protected override void Awake() {
        base.Awake();
        sheetDatas.Add(typeof(MarbleData), GetTSVAddress(sheetURL, marbleDataRange, marbleDataSheetId));
        sheetDatas.Add(typeof(RuneData), GetTSVAddress(sheetURL, runeDataRange, runeDataSheetId));
        sheetDatas.Add(typeof(MapData), GetTSVAddress(sheetURL, mapDataRange, mapDataSheetId));
        sheetDatas.Add(typeof(CommonMarbleData), GetTSVAddress(sheetURL, commonMarbleDataRange, commonMarbleDataSheetId));
    }

    private void Start() {
        StartCoroutine(GetNoticeData());
        StartCoroutine(GetSheetData());

        Resource.runeSkills.ForEach(x => x.runeData = Resource.runes.Find(y => y.id + "_Skill" == x.name));
        Resource.basicRuneSkills.ForEach(x => x.runeData = Resource.basicRunes.Find(y => y.id + "_Skill" == x.name));

        userData = new UserData();

        cryptoFilePath = Application.persistentDataPath;
        Debug.Log(cryptoFilePath);
    }

    private IEnumerator GetSheetData() {
        List<Type> sheetTypes = new List<Type>(sheetDatas.Keys);

        foreach (Type type in sheetTypes) {
            UnityWebRequest www = UnityWebRequest.Get(sheetDatas[type]);
            yield return www.SendWebRequest();

            sheetDatas[type] = www.downloadHandler.text;

            if (type == typeof(MarbleData)) Resource.marbles = GetDatas<MarbleData>(sheetDatas[type]);
            if (type == typeof(RuneData)) Resource.runes = GetDatas<RuneData>(sheetDatas[type]).FindAll(x => x.Type == GlobalEnum.RuneType.Active);
            if (type == typeof(MapData)) Resource.maps = GetDatas<MapData>(sheetDatas[type]);
            if (type == typeof(CommonMarbleData)) Resource.commonMalbles = GetDatas<CommonMarbleData>(sheetDatas[type]);
            if (type == typeof(RuneData)) Resource.basicRunes = GetDatas<RuneData>(sheetDatas[type]).FindAll(x => x.Type == GlobalEnum.RuneType.Passive);
        }
    }

    public static string GetTSVAddress(string address, string range, string sheetID) {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

    private List<T> GetDatas<T>(string data) {
        List<T> returnList = new List<T>();
        string[] splitedData = data.Split('\n');

        foreach (string element in splitedData) {
            string[] datas = element.Split('\t');
            returnList.Add(GetData<T>(datas));
        }

        return returnList;
    }

    private List<T> GetDatasAsChildren<T>(string data) {
        List<T> returnList = new List<T>();
        string[] splitedData = data.Split('\n');

        foreach (string element in splitedData) {
            string[] datas = element.Split('\t');
            returnList.Add(GetData<T>(datas, datas[0]));
        }

        return returnList;
    }

    private T GetData<T>(string[] datas, string childType = "") {
        object data;

        if (string.IsNullOrEmpty(childType) || Type.GetType(childType) == null) {
            data = Activator.CreateInstance(typeof(T));
        }
        else {
            data = Activator.CreateInstance(Type.GetType(childType));
        }

        // 클래스에 있는 변수들을 순서대로 저장한 배열
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        for (int i = 0; i < datas.Length; i++) {
            try {
                // string > parse
                Type type = fields[i].FieldType;

                if (string.IsNullOrEmpty(datas[i])) continue;

                if (type == typeof(int))
                    fields[i].SetValue(data, int.Parse(datas[i]));

                else if (type == typeof(float))
                    fields[i].SetValue(data, float.Parse(datas[i]));

                else if (type == typeof(bool))
                    fields[i].SetValue(data, bool.Parse(datas[i]));

                else if (type == typeof(string))
                    fields[i].SetValue(data, datas[i]);

                else if (type == typeof(List<int>)) { 
                    List<int> list = new List<int>();
                    string[] splitedData = datas[i].Split(',');
                    foreach (string s in splitedData) list.Add(int.Parse(s));
                    fields[i].SetValue(data, list);
                }

                else
                    fields[i].SetValue(data, Enum.Parse(type, datas[i]));
            }

            catch (Exception e) {
                Debug.LogError($"SpreadSheet Error : {e.Message}");
            }
        }

        return (T)data;
    }

    public IEnumerator GetNoticeData() {
        using (UnityWebRequest www = UnityWebRequest.Get(noticeInfoURL)) {
            yield return www.SendWebRequest();

            if (www.isDone) NoticeString = www.downloadHandler.text;
        }
    }

    public void SaveData(Action<UpdateUserDataResult> result = null) {
        Dictionary<string, string> dataDic = new Dictionary<string, string>();
        dataDic.Add("UserData", JsonUtility.ToJson(userData, true));
        PlayFabManager.Instance.SetUserData(dataDic, result);
    }

    public void LoadData(string playfabId) {
        if (File.Exists(Path.Combine(cryptoFilePath, playfabId + "Local.json"))) LocalLoadData();
        else PlayFabManager.Instance.GetUserData(playfabId);
    }

    public void LocalSaveData() {

        jsonData = JsonUtility.ToJson(userData, true);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        byte[] encryptBytes = AES.Cipher(bytes, "1234", AES.mode.ENCRYPT);
        File.WriteAllBytes(Path.Combine(cryptoFilePath , PlayFabManager.Instance.PlayFabID + "Local.json"), encryptBytes);

    }

    public async void LocalLoadData() {
        GameManager.ShowLoadingUI();
        await AsyncLoad();
        GameManager.CloseLoadingUI();
    }

    private async Task AsyncLoad() {
        if (!File.Exists(Path.Combine(cryptoFilePath, PlayFabManager.Instance.PlayFabID + "Local.json"))) {
            Debug.Log("경로가 없습니다.");
            return;
        }

        await Task.Run(() => {
            byte[] bytes = File.ReadAllBytes(Path.Combine(cryptoFilePath, PlayFabManager.Instance.PlayFabID + "Local.json"));
            byte[] DecryptBytes = AES.Cipher(bytes, "1234", AES.mode.DECRYPT);
            jsonData = System.Text.Encoding.UTF8.GetString(DecryptBytes);
        });

        //// 오브젝트 디시리얼라이즈
        UserData data = JsonUtility.FromJson<UserData>(jsonData);
        userData = data;

        SaveData();

        File.Delete(Path.Combine(cryptoFilePath, PlayFabManager.Instance.PlayFabID + "Local.json"));

        PlayFabManager.Instance.SendStatisticToServer(userData.winScore, "WinScore");


    }
}
