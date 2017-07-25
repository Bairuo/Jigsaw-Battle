using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camp
{
	static public Color[] campColor = new Color[]{Util.StringToColor("#DFE3CB"), Util.StringToColor("#69D1E7")};
	
	static public int GetCamp(int playerID)
	{
		return playerID > 0 ? 1 : playerID < 0 ? -1 : 0;
	}
	
	static public Color GetColor(int playerID)
	{
		return campColor[GetCamp(playerID) < 0 ? 0 : 1];
	}
}
