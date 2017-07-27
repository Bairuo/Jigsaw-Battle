﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;


/// [!] Notice: this MonoBehaviour might be used only by the server.
///     In this case this script should be disabled in all non-server player.
public class BlockGenerator : MonoBehaviour
{
    public Vector2[] targets;
    public GameObject[] targeters;
    public GameObject blockSource;
    
    public float delay; // Delay to checkout block creating.
    public enum GeneratingMode
    {
        GenerateWhenAble, // Try to create one.
        GenerateOne // If failed to create one, try another random location until successfully created.
    }
    public GeneratingMode generatingMode = GeneratingMode.GenerateWhenAble;
    public enum LocatingMode
    {
        All,
        TargetsOnly,
        TargetersOnly,
        Random
    }
    public LocatingMode locatingMode = LocatingMode.All;
    public Vector2 locatingRange = new Vector2(40, 40); // distance to the center. Used only when using LocatingMode.Random.
    public int maxAttempt = 64; // max attempt allowed when using Generate-one Mode.
    
    public float distance; // Will not deploy any block if a block has a distance to target point smaller than this.
    
    void Start() 
    {
        
    }
    
    float t = 0f;
    void FixedUpdate()
    {
        t += Time.fixedDeltaTime;
        if(t > delay)
        {
            t -= delay;
            bool generated = false;
            int count = 0;
            int ID;
            if(locatingMode == LocatingMode.All)
                ID = Mathf.FloorToInt(UnityEngine.Random.Range(0, targets.Length + targeters.Length));
            else if(locatingMode == LocatingMode.TargetsOnly)
                ID = Mathf.FloorToInt(UnityEngine.Random.Range(0, targets.Length));
            else if(locatingMode == LocatingMode.TargetersOnly)
                ID = targets.Length + Mathf.FloorToInt(UnityEngine.Random.Range(0, targeters.Length));
            else ID = -1;
            
            if(ID != -1)
            {
                do{
                    if(ID < targets.Length)
                    {
                        if(CanGenerate(targets[ID]))
                        {
                            Generate(targets[ID]);
                            generated = true;
                        }
                    }
                    else
                    {
                        ID -= targets.Length;
                        if(CanGenerate(targeters[ID].transform.position))
                        {
                            Generate(targeters[ID].transform.position);
                            generated = true;
                        }
                    }
                    count++;
                } while(generatingMode == GeneratingMode.GenerateOne && !generated && count < maxAttempt);
            }
            else
            {
                do{
                    Vector2 loc = new Vector2(
                        Mathf.RoundToInt(UnityEngine.Random.Range(-locatingRange.x, locatingRange.x)),
                        Mathf.RoundToInt(UnityEngine.Random.Range(-locatingRange.y, locatingRange.y))
                    );
                    if(CanGenerate(loc))
                    {
                        Generate(loc);
                        generated = true;
                    }
                    count++;
                } while(generatingMode == GeneratingMode.GenerateOne && !generated && count < maxAttempt);    
            }
        }
    }
    
    bool CanGenerate(Vector2 loc)
    {
        // TODO: Use collider to avoid generating near blocks.
        foreach(var i in GameObject.FindGameObjectsWithTag("Block"))
            if(Vector2.Distance(loc, i.transform.position) < distance) return false;
        foreach(var i in GameObject.FindGameObjectsWithTag("Player"))
            if(Vector2.Distance(loc, i.transform.position) < distance) return false;
        return true;
    }
    
    void Generate(Vector2 loc)
    {
        /// The random type thing are done by the block itself.
        var x = Instantiate(blockSource);
        x.transform.position = loc;
    }
}