using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    // ASSUME: lengths of every grid is defined in the grid object.
    public int height;
    public int width;
    
    public GameObject gridSource;
    
    public TargetBlock[,] grids; // true enabled, false disabled.
    
    
    private Vector2 size;
    private Vector2 baseloc;
    private Vector2 halfloc;
    
    void Start() 
    {
        size.x = gridSource.transform.localScale.x;
        size.y = gridSource.transform.localScale.y;
        baseloc = new Vector2(- size.x * width * 0.5f, - size.y * height * 0.5f);
        halfloc = size * 0.5f;
        
        grids = new TargetBlock[height, width]; // form left-top corner to right-bottom corner.
        
        for(int i=0; i<height; i++)
            for(int j=0; j<width; j++)
            {
                GameObject g = Instantiate(gridSource, this.gameObject.transform);
                g.name = "grid(" + i + "," + j + ")";
                g.transform.localPosition = new Vector2(i * size.y, j * size.x) + baseloc + halfloc;
                grids[i,j] = g.GetComponent<TargetBlock>();
            }
        
        if(height < 1) height = 1;
        if(width < 1) width = 1;
    }
    
    void Update()
    {
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
        Vector2 indloc = relloc - baseloc;
        return new Point(Mathf.FloorToInt(indloc.x/size.x), Mathf.FloorToInt(indloc.y/size.y));
    }
    
    bool InRange(Point v) { return v.x >= 0 && v.x < width && v.y >= 0 && v.y < height; }
    
    public TargetBlock GetNearestBlock(Vector2 loc)
    {
        Point v = GetLoc(loc);
        if(InRange(v)) return grids[v.x,v.y];
        else return null;
    }
}
