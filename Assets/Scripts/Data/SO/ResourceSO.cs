using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class MarbleData {

    public enum MarbleType {
        TA, DD, AD, AP
    }

    public string id;
    public string name;
    public MarbleType Type;
    public string explain;
    public int hp;
    public int damage;
}

[System.Serializable]
public class RuneData {

    public string id;
    public string name;
    public int value;
    public int percent;
    public int count;
    public string explain;

}

[System.Serializable]
public class MapData {
    public string id;
    public string name;
}


[CreateAssetMenu(fileName = "ResourceSO", menuName = "Scriptable Object/ResourceSO")]
public class ResourceSO : ScriptableObject
{
    public List<MarbleData> marbles;
    public List<RuneData> runes;
    public List<MapData> maps;


    public List<Sprite> marbleImages;
    public List<Sprite> runeImages;
    public List<Sprite> mapImages;

#if UNITY_EDITOR
    [SerializeField] private string marbleImagePath;
    [SerializeField] private string runeImagePath;
    [SerializeField] private string mapImagePath;

    [ContextMenu("Load Marble Images from Custom Path")]
    private void LoadMarbleImagesFromCustomPath() {
        string path = marbleImagePath;
        marbleImages = LoadSpritesFromPath(path);
        Debug.Log("Marble images loaded from custom path. Count: " + marbleImages.Count);
    }

    [ContextMenu("Load Rune Images from Custom Path")]
    private void LoadRuneImagesFromCustomPath() {
        string path = runeImagePath;
        runeImages = LoadSpritesFromPath(path);
        Debug.Log("Rune images loaded from custom path. Count: " + runeImages.Count);
    }

    [ContextMenu("Load Map Images from Custom Path")]
    private void LoadMapImagesFromCustomPath() {
        string path = mapImagePath;
        mapImages = LoadSpritesFromPath(path);
        Debug.Log("Map images loaded from custom path. Count: " + mapImages.Count);
    }

    private List<Sprite> LoadSpritesFromPath(string path) {
        List<Sprite> sprites = new List<Sprite>();
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { path });

        foreach (string guid in guids) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null) {
                sprites.Add(sprite);
            }
        }

        return sprites;
    }

    [ContextMenu("Append '_image' to Image Resource")]
    private void AppendImageSuffixToMapImages() {
        if (runeImages == null || runeImages.Count == 0) {
            Debug.LogWarning("No map images found to rename.");
            return;
        }

        foreach (var sprite in runeImages) {
            if (sprite != null) {
                string assetPath = AssetDatabase.GetAssetPath(sprite);
                string newName = sprite.name + "_image";

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


