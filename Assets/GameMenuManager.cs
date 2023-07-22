using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    [SerializeField] private Button guide;
    [SerializeField] private Button play;
    [SerializeField] private Button score;
    [SerializeField] private Button back;
    [SerializeField] private string gamescene;
    [SerializeField] private string gamesName;
     private UDPReceive udpReceive_pos;
    private UDPReceive udpReceive_sign;
    public int tabIndex = 0;
    private List<string> scores = new List<string>();
    [SerializeField]private List<Text> scoresText;
    private void Start()
    {
        guide.onClick.Invoke();
        udpReceive_pos = GameObject.Find("UDP5056").GetComponent<UDPReceive>();
        udpReceive_sign = GameObject.Find("UDP5055").GetComponent<UDPReceive>();
        LoadLeaderboard();
        GameFlow.Instance.onLeaderboardLoaded += UpdateLeaderboard;
    }

    public void openTab()
    {
        switch (tabIndex)
        {
            case 0: guide.onClick.Invoke();
                break;
            case 1: play.onClick.Invoke();
                break;
            case 2:score.onClick.Invoke();
                break;
            case 3: back.onClick.Invoke();
                break;
        }
    }

    public void Changeindex(int change)
    {
        tabIndex += change;
        if (tabIndex > 3) tabIndex = 3;
        if (tabIndex < 0) tabIndex = 0;
    }
    public void Setindex(int index)
    {
        tabIndex = index;
        if (tabIndex > 3) tabIndex = 0;
        if (tabIndex < 0) tabIndex = 3;
    }
    public void BackMainMenu()
    {
        SceneManager.LoadScene("Menu_Main");
    }
    public void GogamePlay()
    {
        SceneManager.LoadScene(gamescene);
    }

    private float DelayInput = 1f;
    private float playCounter= 0f;
    private float backCounter= 0f;
    
    private string posdata = "[0,0]";
    private Vector2 posCheckPoint = new Vector2(0, 0);
    private bool isPosChecked = false;
    private float swipeCD = 0.5f;
    
    private float okDelay = 1f;
    private float okCD = 0f;
    private float cancelDelay = 1f;
    private float cancleCD = 0f;

    private void Update()
    {
        if (swipeCD > 0) swipeCD -= Time.deltaTime;
        if (udpReceive_sign.data == "['2']")
        {
            if (swipeCD > 0) return;
            
            posdata = udpReceive_pos.data;
            if (posdata == null) return;
            try
            {
                posdata = posdata.Remove(0, 1);
                posdata = posdata.Remove((posdata.Length - 1), 1);
            }
            catch
            {
                return;
            }

            string[] pos = posdata.Split(',');

            if (!isPosChecked)
            {
                float x = float.Parse(pos[0]);
                float y = float.Parse(pos[1]);
                isPosChecked = true;
                posCheckPoint = new Vector2(x, y);
            }
            else
            {
                float x = float.Parse(pos[0]);
                float y = float.Parse(pos[1]);

                if (y > posCheckPoint.y + 50f)
                {
                    Changeindex(+1);
                    ResetPos();
                    swipeCD = 0.25f;
                    openTab();
                }
                if (y < posCheckPoint.y - 50f) 
                {
                    Changeindex(-1);
                    ResetPos();
                    swipeCD = 0.25f;
                    openTab();
                }
            }
        }
        else
        {
            ResetPos();
        }
        
        
        if (tabIndex == 1 )  //&& play tab
        {
            if (udpReceive_sign.data == "['1']")
            {
                if (okCD < okDelay)
                {
                    okCD += Time.deltaTime;
                }

               GogamePlay();
            }
        }

        if (tabIndex == 3) // back tab
        
        {
            if (udpReceive_sign.data == "['3']")
            {
                if (cancleCD < cancelDelay)
                {
                    cancleCD += Time.deltaTime;
                }
                BackMainMenu();
            }
        }
    }
    void ResetPos()
    {
         posdata = "[0,0]";
         posCheckPoint = new Vector2(0, 0);
         isPosChecked = false;
    }

    void LoadLeaderboard()
    {
        GameFlow.Instance.GetLeaderboard(gamesName);
    }

    void UpdateLeaderboard(List<string> highscores)
    {
        scores.Clear();
        foreach (var score in highscores)
        {
          Debug.Log(score);
        scores.Add(score);
        }
        Debug.Log("Here:--->"+highscores.Count);
        while (scores.Count<scoresText.Count)
        {
            scores.Add("No");
        }

        for (int i = 0; i < scoresText.Count; i++)
        {
            scoresText[i].text = scores[i]+" points";
        }
    }

    private void OnDestroy()
    {
        GameFlow.Instance.onLeaderboardLoaded -= UpdateLeaderboard;
    }
}
