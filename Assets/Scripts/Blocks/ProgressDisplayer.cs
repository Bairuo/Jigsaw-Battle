using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressDisplayer : MonoBehaviour
{
    public int playerID;
    public TargetArea target;
    public Image[,] subs;
    
    public GameObject subDisplayerSource;
    public Vector2 offset;
    
    bool inited = false;
    void Start()
    {
        inited = false;
    }
    
    void LateUpdate()
    {
        if (!inited)
        {
            //Debug.Log("init");
            target = Camp.GetTarget(playerID);
            if (target != null && target.grids != null && playerID != 0)
            {
                target = Camp.GetTarget(playerID);
                subs = new Image[target.width, target.height];
                //Debug.Log(subs == null);
                //Debug.Log(target.width +" "+ target.height);
                //Debug.Log("* " + subs.GetLength(0) + " " + subs.GetLength(1));
                //Debug.Log("* " + target.grids.GetLength(0) + " " + target.grids.GetLength(1));
                Vector2 cur = new Vector2(0.0f, 0.0f);
                for (int i = 0; i < target.width; i++)
                {
                    for (int j = 0; j < target.height; j++)
                    {
                        if (target.grids[i, j])
                        {
                            var x = Instantiate(subDisplayerSource, this.gameObject.transform);
                            subs[i, j] = x.GetComponent<Image>();
                            x.transform.localPosition = new Vector2(cur.x, -cur.y);
                        }
                        cur.x += offset.x;
                    }
                    cur.y += offset.y;
                    cur.x = 0.0f;
                }
                inited = true;
            }
            else
            {
                //Debug.Log("failed.");
                return;
            }
        }

        //Debug.Log(subs.GetLength(0) + " " + subs.GetLength(1));
        //Debug.Log(target.grids.GetLength(0) + " " + target.grids.GetLength(1));

        for(int i=0; i<target.width; i++)
            for(int j=0; j<target.height; j++)
            {
                if(subs[i,j] && target.grids[i,j])
                {
                    if(target.grids[i,j].established)
                    {
                        Color c = target.grids[i,j].establishedColor * Camp.GetColor(playerID);
                        c.a = 1.0f - (1.0f - c.a) * 0.7f;
                        subs[i,j].color = c;
                    }
                    else
                    {
                        subs[i,j].color = target.grids[i,j].disabledColor * Camp.GetColor(playerID);
                    }
                    
                    if(target.grids[i,j].isBonus)
                    {
                        subs[i,j].color = target.grids[i,j].rd.color * 1.3f;
                    }
                }
            }
    }
}
