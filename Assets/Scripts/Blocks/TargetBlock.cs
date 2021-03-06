﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBlock : MonoBehaviour
{
    public int playerID;
    public Color enabledColor;
    public Color disabledColor;
    public Color establishedColor;
    
    public bool isBonus;
    
    public int count; // how many blocks are taken up this target.
                      // should be always 0 or 1.
    
    public bool established; // whether this object is taken-up and settled down.
    
    public SpriteRenderer rd;
    
    void Start()
    {
        if(!rd) rd = this.gameObject.GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if(count > 0)
        {
            if(established)
            {
                Color c = establishedColor * Camp.GetColor(playerID);
                c.a = 1.0f - (1.0f - c.a) * 0.7f;
                rd.color = c;
            }
            else
            {
                rd.color = enabledColor * Camp.GetColor(playerID);
            }
        }
        else rd.color = disabledColor * Camp.GetColor(playerID);
        
        if(isBonus)
        {
            rd.color = rd.color * 1.3f;
        }
    }
    
    public void Enter()
    {
        count++;
    }
    
    public void Leave()
    {
        count--;
    }
    
    public void TagSettled()
    {
        //if(count == 0) return;
        established = true;
    }
    
    public void LeaveSettled()
    {
        //if(count == 0) return;
        established = false;
    }
}
