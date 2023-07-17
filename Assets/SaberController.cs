using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaberController : MonoBehaviour
{
    
    //[SerializeField]private ShipColliderController shipColl;
   // public Action OnGameOver;
    
    public Vector3 newPosition;
    
   public Vector3 newRotation;
   
    private void Awake()
    {
       
        newPosition = transform.position;
        //newRotation = transform.localEulerAngles;
       // shipColl.OnShipHit = ShipHit;
    }

    public void UpdatePosition()
    {
        var curPos = transform.position;
        transform.position = Vector3.Lerp(curPos,newPosition,Time.deltaTime*20);
    }

    public void UpdateRotation()
    {
       var curRotation = transform.localEulerAngles;
       transform.localEulerAngles = Vector3.Lerp(curRotation, newRotation, 20f);
    }
    

    void ShipHit()
    {
       // Debug.Log("call Game Over");
        //OnGameOver?.Invoke();
    }
    
   
}