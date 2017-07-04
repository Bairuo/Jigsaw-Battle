using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // properties...
    public TargetArea target;
    public GameObject subSource;
    public float settleTime = 1.0f;
    public float obstacleTime = 1.0f;
    SubBlock[] subs;
    
    // automaton...
    public enum State
    {
        None,
        FreeToCatch,
        Catched,
        Obstacle,
        Settling,
        SettleDown
    };
    public State state;
    
    int playerID = -1; // -1 for none. 0 for recently obstacles. ID counts from 1.
    float t = 0.0f; // time count.
    
    
    // settling...
    Vector2 settleLoc;
    
    // debug...
    public bool debugMode = true; //false; // only used in dev.
    Vector2 anchorLoc;
    bool inControl = false;
    SpriteRenderer bugrd = null;
    
    
    /// [!]Notice: Assume that all sub-blocks size is 1x1.
    void BuildSubBlocks()
    {
        int count = 0;
        Pattern p = Pattern.randomBlock;
        for(int i=0; i<p.height; i++)
            for(int j=0; j<p.width; j++)
                if(p[i,j]) count++; 
        subs = new SubBlock[count];
        Vector2 baseloc = new Vector2(- p.width * 0.5f, - p.height * 0.5f);
        for(int i=0; i<p.height; i++)
            for(int j=0; j<p.width; j++)
                if(p[i,j])
                {
                    count--;
                    subs[count] = Instantiate(subSource, this.gameObject.transform).GetComponent<SubBlock>();
                    subs[count].gameObject.transform.localPosition = 
                        Vector2.right * (0.5f + j) + Vector2.up * (0.5f + i) + baseloc;
                }
    }
    
    void Start()
    {
        BuildSubBlocks();
        foreach(var i in subs)
            i.target = target;
    }
    
    void Update()
    {
        if(state == State.Settling)
        {
             t -= Time.deltaTime;
            /// TODO!!!
            /// move, and state-changing.
        }
        
        if(state == State.Obstacle)
        {
            t -= Time.deltaTime;
            /// TODO!!!
            /// state-chaging.
        }
        
        DebugUpdate();
    }
    
    public bool isSettlable
    {
        get
        {
            bool able = true;
            foreach(var i in subs)
                if(!i.tg) { able = false; break; }
            return able;
        }
    }
    
    /// call by player when get out of this block.
    public void Leave()
    {
        /// Interface required!!!
        /// TODO!!!
    }
    
    /// call by player when get into this block.
    public void Engage()
    {
        /// Interface required!!!
        /// TODO!!!
    }
    
    void DebugUpdate()
    {
        if(!debugMode && bugrd) bugrd.enabled = false;
        
        if(!debugMode) return;
        
        
        if(!bugrd)
        {
            bugrd = this.gameObject.GetComponent<SpriteRenderer>();
        }
        
        Vector2 worldLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if(Vector2.Distance(this.gameObject.transform.position, worldLoc) <= 1.0f)
        {
            bugrd.enabled = true;
            
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                inControl = true;
                anchorLoc = (Vector2)this.gameObject.transform.position - worldLoc;
                DebugEngage();
            }
        }
        else
        {
            bugrd.enabled = false;
            bugrd.color = Color.white;
        }
        
        if(Input.GetKey(KeyCode.Mouse0))
        {
            bugrd.color = Color.red;
        }
        else
        {
            bugrd.color = Color.white;
            inControl = false;
        }
        
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            DebugLeave();
        }
        
        if(inControl)
        {
            this.gameObject.transform.position = worldLoc + anchorLoc;
        }
    }
    
    public void DebugLeave()
    {
        if(state != State.Catched) return;
        
        if(isSettlable)
        {
            // set the settling procedure.
            playerID = 0;
            t = settleTime;
            
        }
        else
        {
            // set this an obstacle.
            playerID = 0;
            t = obstacleTime;
            state = State.Obstacle;
            
            /// TODO!!!
            /// Needs something to do with collision box.
            
        }
    }
    
    public void DebugEngage()
    {
        if(state != State.FreeToCatch || isSettlable) return;
        
        playerID = 1;
        state = State.Catched; // now movement is controlled by user/AI.
    }
}
