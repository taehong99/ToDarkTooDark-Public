using Firebase.Database;
using Firebase.Extensions;
using Google.MiniJSON;
using JH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseModifire : MonoBehaviour
{
    public Userdata LoadData()
    {
        Userdata userData = new Userdata();
        string userID = FirebaseManager.Auth.CurrentUser.UserId;
        FirebaseManager.DB
            .GetReference("Userdata")
            .Child(userID)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("취소");
                    userData = null;
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("오류");
                    userData = null;
                    return;
                }
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();

                userData = JsonUtility.FromJson<Userdata>(json);
            }
            );
        return userData;
    }

    public bool SaveData(Userdata newUserData)
    {
        bool result = false;

        Debug.Log("Saving User Data");
        string userId = FirebaseManager.Auth.CurrentUser.UserId;
        string json = JsonUtility.ToJson(newUserData);

        FirebaseManager.DB
            .GetReference("Userdata")
            .Child(userId)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }
                else if (task.IsFaulted)
                {
                    return;
                }

                result = true;
            }
            );
        Debug.Log("Save Data Success!");

        return result;
    }
}
