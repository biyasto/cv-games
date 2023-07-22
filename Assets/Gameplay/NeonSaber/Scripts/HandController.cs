using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HandController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Vector2 Sen;
    [SerializeField] private Vector2 StartPoint;

    public UDPReceive udpReceive;
    public Vector3  Position0;
    public Vector3  Position1;
    public Vector3  startPos;
    public bool isStarted = false;
    public float Rotation0, Rotation1;
    //public GameObject FacePoint;

    // private Vector3 originPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = new Vector3(1, 1.5f, -5);
        Position0 = startPos;
        Position1 = startPos;
        udpReceive = GameObject.Find("UDP5053").GetComponent<UDPReceive>();
    }
    
    public void ReadyFaceCheck()
    {
        if (!isStarted)
        {
            if (udpReceive.isReceived)
                isStarted = true;
        }
    }

    // Update is called once per frame
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
        float x0 = float.Parse(pos[6]) * Sen.x + StartPoint.x;
        float y0 = -float.Parse(pos[7]) * Sen.y + StartPoint.y;
        
        
        float x1 = float.Parse(pos[9]) * Sen.x + StartPoint.x;
        float y1 = -float.Parse(pos[10]) * Sen.y + StartPoint.y;
        Vector2 point1 = new Vector2(-float.Parse(pos[6]), -float.Parse(pos[7]));
        Vector2 point1x = new Vector2(-float.Parse(pos[0]), -float.Parse(pos[1]));
        Vector2 point2 = new Vector2(-float.Parse(pos[9]), -float.Parse(pos[10]));
        Vector2 point2x = new Vector2(-float.Parse(pos[3]), -float.Parse(pos[4]));
      
        
        Position0 =startPos + new Vector3(x0,y0,8);
        Position1 =startPos + new Vector3(x1,y1,8);
        Rotation0 = Angle(point1 - point1x);
        Rotation1 = Angle(point2 - point2x);
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