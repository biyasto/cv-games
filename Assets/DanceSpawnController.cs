using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.SpaceDasher.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class DanceSpawnController : MonoBehaviour
{
    [SerializeField] public List<GameObject> enemyList;
    [SerializeField] public List<GameObject> curList;
    private float timerCDmax = 5f;
    private float timerCD = 5f;
    private float timeScale = 1f;
    private string curMove="none";
    public int points;
    public int hp = 5;
    public int combo = 0;

    public void UpdatePositionEnemyList()
    {
        foreach (var enemy in enemyList)
        {

            enemy.transform.localPosition = new Vector3(enemy.transform.localPosition.x,
                enemy.transform.localPosition.y - timeScale*100 * Time.deltaTime, enemy.transform.localPosition.z);
            Debug.Log("move");
        }

     
        
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i].transform.localPosition.y <= -185 &&
                enemyList[i].GetComponent<DanceData>().danceName == curMove)
            {
                points++;
                points += combo;
                combo++;
                var enemy = enemyList[i];
                enemyList.RemoveAt(i);
                Destroy(enemy);
                return;
            }
            if (enemyList[i].transform.localPosition.y<= -325)
            {
                var enemy = enemyList[i];
                enemyList.RemoveAt(i);
                Destroy(enemy);
                hp--;
                combo = 0;
            }
            
        }
       
         
    }
       
        
    
   public void SpawnEnemyList()
    {

        timeScale += Time.deltaTime / 100;
        timerCD -= Time.deltaTime;
        if (timerCD < 0)
        {
            var index= Random.Range(0, 3);
            timerCD = timerCDmax/timeScale;
            GameObject enemy;
            switch (index)
            {
                case 0: enemy = Instantiate(Resources.Load("Prefabs/Rise", typeof(GameObject))) as GameObject; break;
                case 1: enemy = Instantiate(Resources.Load("Prefabs/TPose", typeof(GameObject))) as GameObject;break;
                case 2:  enemy = Instantiate(Resources.Load("Prefabs/Tree", typeof(GameObject))) as GameObject;break;
                case 3:enemy = Instantiate(Resources.Load("Prefabs/Warrior", typeof(GameObject))) as GameObject; break;
                default:
                    enemy = Instantiate(Resources.Load("Prefabs/Green1", typeof(GameObject))) as GameObject; break;
            }

        
            
            enemy.transform.SetParent(this.transform);
            enemy.transform.localPosition = new Vector3(Random.Range(-290f,290f), 650, 0);
            enemyList.Add(enemy);
        }
    }

   public void UpdateCurrentDance(string cur)
   {
       if (cur == null) cur = "none";
       Debug.Log(cur);
       curMove = cur;
       foreach (var dance in curList)
       {
           if (dance.GetComponent<DanceData>().danceName == cur)
           {
               dance.gameObject.SetActive(true);
           }
           else
           {
               dance.gameObject.SetActive(false);
           }
       }
   }
}

