using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
	static public int CharToInt(char x)
	{
		switch(x)
		{
			case '0': return 0;
			case '1': return 1;
			case '2': return 2;
			case '3': return 3;
			case '4': return 4;
			case '5': return 5;
			case '6': return 6;
			case '7': return 7;
			case '8': return 8;
			case '9': return 9;
			case 'A': return 10;
			case 'B': return 11;
			case 'C': return 12;
			case 'D': return 13;
			case 'E': return 14;
			case 'F': return 15;
			default: return -1;
		}
	}
	
	static public Color StringToColor(string s)
	{
		if(s[0] != '#')
		{
			Debug.Log("syntax error!");
			return new Color(0.0f, 0.0f, 0.0f, 0.0f);
		}
		int r = CharToInt(s[1]) * 16 + CharToInt(s[2]);
		int g = CharToInt(s[3]) * 16 + CharToInt(s[4]);
		int b = CharToInt(s[5]) * 16 + CharToInt(s[6]);
		if(s.Length > 7)
		{
			int a = CharToInt(s[7]) * 16 + CharToInt(s[8]);
			return new Color(r/255.0f, g/255.0f, b/255.0f, a/255.0f);
		}
		return new Color(r/255.0f, g/255.0f, b/255.0f, 1.0f);
	}
}