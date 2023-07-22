using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FaceController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Vector2 Sen;
    [SerializeField] private Vector2 StartPoint;

    public UDPReceive udpReceive;
    public Vector3  Position;
    public Vector3  startPos;
    public float  Rotation;

    public bool isStarted = false;
    //public GameObject FacePoint;

   // private Vector3 originPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = new Vector3(0, 2, -5);
        Position = startPos;
        udpReceive = GameObject.Find("UDP5052").GetComponent<UDPReceive>();
    }

    // Update is called once per frame
    public void ReadyFaceCheck()
    {
        if (!isStarted)
        {
            if (udpReceive.isReceived)
                isStarted = true;
        }
    }
    public void UpdateFace()
    {
       
        string data = udpReceive.data;
        if(data==null) return;
        try
        {
            data = data.Remove(0, 1);
            data = data.Remove((data.Length - 1), 1);
        }
        catch
        {
            return;
        }
       
        string[] pos = data.Split(',');
        
        //middle     
        float x = float.Parse(pos[0]) * Sen.x + StartPoint.x;
        float y = -float.Parse(pos[1]) * Sen.y + StartPoint.y;

        
        Vector2 point1 = new Vector2(-float.Parse(pos[2]), -float.Parse(pos[3]));
        Vector2 point2 = new Vector2(-float.Parse(pos[4]), -float.Parse(pos[5]));
        
      
        
        Position =startPos + new Vector3(x,y,0);
        Rotation = -(180-Angle(point2 - point1));
        // print(Angle( new Vector2(1,0)));
    }
    
    public float Angle(Vector2 vector2)
    {
        if (vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
        }
    }
}