using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
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
    
    private static void GeneratePatternList(String source, ref List<Pattern> patternArray)
    {
        StringReader inp = new StringReader(source);
        String curline = null;
        List<String> strs = new List<String>();
        while((curline = inp.ReadLine()) != null) // I'm not reading at the file end.
        {
            if(curline.Equals("")) // the line only contains a '\n' and this character is removed in reading.
            {
                if(strs.Count != 0)
                {
                    int len = strs[0].Length;
                    for(int i=1; i<strs.Count; i++)
                    {
                        if(len != strs[i].Length)
                        {
                            Debug.Log("WARNING: pattern length not correct.");
                        }
                    }
                    
                    patternArray.Add(new Pattern(strs.ToArray()));
                    
                    strs.Clear();
                }
            }
            else
            {
                strs.Add(curline);
            }
        }
        
        if(strs.Count != 0)
        {
            patternArray.Add(new Pattern(strs.ToArray()));
        }
    }
    
    private static bool blockPatternListGenerated = false;
    public static Pattern randomBlock
    {
        get{
            if(!blockPatternListGenerated)
            {
                GeneratePatternList(blocksData, ref blocks);
                blockPatternListGenerated = true;
            }
            return blocks[Mathf.FloorToInt(UnityEngine.Random.Range(0, blocks.Count))]; 
        }
    }
    
    private static bool targetPatternListGenerated = false;
    public static Pattern randomTarget
    {
        get{
            if(!targetPatternListGenerated)
            {
                GeneratePatternList(targetsData, ref targets);
                targetPatternListGenerated = true;
            }
            return targets[Mathf.FloorToInt(UnityEngine.Random.Range(0, targets.Count))];
        }
    }
    
    /// may cause OutOfRangeException. Let it go along.
    public bool this[int x, int y]
    {
        get{ return v[x,y]; }
        set{ v[x,y] = value; }
    }


// ====================global patterns==================
    
    public static List<Pattern> blocks = new List<Pattern>();
    public static List<Pattern> targets = new List<Pattern>();


private static String blocksData =
@".#.
###

#.
##
#.

###
.#.

.#
##
.#

##
##

.##
##.

#.
##
.#

.#
.#
##

#..
###

##
#.
#.

###
..#

####

#
#
#
#

##.
.##

.#
##
#.

#.
#.
##

###
#..

##
.#
.#

..#
###

##

#
#

###

#
#
#

##
#.

##
.#

.#
##

#.
##

#

###
#..
#..

###
..#
..#

..#
..#
###

#..
#..
###";


private static String targetsData =
@"######
######
######
######
######
######

######
######
..##..
..##..
######
######

..##..
..##..
######
######
..##..
..##..

####..
#####.
######
######
.#####
..####

#....#
######
######
######
######
#....#

#.####
#.####
######
######
####.#
####.#

..####
..####
.#####
.#..##
.#..##
######

####
####
####
####

####
.###
.###
####

####
####
#..#
####

.##.
####
####
.##.

####
####
##..
####
";

}


