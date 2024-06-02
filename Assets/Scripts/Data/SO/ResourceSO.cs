using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class MarbleData {

    public string id;
    public string name;
    public GlobalEnum.MarbleType Type;
    public string explain;
    public int additionalHp;
    public int additionalDamage;
    public string originalRuneID;
    public int gold;
}

[System.Serializable]
public class CommonMarbleData {

    public GlobalEnum.MarbleType Type;
    public int basicHp;
    public int basicDamage;
    public float basicMass;
    public string typeRuneID;
}

[System.Serializable]
public class RuneData {

    public string id;
    public GlobalEnum.BaseRuneType BaseType;
    public GlobalEnum.RuneType RuneType;
    public string name;
    public int cost;
    public List<int> value;
    public int count;
    public int boundCount;
    public int maxHasCount;
    public string explain;
    public int gold;

}

[System.Serializable]
public class MapData {
    public string id;
    public string name;
}


[CreateAssetMenu(fileName = "ResourceSO", menuName = "Scriptable Object/ResourceSO")]
public class ResourceSO : ScriptableObject
{
    [Header("인게임 DB")]
    public List<MarbleData> marbles;
    public List<RuneData> runes;
    public List<RuneData> basicRunes;
    public List<MapData> maps;
    public List<CommonMarbleData> commonMalbles;

    [Header("아트 리소스 DB")]
    public List<Sprite> marbleImages;
    public List<Sprite> runeImages;
    public List<Sprite> mapImages;

    [Header("스킬 리소스 DB")]
    public List<BaseRuneSkill> runeSkills;
    public List<BaseRuneSkill> basicRuneSkills;

#if UNITY_EDITOR
    [SerializeField] private string marbleImagePath;
    [SerializeField] private string runeImagePath;
    [SerializeField] private string mapImagePath;
    [SerializeField] private string runeSkillsPath;
    [SerializeField] private string basicRuneSkillsPath;

    [ContextMenu("Load Marble Images from Custom Path")]
    private void LoadMarbleImagesFromCustomPath() {
        string path = marbleImagePath;
        marbleImages = LoadAssetsFromPath<Sprite>(path);
        Debug.Log("Marble images loaded from custom path. Count: " + marbleImages.Count);
    }

    [ContextMenu("Load Rune Images from Custom Path")]
    private void LoadRuneImagesFromCustomPath() {
        string path = runeImagePath;
        runeImages = LoadAssetsFromPath<Sprite>(path);
        Debug.Log("Rune images loaded from custom path. Count: " + runeImages.Count);
    }

    [ContextMenu("Load Map Images from Custom Path")]
    private void LoadMapImagesFromCustomPath() {
        string path = mapImagePath;
        mapImages = LoadAssetsFromPath<Sprite>(path);
        Debug.Log("Map images loaded from custom path. Count: " + mapImages.Count);
    }

    [ContextMenu("Load Rune Skills from Custom Path")]
    private void LoadRuneSkillsFromCustomPath() {
        string path = runeSkillsPath;
        runeSkills = LoadAssetsFromPath<BaseRuneSkill>(path);
        Debug.Log("Map images loaded from custom path. Count: " + runeSkills.Count);
    }

    [ContextMenu("Load Basic Rune Skills from Custom Path")]
    private void LoadBasicRuneSkillsFromCustomPath() {
        string path = basicRuneSkillsPath;
        basicRuneSkills = LoadAssetsFromPath<BaseRuneSkill>(path);
        Debug.Log("Map images loaded from custom path. Count: " + basicRuneSkills.Count);
    }

    private List<T> LoadAssetsFromPath<T>(string path) where T : UnityEngine.Object {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });

        foreach (string guid in guids) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null) {
                assets.Add(asset);
            }
        }

        return assets;
    }

    [ContextMenu("Append '_Image' to Image Resource")]
    private void AppendImageSuffixToMapImages() {
        if (runeImages == null || runeImages.Count == 0) {
            Debug.LogWarning("No map images found to rename.");
            return;
        }

        foreach (var sprite in runeImages) {
            if (sprite != null) {
                string assetPath = AssetDatabase.GetAssetPath(sprite);
                string newName = sprite.name + "_Image";

                // Ensure the new name doesn't already exist to avoid duplicates
                if (!AssetDatabase.LoadAssetAtPath<Sprite>(assetPath.Replace(sprite.name, newName))) {
                    RenameAsset(assetPath, newName);
                }
                else {
                    Debug.LogWarning($"Asset with name {newName} already exists.");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void RenameAsset(string assetPath, string newName) {
        string errorMessage = AssetDatabase.RenameAsset(assetPath, newName);
        if (!string.IsNullOrEmpty(errorMessage)) {
            Debug.LogError($"Failed to rename asset at {assetPath} to {newName}: {errorMessage}");
        }
        else {
            Debug.Log($"Renamed asset at {assetPath} to {newName}");
        }
    }
#endif

}


