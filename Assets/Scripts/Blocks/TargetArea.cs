using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetArea : MonoBehaviour
{
    public GameObject gridSource;
    public int playerID; // or campID as -1 or 1.
    public TargetBlock[,] grids; // true enabled, false disabled.
    public TargetArea opposite;
    int patternID = -1;
    public Slider slider;
    public Text rateDisplay;
    public bool fullfilled;
    
    public int height;
    public int width;
    
    Vector2 baseloc;
    Vector2 halfloc;
    
    public bool completed { get
        {
            for(int i=0; i<height; i++)
                for(int j=0; j<width; j++)
                    if(grids[i,j] && !grids[i,j].established) return false;
            return true;
        }}
    
    public bool bonusCompleted { get
        {
            for(int i=0; i<height; i++)
                for(int j=0; j<width; j++)
                    if(grids[i,j] && grids[i,j].isBonus && !grids[i,j].established)
                        return false;
            return true;
        }
    }
    
    void Start()
    {
        Camp.SetTargetArea(this, playerID);
    }

    public static void NetAreaInit(string tag, int patternID){
        //Debug.Log("Is client");
        /*
        GameObject area = GameObject.FindGameObjectWithTag(tag);
        area.GetComponent<TargetArea>().AreaInit(patternID);
         * */
        GameObject[] areas = GameObject.FindGameObjectsWithTag("Area");
        foreach (var item in areas)
        {
            item.GetComponent<TargetArea>().AreaInit(patternID);
        }
    }
    
    void AreaInit()
    {
        if(patternID == -1)
        {
            opposite.patternID = patternID = Pattern.randomTargetID;
            //Debug.Log(patternID);
            AreaInit(patternID);
            opposite.AreaInit(patternID);
            Client.instance.SendAreaInit(this.tag, patternID);
            
        }
    }
    
    public void AreaInit(int patternID)
    {
        fullfilled = false;
        
        Pattern p = Pattern.GetTargetPattern(patternID);
        
        height = p.height;
        width = p.width;
        //Debug.Log(playerID + " " + patternID + " cr " + height + " " + width);
        
        baseloc = new Vector2(- width * 0.5f, height * 0.5f);
        halfloc = new Vector2( 0.5f, -0.5f);
        
        grids = new TargetBlock[height, width]; // form left-top corner to right-bottom corner.
        
        for(int i=0; i<height; i++)
            for(int j=0; j<width; j++)
                if(p[i,j] != 0)
                {
                    GameObject g = Instantiate(gridSource, this.gameObject.transform);
                    g.name = "grid(" + i + "," + j + ")";
                    Vector2 loc = new Vector2(j, -i) + baseloc + halfloc;
                    loc.x *= this.gameObject.transform.localScale.x;
                    loc.y *= this.gameObject.transform.localScale.y;
                    g.transform.localPosition = loc;
                    grids[i,j] = g.GetComponent<TargetBlock>();
                    g.transform.localScale = this.gameObject.transform.localScale;
                    grids[i,j].playerID = playerID;
                    if(p[i,j] > 1)
                        grids[i,j].isBonus = true;
                }
                else
                {
                    grids[i,j] = null;
                }
    }
    
    void Update()
    {
        if (Client.instance.playerid != "0") return;
        
        AreaInit();
        
        int cnt = 0;
        int crr = 0;
        for(int i=0; i<height; i++)
            for(int j=0; j<width; j++)
                if(grids[i,j])
                {
                    crr++;
                    if(grids[i,j].established)
                        cnt++;
                }
        rateDisplay.text = (100 * (slider.value = (float)cnt / crr)).ToString("00.") + "%";
        
        if(bonusCompleted)
            SetBonus();
    }
    
    public struct Point
    {
        public int x,y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    
    public Point GetLoc(Vector2 loc)
    {
        Vector2 relloc = loc - (Vector2)this.gameObject.transform.position;
        relloc.x /= this.gameObject.transform.localScale.x;
        relloc.y /= this.gameObject.transform.localScale.y;
        Vector2 indloc = relloc - baseloc;
        return new Point(Mathf.FloorToInt(indloc.x), Mathf.FloorToInt(-indloc.y));
    }
    
    bool InRange(Point v) { return v.x >= 0 && v.x < width && v.y >= 0 && v.y < height; }
    
    public TargetBlock GetNearestBlock(Vector2 loc)
    {
        Point v = GetLoc(loc);
        if(InRange(v)) return grids[v.y,v.x];
        else return null;
    }
    
    public void ReportCompleted()
    {
        fullfilled  = true;
        /// TODO!!!
    }
    
    bool bonusGiven = true;
    public void SetBonus()
    {
        if(bonusGiven) return;
        // TODO!!!
    }
}
