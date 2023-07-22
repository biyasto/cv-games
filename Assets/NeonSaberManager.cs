using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NeonSaberManager : MonoBehaviour
{
    [SerializeField] private string gameName;
    [SerializeField] private string MenuGameName;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject OverMenu;
    
    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private SaberController Red;
    [SerializeField] private SaberController Green;
    [SerializeField] private HandController saber;
    [SerializeField] private FoodsController foods;
    [SerializeField] private Text pointUI;
    [SerializeField] private Text timerUI;
    public UDPReceive udpReceive_sign;
    private bool isPause = false;
    private bool isStarted = false;
    private bool isGameOver;
    private int timer = 0;
    private int points = 0;
    void Start()
    {
        udpReceive_sign = GameObject.Find("UDP5053").GetComponent<UDPReceive>();
        isGameOver = false;
        Red.onHit = AddScore;
        Green.onHit = AddScore;
        Red.onGameOver = GameOver;
        Green.onGameOver = GameOver;
        
    }

    private float okDelay = 1f;
    private float okCD = 0f;
    private float cancelDelay = 1f;
    private float cancleCD = 0f;
    void AddScore()
    {
        Debug.Log("AAAAAAAAAAAAAAAAA");
        points++;
        pointUI.text = "SCORE:" +points;
    }
    void GameOver()
    {
        isGameOver = true;
        Debug.Log("game over");
        OverMenu.SetActive(true);
        GameFlow.Instance.AddLeaderboard(gameName,points);
    }
    private float timerCDmax = 1f;
    private float timerCD = 1f;
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameOver();
        }
        
        
        if (!isStarted)
        {
            saber.ReadyFaceCheck();
            isStarted = saber.isStarted;
        }
        if (isGameOver)
        {
            if (udpReceive_sign.data == "['3']")
            {
                if (cancleCD < cancelDelay)
                {
                    cancleCD += Time.deltaTime;
                }
                BackToMenu();
            }
            else
            {
                cancleCD = 0;
            }
            if (udpReceive_sign.data == "['1']")
            {
                if (okCD < okDelay)
                {
                    okCD += Time.deltaTime;
                }

                ReloadGame();
            }
            else
            {
                okCD = 0;
            }
            
            return;
        }

        if (isPause)
        {
            if (udpReceive_sign.data == "['3']")
            {
                if (cancleCD < cancelDelay)
                {
                    cancleCD += Time.deltaTime;
                }
                BackToMenu();
            }
            else
            {
                cancleCD = 0;
            }
            if (udpReceive_sign.data == "['1']")
            {
                if (okCD < okDelay)
                {
                    okCD += Time.deltaTime;
                }

                ContinueGame();
            }
            else
            {
                okCD = 0;
            }
            
            return;
        }
        timerCD -= Time.deltaTime;
        if (timerCD < 0)
        {
            timer++;
            timerCD = timerCDmax;
            timerUI.text = (timer/60>=10?(timer / 60).ToString():"0"+(timer / 60))+ ":" + (timer%60>=10?(timer % 60).ToString():"0"+(timer % 60));
        }

        
        
        saber.UpdateFace();
        Red.newPosition = saber.Position0;
        Green.newPosition = saber.Position1;
        Red.newRotation = new Vector3(0, 0, saber.Rotation0);
        Green.newRotation = new Vector3(0, 0, saber.Rotation1);
        Red.UpdatePosition();
        Red.UpdateRotation();
        Green.UpdatePosition();
        Green.UpdateRotation();
        
        foods.SpawnEnemyList();
        foods.UpdatePositionEnemyList();
        Debug.Log("Updated");
    }
    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        isPause = true;
    }

    public void ContinueGame()
    {
        PauseMenu.SetActive(false);
        isPause = false;
    }

    public void ReloadGame()
    {
        GameFlow.Instance.AddLeaderboard(gameName,points);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void BackToMenu()
    {
        GameFlow.Instance.AddLeaderboard(gameName,points);
        SceneManager.LoadScene(MenuGameName);
    }
}