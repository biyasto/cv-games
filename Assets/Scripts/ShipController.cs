using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    
    [SerializeField]private ShipColliderController shipColl;
    public Action OnGameOver;
    
    private Vector3 newPosition;
    private Quaternion newRotation;
   
    private void Awake()
    {
       
        newPosition = transform.position;
        newRotation = transform.rotation;
        shipColl.OnShipHit = ShipHit;
    }

    public void UpdatePosition()
    {
        var curPos = transform.position;
        transform.position = Vector3.Lerp(curPos,newPosition,2f);
    }

    public void UpdateRotation()
    {
        var curRotation = transform.rotation;
        transform.rotation = Quaternion.Lerp(curRotation, newRotation, 60f);
    }
    

    void ShipHit()
    {
        Debug.Log("call Game Over");
        OnGameOver?.Invoke();
    }
    
   
}
