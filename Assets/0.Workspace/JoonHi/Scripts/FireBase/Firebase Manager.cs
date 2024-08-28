using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google.MiniJSON;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : Singleton<FirebaseManager>
{
    private static FirebaseApp app;
    public static FirebaseApp App { get { return app; } }

    private static FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return auth; } }

    private static FirebaseDatabase db;
    public static FirebaseDatabase DB { get { return db; } }

    private static bool isValid;
    public static bool IsValid { get { return isValid; } }

    protected override void Awake()
    {
        base.Awake();
        CheckDependency();
        if(auth != null)
            auth.SignOut();
    }

    private async void CheckDependency()
    {
        DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
            app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseDatabase.DefaultInstance;

            // Set a flag here to indicate whether Firebase is ready to use by your app.
            Debug.Log("Firebase Check and FixDependencies success");
            isValid = true;
        }
        else
        {
            // Firebase Unity SDK is not safe to use here.
            Debug.LogError("Firebase Check and FixDependencies fail");
            isValid = false;

            app = null;
            auth = null;
            db = null;
        }
    }


    

    const string REFPATH = "Userdata";
    /// <summary>
    /// UserData 받아오는 비동기 메서드 : 24.08.12 PJH
    /// </summary>
    /// <returns></returns>
    public async Task<Userdata> LoadDataAsync()
    {
        Userdata userData = null;
        string userID = Auth.CurrentUser.UserId;

        DataSnapshot snapshot = await DB
            .GetReference("REFPATH")
            .Child(userID)
            .GetValueAsync();

        if (snapshot.Exists)
        {
            string json = snapshot.GetRawJsonValue();
            userData = JsonUtility.FromJson<Userdata>(json);
        }
        else
        {
            Debug.Log("No data found");
        }

        return userData;
    }


    /*
    public Userdata LoadData()
    {
        Userdata userData = new Userdata();
        string userID = auth.CurrentUser.UserId;

        DataSnapshot snapshot = FetchUserData(userID).Result;
        string json = snapshot.GetRawJsonValue();
        userData = JsonUtility.FromJson<Userdata>(json);
        Debug.Log($"Load {userData.nickName}");
        return userData;
    }

    public async Task<DataSnapshot> FetchUserData(string userID)
    {
        var task = FirebaseManager.DB
            .GetReference("Userdata")
            .Child(userID)
            .GetValueAsync();

        await task;

        if (task.IsCompletedSuccessfully)
        {
            return task.Result;
        }
        else
        {
            Debug.LogError("Failed to fetch user data: " + task.Exception);
            return null;
        }
    }
    */
    public bool SaveData(Userdata newUserData)
    {
        bool result = false;

        Debug.Log("Saving User Data");
        string userId = auth.CurrentUser.UserId;
        string json = JsonUtility.ToJson(newUserData);

        FirebaseManager.DB
            .GetReference("REFPATH")
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
