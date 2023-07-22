using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpaceDasherManager : MonoBehaviour
{
    [SerializeField] private string gameName;
    [SerializeField] private string MenuGameName;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject OverMenu;
    [SerializeField] private ShipController ship;
    [SerializeField] private EnemyController enemy;
    [SerializeField] private FaceController face;
    [SerializeField] private Text pointUI;
    [SerializeField] private Text timerUI;
    
    public UDPReceive udpReceive_sign;
    private bool isGameOver;
    private bool isPause = false;
    private bool isStarted = false;
    private int points = 0;
    private int timer = 0;
    void Start()
    {
        isGameOver = false;
        ship.OnGameOver = GameOver;
        udpReceive_sign = GameObject.Find("UDP5055").GetComponent<UDPReceive>();
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
    
    private float okDelay = 1f;
    private float okCD = 0f;
    private float cancelDelay = 1f;
    private float cancleCD = 0f;
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
            face.ReadyFaceCheck();
            isStarted = face.isStarted;
            return;
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
            points = timer/ 3;
            pointUI.text = "SCORE:" +points;
            timerUI.text = (timer/60>=10?(timer / 60).ToString():"0"+(timer / 60))+ ":" + (timer%60>=10?(timer % 60).ToString():"0"+(timer % 60));
        }
        
       
        
        face.UpdateFace();
        ship.newPosition = face.Position;
        ship.newRotation = new Vector3(0, 0, face.Rotation);
        ship.UpdatePosition();
        ship.UpdateRotation();
        enemy.UpdatePositionEnemyList();
        enemy.SpawnEnemyList();
        
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
