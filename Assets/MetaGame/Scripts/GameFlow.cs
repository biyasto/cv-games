using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
    public Action<List<string>> onLeaderboardLoaded;
    private string username = "admin";
    private string userEmail = "admin@gmail.com";
    private string userPassword = "@admin123";
    private string _playFabId;
    
    public static GameFlow Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        SetupAthenaApp();
        DontDestroyOnLoad(gameObject);
        //SceneManager.LoadScene("NeonSaberGameplay");
        SceneManager.LoadScene("Menu_Main");
    }

    void SetupAthenaApp()
    {
        
    }

    IEnumerator Start()
    {
        LoginPlayfab();
        
        //if (!_athenaApp.IsFirebaseReady)
        {
            //_objOverlay.SetActive(true);

           // while (!_athenaApp.IsFirebaseReady)
                yield return null;

            //_objOverlay.SetActive(false);
        }

        // Now you are safe to go
    }

    public void TrackEvent()
    {
      
    }

    public void TrackEventWithParameters()
    {
    }

    public void LoginPlayfab()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, On Login Email Success");
        //GetStats();
        _playFabId = result.PlayFabId;
        //AddLeaderboard("Neon-Saber",97);
        //AddLeaderboard("Space-Dasher",72);
        //AddLeaderboard("Lets-Dance",49);
        //GetLeaderboard("Neon-Saber");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
        RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = username };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Register success");
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.Log("Register failure");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void UpdateUserData(string keyData, string valueData)
    {
        /*PlayFabClientAPI.UpdateUserData(new GetLeaderboardRequest()
            
            {
                StatisticName = "Neon-Saber",
                MaxResultsCount = 4,
            },
            result => Debug.Log("Successfully updated user data"),
            error =>
            {
                Debug.Log("Got error setting user data Ancestor to Arthur");
                Debug.Log(error.GenerateErrorReport());
            });*/
    }
    public void GetLeaderboard(string gamename)
    {
        var request = new GetLeaderboardRequest();
        request.StartPosition = 0;
        request.StatisticName = gamename;
        request.MaxResultsCount = 4;
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardSuccess, e =>
        {
            print(e.Error);
        });
    }

    public List<string> highscores = new List<string>();
    private void OnLeaderboardSuccess(GetLeaderboardResult obj)
    {
        highscores.Clear();
        print("get lb success");
        foreach(var value in obj.Leaderboard)
        {
            if (value != null)
            {
                highscores.Add(value.StatValue.ToString());
            }
            else
            {
                highscores.Add("No highscore");
            }
            
           // print(value.StatValue);
            
        }
        onLeaderboardLoaded?.Invoke(highscores);
    }
    public void AddLeaderboard(string gamename, int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = gamename,
                    Value = score
                }
            }
        };
      
        
        PlayFabClientAPI.UpdatePlayerStatistics(request, s =>
        {
            print("Success");
        }, e =>
        {
            print(e.Error);
        });
    }
    
}