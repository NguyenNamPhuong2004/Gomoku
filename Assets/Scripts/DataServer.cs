using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;

[Serializable]
public class DataToSave
{
    public string userName;
    public int elo;
}
public class DataServer : MonoBehaviour
{
    public static DataServer ins;
    public DataToSave dts;
    public string userId;
    DatabaseReference dbRef;

    private void Awake()
    {
        ins = this;
        DontDestroyOnLoad(ins);
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveDataFn(int elo, string id)
    {
        userId = id;
        dts.elo = elo;
        string json = JsonUtility.ToJson(dts);
        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    public void LoadDataFn(string id)
    {
        userId = id;
        StartCoroutine(LoadDataEnum());
    }

    IEnumerator LoadDataEnum()
    {
        var serverData = dbRef.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        Debug.Log("process is complete");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            Debug.Log("server data found");
            dts = JsonUtility.FromJson<DataToSave>(jsonData);
        }
        else
        {
            print("no data found");
        }

    }
}