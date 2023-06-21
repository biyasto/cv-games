using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public List<GameObject> enemyList;
    public void UpdatePositionEnemyList()
    {
        foreach (var enemy in enemyList)
        {
           // if (enemy.transform.position.z < -8)
            {
                enemy.transform.position = Vector3.Lerp(enemy.transform.position, enemy.transform.position+Vector3.back, Time.deltaTime*50);
            }
            Debug.Log("enemy");
        }
    }
}
