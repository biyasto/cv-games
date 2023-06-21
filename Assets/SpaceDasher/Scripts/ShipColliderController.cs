using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipColliderController : MonoBehaviour
{
    public Action OnShipHit;
   
    private void OnTriggerEnter(Collider other)
    {
       // if (other.tag.)
        {
            OnShipHit?.Invoke();
        }
    }
    
}
