﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBlock : MonoBehaviour
{
    public TargetArea target;
    
    
    void Start()
    {
        
    }
    
    public TargetBlock tg = null; // the block I already stand on last frame.
    
    void Update()
    {
        TargetBlock tb = null; // the block I stand on this frame.
        if(target) tb = target.GetNearestBlock(this.gameObject.transform.position);
        
        // switch linked-sub-block from tg to tb.
        if(tg) tg.Leave();
        if(tb) tb.Enter();
        tg = tb;
        
    }
    
    public Vector2 GetDistanceVector()
    {
        return tg.gameObject.transform.position - this.gameObject.transform.position;
    }
    
    public void Settle()
    {
        if(tg) tg.TagSettled();
    }
    
    public void UnSettle()
    {
        if(tg) tg.LeaveSettled();
    }
}
