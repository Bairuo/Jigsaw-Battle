using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // properties...
    public TargetArea target = null;
    public GameObject subSource;
    public float settleTime = 0.5f;
    public float settleDownTime = 0.4f; // You have to wait for this time to pull it out from settled.
    public float obstacleTime = 3.0f;
    SubBlock[] subs;
    
    public Color normalColor;
    public Color obstacleColor;
    
    // automaton...
    public enum State
    {
        None,
        FreeToCatch,
        Catching,
        Catched,
        Obstacle,
        Settling,
        SettleDown,
        
    };
    public State state = State.FreeToCatch;
    
    int playerID = -1; // -1 for none. 0 for recently obstacles. ID counts from 1.
    float t = 0.0f; // time count.
    
    
    // settling...
    Vector2 settleFrom;
    Vector2 settleTo;
    
    public bool isSettlable {
        get
        {
            bool able = true;
            foreach(var i in subs)
                if(!i.tg) { able = false; break; }
            return able;
        }}
    
    Vector2 baseloc;
    public Vector2 settleVec { get { return subs[0].GetDistanceVector(); } }
    
    // debug...
    public bool debugMode = true; //false; // only used in dev.
    Vector2 anchorLoc;
    bool inControl = false;
    SpriteRenderer bugrd = null;
    
//=======================================================================================
    
    /// [!]Notice: Assume that all sub-blocks size is 1.0x1.0.
    void BuildSubBlocks()
    {
        int count = 0;
        Pattern p = Pattern.randomBlock;
        for(int i=0; i<p.height; i++)
            for(int j=0; j<p.width; j++)
                if(p[i,j]) count++; 
        subs = new SubBlock[count];
        baseloc = new Vector2(- (p.width - 1) * 0.5f, (p.height - 1) * 0.5f);
        for(int i=0; i<p.height; i++)
            for(int j=0; j<p.width; j++)
                if(p[i,j])
                {
                    count--;
                    subs[count] = Instantiate(subSource, this.gameObject.transform).GetComponent<SubBlock>();
                    subs[count].gameObject.transform.localPosition = 
                        Vector2.right * j - Vector2.up * i + baseloc;
                    subs[count].gameObject.GetComponent<SpriteRenderer>().color = normalColor;
                    var box = this.gameObject.AddComponent<BoxCollider2D>();
                    box.size = new Vector2(0.6f, 0.6f);
                    box.edgeRadius = 0.17f;
                    box.offset = subs[count].gameObject.transform.localPosition;
                }
    }
    
    void Start()
    {
        BuildSubBlocks();
        foreach(var i in subs)
            i.target = target;
    }
    
    
//=======================================================================================
    
    /// Changed from UPdate() to FixedUpdate()...
    void FixedUpdate()
    {
        switch(state)
        {
            case State.Catching : // [!]Notice: This state cannot be test without player.
            {
                t -= Time.fixedDeltaTime;
                // [!]TODO: do some interpolation of magical drawing...
                if(t < 0f)
                {
                    t = 0f;
                    state = State.Catched; // [!]automaton: To cached.
                }
            }
            break;
            case State.Settling : 
            {
                t -= Time.fixedDeltaTime;
                if(t > 0f) // Moving the block to the correct location.
                {
                    float rate = t / settleTime;
                    float inter = rate * rate;
                    this.gameObject.transform.position = inter * (settleFrom - settleTo) + settleTo;
                }
                else // Blcok is not placed correctly. Hold on a delay preventing mis-take this block.
                {
                    this.gameObject.transform.position = settleTo;
                    if(t <= -settleDownTime) // Block hold-on duration ended.
                    {
                        t = 0f;
                        state = State.SettleDown; // [!]automaton: To SettleDown.
                    }
                }
            }
            break;
            case State.Obstacle :
            {
                t -= Time.fixedDeltaTime;
                if(t <= 0f)
                {
                    t = 0f;
                    CancelObstacle();
                    state = State.FreeToCatch; // [!]automaton: To FreeToCatch.
                }
            }
            break;
            default: break;
        } // swtich.
        
        DebugUpdate();
    }
    
    /// Called by player when get out of this block.
    /// This function represents an "input" if the automaton, it can change state safely.
    public void Leave()
    {
        if(state != State.Catched) return;
        
        if(isSettlable) 
        {
            // set the settling procedure.
            playerID = 0;
            t = settleTime;
            state = State.Settling; // [!]automaton: To settling.
            settleFrom = this.gameObject.transform.position;
            settleTo = (Vector2)this.gameObject.transform.position + settleVec;
            Settle();
        }
        else 
        {
            // set this an obstacle.
            playerID = 0;
            t = obstacleTime;
            state = State.Obstacle; // [!]automaton: To obstacle.
            SetObstacle();
            /// TODO!!!
            /// Needs something to do with collision box.
            
        }
    }
    
    /// Called by player when get into this block.
    /// This function represents an "input" if the automaton, it can change state safely.
    public void Engage(Vector2 loc, int playerID)
    {
        if(state != State.FreeToCatch && state != State.SettleDown) return;
        
        /// TODO!!! set the target area.
        target = null;
        this.playerID = playerID;
        state = State.Catching;
    }
    
    public void Settle()
    {
        foreach(var i in subs) i.Settle();
    }
    
    public void UnSettle()
    {
        foreach(var i in subs) i.UnSettle();
    }
    
    public void SetObstacle()
    {
        foreach(var i in subs)
            i.gameObject.GetComponent<SpriteRenderer>().color = obstacleColor;
    }
    
    public void CancelObstacle()
    {
        foreach(var i in subs)
            i.gameObject.GetComponent<SpriteRenderer>().color = normalColor;
    }
    
//=======================================================================================
    
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
            state = State.Settling;
            settleFrom = this.gameObject.transform.position;
            settleTo = (Vector2)this.gameObject.transform.position + settleVec;
            Settle();
        }
        else
        {
            // set this an obstacle.
            playerID = 0;
            t = obstacleTime;
            state = State.Obstacle;
            SetObstacle();
            /// TODO!!!
            /// Needs something to do with collision box.
            
        }
    }
    
    public void DebugEngage()
    {
        if(state != State.FreeToCatch && state != State.SettleDown) return;
        target = GameObject.Find("target-area").GetComponent<TargetArea>();
        foreach(var i in subs) i.target = this.target;
        
        // DEBUG SECTION...
        Vector2 worldLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        inControl = true;
        anchorLoc = (Vector2)this.gameObject.transform.position - worldLoc;
        // END...
        
        playerID = 1;
        state = State.Catched; // now movement is controlled by user/AI.
        UnSettle();
    }
}
