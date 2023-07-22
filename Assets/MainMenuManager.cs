using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public int gameIndex = 1;
    public Button right;
    public Button left;
    public UDPReceive udpReceive_pos;
    public UDPReceive udpReceive_sign;

    public void ChangeIndex(int change)
    {
        gameIndex += change;
        if (gameIndex > 3) gameIndex = 0;
        if (gameIndex < 0) gameIndex = 3;
    }

    private void Start()
    {
        udpReceive_pos = GameObject.Find("UDP5056").GetComponent<UDPReceive>();
        udpReceive_sign = GameObject.Find("UDP5055").GetComponent<UDPReceive>();
    }

    private float okDelay = 0.5f;
    private float okCD = 0f;
    private string posdata = "[0,0]";
    private Vector2 posCheckPoint = new Vector2(0, 0);
    private bool isPosChecked = false;
    private float swipeCD = 0.25f;
    private void Update()
    {
        if (udpReceive_sign.data == "['1']")
        {
            if (okCD < okDelay)
            {
                okCD += Time.deltaTime;
            }
            else
            {
                ChangeMenu();
            }
        }
        else
        {
            okCD = 0f;
        }

        if (swipeCD > 0) swipeCD -= Time.deltaTime;
        if (udpReceive_sign.data == "['2']")
        {
            if (swipeCD > 0) return;
            
            posdata = udpReceive_pos.data;
            if (posdata == null) return;
            try
            {
                posdata = posdata.Remove(0, 1);
                posdata = posdata.Remove((posdata.Length - 1), 1);
            }
            catch
            {
                return;
            }

            string[] pos = posdata.Split(',');

            if (!isPosChecked)
            {
                float x = float.Parse(pos[0]);
                float y = float.Parse(pos[1]);
                isPosChecked = true;
                posCheckPoint = new Vector2(x, y);
            }
            else
            {
                float x = float.Parse(pos[0]);
                float y = float.Parse(pos[1]);

                if (x > posCheckPoint.x + 50)
                {
                    right.onClick.Invoke();
                    ResetPos();
                    swipeCD = 0.25f;
                }
                if (x < posCheckPoint.x - 50) 
                {
                    left.onClick.Invoke();
                    ResetPos();
                    swipeCD = 0.25f;
                }
            }
        }
        else
        {
            ResetPos();
        }
    }

    void ResetPos()
    {
        posdata = "[0,0]";
        posCheckPoint = new Vector2(0, 0);
         isPosChecked = false;
    }
    // Update is called once per frame
    public void ChangeMenu()
    {
        Debug.Log("change menu" + gameIndex);
        switch (gameIndex)
        {
            case 0:
                SceneManager.LoadScene("Menu_NS");
                break; // neon saber
            case 1:
                SceneManager.LoadScene("Menu_SD");
                break; // space dasher
            case 2:
                SceneManager.LoadScene("Menu_LD");
                break; // let's dance
            case 3:
                Application.Quit();
                break; // quit
        }
    }
}