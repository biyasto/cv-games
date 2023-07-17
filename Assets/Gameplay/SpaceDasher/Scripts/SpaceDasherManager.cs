using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceDasherManager : MonoBehaviour
{
    [SerializeField] private ShipController ship;
    [SerializeField] private EnemyController enemy;
    [SerializeField] private FaceController face;
    [SerializeField] private Text pointUI;
    [SerializeField] private Text timerUI;
    private bool isGameOver;
    private bool isPause = true;
    private int points = 0;
    private int timer = 0;
    void Start()
    {
        isGameOver = false;
        ship.OnGameOver = GameOver;
    }
    
    void GameOver()
    {
        isGameOver = true;
        Debug.Log("restart game");
    }

    private float timerCDmax = 1f;
    private float timerCD = 1f;
    
    void Update()
    {
        
        timerCD -= Time.deltaTime;
        if (timerCD < 0)
        {
            timer++;
            timerCD = timerCDmax;
            points = timer/ 3;
            pointUI.text = "SCORE:" +points;
            timerUI.text = (timer/60>=10?(timer / 60).ToString():"0"+(timer / 60))+ ":" + (timer%60>=10?(timer % 60).ToString():"0"+(timer % 60));
        }
        
        if (isGameOver) return;
        
        face.UpdateFace();
        ship.newPosition = face.Position;
        ship.newRotation = new Vector3(0, 0, face.Rotation);
        ship.UpdatePosition();
        ship.UpdateRotation();
        enemy.UpdatePositionEnemyList();
        enemy.SpawnEnemyList();
        
        Debug.Log("Updated");
        
    }
}
