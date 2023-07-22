using System.Collections;
using System.Collections.Generic;
using Gameplay.NeonSaber.Scripts;
using Gameplay.SpaceDasher.Scripts;
using UnityEngine;

public class FoodsController : MonoBehaviour
{
    [SerializeField] public List<GameObject> enemyList;
    private float timerCDmax = 2f;
    private float timerCD = 1f;
    private float timeScale = 1f;
    public void UpdatePositionEnemyList()
    {
        foreach (var enemy in enemyList)
        {
           
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.GetComponent<FoodData>().des, Time.deltaTime*5f);
                Debug.Log("move");
        }

     
        
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i].transform.position == enemyList[i].GetComponent<FoodData>().des || enemyList[i].GetComponent<FoodData>().type ==-1)
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
            var index= Random.Range(0, 7);
            timerCD = timerCDmax/timeScale;
            GameObject enemy;
            switch (index)
            {
                case 0: enemy = Instantiate(Resources.Load("Prefabs/Green1", typeof(GameObject))) as GameObject; break;
                case 1: enemy = Instantiate(Resources.Load("Prefabs/Green2", typeof(GameObject))) as GameObject;break;
                case 2:  enemy = Instantiate(Resources.Load("Prefabs/Green3", typeof(GameObject))) as GameObject;break;
                case 3:enemy = Instantiate(Resources.Load("Prefabs/Red1", typeof(GameObject))) as GameObject; break;
                case 4:enemy = Instantiate(Resources.Load("Prefabs/Red2", typeof(GameObject))) as GameObject; break;
                case 5: enemy = Instantiate(Resources.Load("Prefabs/Red3", typeof(GameObject))) as GameObject;break;
                case 6: enemy = Instantiate(Resources.Load("Prefabs/Danger", typeof(GameObject))) as GameObject;break;
                default:
                    enemy = Instantiate(Resources.Load("Prefabs/Green1", typeof(GameObject))) as GameObject; break;
            }

            if (index < 3)
            {
                enemy.GetComponent<FoodData>().type = 0;
            }
            else if (index < 6)
            {
                enemy.GetComponent<FoodData>().type = 1;
            }
            else
            {
                enemy.GetComponent<FoodData>().type = 2;
            }
            
            enemy.transform.SetParent(this.transform);
            enemy.transform.position = new Vector3(Random.Range(-10f,10f), Random.Range(-7f,7f), 30);
            enemy.transform.localEulerAngles =new Vector3(0,180, Random.Range(-180, 180));
            enemy.GetComponent<FoodData>().des = new Vector3(Random.Range(-1.25f, 1.25f), Random.Range(-0.6f, 0.6f), 2.5f);
            enemyList.Add(enemy);
        }
    }
}
