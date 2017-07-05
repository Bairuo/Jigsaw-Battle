using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBlock : MonoBehaviour
{
    public Color enabledColor;
    public Color disabledColor;
    public Color establishedColor;
    
    public int count; // how many blocks are taken up this target.
                      // should be always 0 or 1.
    
    public bool established; // whether this object is taken-up and settled down.
    
    SpriteRenderer rd;
    
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
                rd.color = establishedColor;
            }
            else
            {
                rd.color = enabledColor;
            }
        }
        else rd.color = disabledColor;
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
        if(count == 0) return;
        established = true;
    }
    
    public void LeaveSettled()
    {
        if(count == 0) return;
        established = false;
    }
}
