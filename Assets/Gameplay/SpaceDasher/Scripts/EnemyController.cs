using System.Collections;
using System.Collections.Generic;
using Gameplay.SpaceDasher.Scripts;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public List<GameObject> enemyList;
    private float timerCDmax = 0.75f;
    private float timerCD = 1f;
    private float timeScale = 1f;
    public void UpdatePositionEnemyList()
    {
        foreach (var enemy in enemyList)
        {
           
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.GetComponent<EnemyData>().des, Time.deltaTime*20);
                Debug.Log("move");
        }

     
        
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i].transform.position == enemyList[i].GetComponent<EnemyData>().des)
            {
                var enemy = enemyList[i];
                enemyList.RemoveAt(i);
                Destroy(enemy);
            }
            
        }
       
         
        }
       
        
    
    public void SpawnEnemyList()
    {

        timeScale += Time.deltaTime / 100;
        timerCD -= Time.deltaTime;
        if (timerCD < 0)
        {
            timerCD = timerCDmax/timeScale;
            GameObject enemy = Instantiate(Resources.Load("Prefabs/Enemy", typeof(GameObject))) as GameObject;
            enemy.transform.SetParent(this.transform);
            enemy.transform.position = new Vector3(Random.Range(-30,30), Random.Range(0,15), 40);
            enemy.transform.localEulerAngles =new Vector3(0,180, Random.Range(-180, 180));
            enemy.GetComponent<EnemyData>().des = new Vector3(Random.Range(-3, 4), Random.Range(0, 4), -11);
            enemyList.Add(enemy);
        }
    }
}
