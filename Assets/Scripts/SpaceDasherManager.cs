using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceDasherManager : MonoBehaviour
{
    [SerializeField] private ShipController ship;
    [SerializeField] private EnemyController enemy;
    
    private bool isGameOver;
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
    
    void Update()
    {
        
        if (isGameOver) return;
        ship.UpdatePosition();
        ship.UpdateRotation();
        enemy.UpdatePositionEnemyList();
        Debug.Log("Updated");
        
    }
}
