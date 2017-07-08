using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// ========================================================
/// This file is for storing patterns and their conversions.
/// ========================================================


public class Pattern
{
    public int height;
    public int width;
    bool[,] v; 
    Pattern(int w, int h)
    {
        width = w;
        height = h;
        v = new bool[h,w];
    }
    
    Pattern(String[] s)
    {
        height = s.Length;
        width = s[0].Length;
        v = new bool[height,width];
        
        for(int i=0; i<height; i++)
        {
            for(int j=0; j<width; j++)
            {
                v[i,j] = (s[i][j] == '0' || s[i][j] == ' ' || s[i][j] == '.' ? false : true);
            }
        }
    }
    
    public static Pattern randomBlock
    {
        get{ return blocks[Mathf.FloorToInt(UnityEngine.Random.Range(0, blocks.Length))]; }
    }
    
    public static Pattern randomTarget
    {
        get{ return targets[Mathf.FloorToInt(UnityEngine.Random.Range(0, targets.Length))]; }
    }
    
    /// may cause OutOfRangeException. Let it go along.
    public bool this[int x, int y]
    {
        get{ return v[x,y]; }
        set{ v[x,y] = value; }
    }




// ====================static patterns==================
    
    static Pattern block1 = new Pattern(
        new String[]{
            "#.#",
            "###"
        });
    static Pattern block2 = new Pattern(
        new String[]{
            "#..",
            "###"
        });
    static Pattern block3 = new Pattern(
        new String[]{
            "####"
        });
    
    
// ====================global patterns==================
    
    public static Pattern[] blocks = {
        block1,
        block2,
        block3
        };

        
    public static Pattern[] targets = {
        new Pattern( new String[] {
        "######",
        "######",
        "######",
        "######",
        "######",
        "######"}), 
        new Pattern( new String[] {
        "######",
        "######",
        "..##..",
        "..##..",
        "######",
        "######"}), 
        new Pattern( new String[] {
        "..##..",
        "..##..",
        "######",
        "######",
        "..##..",
        "..##.."}), 
        new Pattern( new String[] {
        "####..",
        "#####.",
        "######",
        "######",
        ".#####",
        "..####"}), 
        new Pattern( new String[] {
        "#....#",
        "######",
        "######",
        "######",
        "######",
        "#....#"}), 
        new Pattern( new String[] {
        "#.####",
        "#.####",
        "######",
        "######",
        "####.#",
        "####.#"}), 
        new Pattern( new String[] {
        "..####",
        "..####",
        ".#####",
        ".#..##",
        ".#..##",
        "######"}), 
        new Pattern( new String[] {
        "####",
        "####",
        "####",
        "####"}), 
        new Pattern( new String[] {
        "####",
        ".###",
        ".###",
        "####"}), 
        new Pattern( new String[] {
        "####",
        "####",
        "#..#",
        "####"}), 
        new Pattern( new String[] {
        ".##.",
        "####",
        "####",
        ".##."}), 
        new Pattern( new String[] {
        "####",
        "####",
        "##..",
        "####"})
        };




}


