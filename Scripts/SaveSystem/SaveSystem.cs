using Game.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : Singleton<SaveSystem>
{
    public DataList Data = new DataList();

    [SerializeField] private bool reWriteAlways = false;
    [SerializeField] private string fileName = "PlayerData.txt";
    private string path;

    private void Awake()
    {
        Inicialize();
        if (Application.platform == RuntimePlatform.Android)
            path = Application.persistentDataPath + "/" + fileName;
        else
            path = Application.dataPath + "/" + fileName;
    }

    void Start()
    {
        Debug.Log(path + " Exists: " + File.Exists(path));

        if (reWriteAlways)
            SaveData();

        if (!File.Exists(path))
            SaveData();
        else
            LoadData();
    }


    public bool TryGetAccessLevel(int tom, int lvl)
    {
        string nameD = tom + "_" + lvl;

        string info;
        if (Data.TryGetDataToName(nameD, out info))
        {
            if (info.Equals("0"))
                return false;

            return true;
        }

        return false;

    }

    public bool TrySetAccessLevel(int tom, int lvl, bool access)
    {
        string nameD = tom + "_" + lvl;
        var accessBolean = access ? "1" : "0";

        if (Data.TryRewriteData(nameD, accessBolean))
        {
            SaveData();
            return true;
        }

        return false;
    }

    public bool TryRewriteData(string name, string dataInfo) => Data.TryRewriteData(name, dataInfo);

    public bool TryGetDataToName(string name, out string dataInfo) => Data.TryGetDataToName(name, out dataInfo);

    public void SaveData()
    {
        string dataToJson = JsonUtility.ToJson(Data, true);
        File.WriteAllText(path, dataToJson);
    }

    private void LoadData()
    {
        string dataToJson = JsonUtility.ToJson(Data, true);
        dataToJson = File.ReadAllText(path);
        Data = JsonUtility.FromJson<DataList>(dataToJson);
    }



    [Serializable]
    public class DataList
    {
        [SerializeField] public PlayerData[] data;

        public bool TryRewriteData(string name, string dataInfo)
        {
            PlayerData playerData;
            if (TryFindPlayerData(name, out playerData))
            {
                playerData.data = dataInfo;
                return true;
            }

            return false;
        }

        public bool TryGetDataToName(string name, out string dataInfo)
        {
            PlayerData playerData;
            if (TryFindPlayerData(name, out playerData))
            {
                dataInfo = playerData.data;
                return true;
            }

            dataInfo = "";
            return false;
        }

        private bool TryFindPlayerData(string name, out PlayerData playerData)
        {
            if (data != null && data.Length > 0)
            {
                foreach (var element in data)
                {
                    if (element.nameData.Equals(name))
                    {
                        playerData = element;
                        return true;
                    }
                }
            }

            playerData = null;
            return false;
        }
    }

    [Serializable]
    public class PlayerData
    {
        [SerializeField] public string nameData;
        [SerializeField] public string data;
    }
}
