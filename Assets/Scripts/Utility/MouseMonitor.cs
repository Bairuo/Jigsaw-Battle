using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MouseMonitor : MonoBehaviour
{
    public Vector2 screenLoc;
    public Vector2 viewLoc;
    public Vector2 worldLoc;
    
    void Awake()
    {
    }
    
    
    void Start() 
    {
    }
    
    void Update()
    {
        screenLoc = Input.mousePosition;
        viewLoc = Camera.main.ScreenToViewportPoint(screenLoc);
        worldLoc = Camera.main.ScreenToWorldPoint(screenLoc);
    }
}
