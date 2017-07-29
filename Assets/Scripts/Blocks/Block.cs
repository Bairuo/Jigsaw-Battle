using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Block : MonoBehaviour
{
    // properties...
    public TargetArea target = null;
    public GameObject subSource;
    public float catchingTime = 2f;
    public float settleTime = 0.5f;
    public float settleDownTime = 0.4f; // You have to wait for this time to pull it out from settled.
    public float obstacleTime = 3.0f;
    SubBlock[] subs;
    
    GameObject circle;
    int height;
    int width;
    
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
    
    int playerID = 1; // -1 for none. 0 for recently obstacles. ID counts from 1.
    float t = 0.0f; // time count.
    
    // catching...
    float radius;
    
    // settling...
    Vector2 settleFrom;
    Vector2 settleTo;
    
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
    
    Vector2 baseloc;
    public Vector2 settleVec { get { return subs[0].GetDistanceVector(); } }
    
//=======================================================================================
    
    static int baseCount = 0;
    /// [!]Notice: Assume that all sub-blocks size is 1.0x1.0.
    void BuildSubBlocks()
    {
        int count = 0;
        circle = this.gameObject.transform.GetChild(0).gameObject;
        Pattern p = Pattern.randomBlock;
        height = p.height;
        width = p.width;
        
        for(int i=0; i<height; i++)
            for(int j=0; j<width; j++)
                if(p[i,j] != 0) count++; 
        subs = new SubBlock[count];
        baseloc = new Vector2(- (width - 1) * 0.5f, (height - 1) * 0.5f);
        int cnt = count; // Temp. count.
        
        for(int i=0; i<height; i++)
            for(int j=0; j<width; j++)
                if(p[i,j] != 0)
                {
                    cnt--;
                    subs[cnt] = Instantiate(subSource, this.gameObject.transform).GetComponent<SubBlock>();
                    subs[cnt].name = "sub(" + i + "," + j + ")";
                    subs[cnt].gameObject.transform.localPosition =  Vector2.right * j - Vector2.up * i + baseloc;
                    
                    var box = this.gameObject.AddComponent<BoxCollider2D>();
                    box.size = new Vector2(0.86f, 0.86f);
                    box.edgeRadius = 0.05f;
                    box.offset = subs[cnt].gameObject.transform.localPosition;
                    box.isTrigger = true;
                    
                    var rg = subs[cnt].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
                    var rc = subs[cnt].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
                    var rd = subs[cnt].gameObject.GetComponent<SpriteRenderer>();
                    rd.sortingOrder = baseCount + count - 1 - cnt; // draw pictures first.
                    rd.color = normalColor;
                    rg.sortingOrder = baseCount + 2 * count - 1 - cnt; // draw stencil.
                    rc.sortingOrder = baseCount + 3 * count - cnt; // clear stencil after draw the capture circle.
                }
        
        var crd = circle.GetComponent<SpriteRenderer>();
        crd.sortingOrder = baseCount + 2 * count;
        baseCount += 3 * count;
    }
    
    void Start()
    {
        BuildSubBlocks();
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
                float rate = (catchingTime - t) / catchingTime;
                CircleInterpolate(Mathf.Pow(rate, 0.65f), radius);
                if(t < 0f)
                {
                    t = 0f;
                    CircleInterpolate(1.0f, radius);
                    state = State.Catched; // [!]automaton: To cached.
                }
            }
            break;
            case State.Settling : 
            {
                t -= Time.fixedDeltaTime;
                float rate = (t + settleDownTime) / (settleTime + settleDownTime);
                CircleInterpolate(rate, Mathf.Sqrt(width * width * 0.25f + height * height * 0.25f));
                
                if(t > 0f) // Moving the block to the correct location.
                {
                    float settleRate = t / settleTime;
                    float inter = settleRate * settleRate;
                    this.gameObject.transform.position = inter * (settleFrom - settleTo) + settleTo;
                }
                else // Blcok is now placed correctly. Hold on a delay preventing mis-take this block.
                {
                    this.gameObject.transform.position = settleTo;
                    if(t <= -settleDownTime) // Block hold-on duration ended.
                    {
                        t = 0f;
                        CircleInterpolate(rate * rate, 9f);
                        state = State.SettleDown; // [!]automaton: To SettleDown.
                    }
                }
            }
            break;
            case State.SettleDown : // No state transform by itself.
            {
                t -= Time.fixedDeltaTime;
                float rate = t / obstacleTime;
                CircleInterpolate(rate * rate, Mathf.Sqrt(width * width * 0.25f + height * height * 0.25f));
                if(t <= 0f)
                {
                    t = 0f;
                    CircleInterpolate(0f, 9f);
                    foreach(var i in subs)
                        i.gameObject.GetComponent<SpriteRenderer>().color = normalColor;
                }
            }
            break;
            case State.Obstacle :
            {
                t -= Time.fixedDeltaTime;
                float rate = t / obstacleTime;
                CircleInterpolate(rate * rate, Mathf.Sqrt(width * width * 0.25f + height * height * 0.25f));
                if(t <= 0f)
                {
                    t = 0f;
                    CircleInterpolate(0f, 9f);
                    foreach(var i in subs)
                        i.gameObject.GetComponent<SpriteRenderer>().color = normalColor;
                    state = State.FreeToCatch; // [!]automaton: To FreeToCatch.
                }
            }
            break;
            default: break;
        } // swtich.
    }
    
    /// Called by player when get out of this block.
    /// This function represents an "input" if the automaton, it can change state safely.
    public bool Leave()
    {
        if(state != State.Catched) return false;
        
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
            foreach(var i in subs)
                i.gameObject.GetComponent<SpriteRenderer>().color = obstacleColor;
            /// TODO!!!
            /// Needs something to do with collision box.
        }
        
        float mx = Mathf.Max(width, height);
        circle.transform.localScale = new Vector2(mx, mx) * 0.5f;
        circle.transform.position = this.gameObject.transform.position;
        return true;
    }
    
    /// Called by player when get into this block.
    /// This function represents an "input" if the automaton, it can change state safely.
    public bool Engage(Vector2 loc, int playerID)
    {
        if(state != State.FreeToCatch && state != State.SettleDown) return false;
        
        target = null;
        this.playerID = playerID;
        t = catchingTime;
        radius = MaxDistance(loc);
        circle.transform.position = loc;
        if(state == State.SettleDown)
            UnSettle();
        state = State.Catching;
        
        var crd = circle.GetComponent<SpriteRenderer>();
        crd.color = Camp.GetColor(playerID);
        target = Camp.GetTarget(playerID);
        foreach(var i in subs)
            i.target = target;
        
        return true;
    }
    
    public void Settle()
    {
        foreach(var i in subs) i.Settle();
    }
    
    public void UnSettle()
    {
        foreach(var i in subs) i.UnSettle();
    }
    
    /// Get the max distance from loc to one of the vertices of this block's surrounding rectangle.
    public float MaxDistance(Vector2 loc)
    {
        Vector2 relloc = loc - (Vector2)this.gameObject.transform.position;
        Vector2 rt = Vector2.right * width * 0.5f;
        Vector2 up = Vector2.up * height * 0.5f;
        float mx = (relloc + rt + up).magnitude;
        mx = Mathf.Max(mx, (relloc + rt - up).magnitude);
        mx = Mathf.Max(mx, (relloc - rt + up).magnitude);
        mx = Mathf.Max(mx, (relloc - rt - up).magnitude);
        return mx;
    }
    
    /// x between 0 to 1. Size as the basic local scale when x == 1.
    public void CircleInterpolate(float x, float sz)
    {
        x = Mathf.Clamp(x, 0.0f, 1.0f);
        circle.transform.localScale = new Vector2(sz * x, sz * x);
    }
    
    
//=======================================================================================
// Debug section removed.
}
