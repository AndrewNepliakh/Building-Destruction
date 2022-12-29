using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    public float updateInterval = 0.5F;
    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private string fpsText;
    private GameObject[] AllObjects;
    private int DrawCalls;
    [SerializeField] private GUIStyle _guiStyle;
    [SerializeField] private bool _displayDrawCalls = false;

    void CalculateFPS()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("{0:F2}", fps);
            fpsText = format;
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }

    void Draw_DrawCalls()
    {
        DrawCalls = 0;
        foreach (GameObject g in AllObjects)
        {
            if (g != null && g.GetComponent<Renderer>() && g.GetComponent<Renderer>().isVisible)
            {
                DrawCalls++;
            }
        }
    }

    void Start()
    {
        timeleft = updateInterval;
        AllObjects = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFPS();
        // Draw_DrawCalls();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(30f, 30f, 100.0f, 25.0f), fpsText, _guiStyle);
        if (_displayDrawCalls)
            GUI.Label(new Rect(0.0f, 50.0f, 200.0f, 25.0f), "Total Draw Calls : " + DrawCalls.ToString(), _guiStyle);
    }
}