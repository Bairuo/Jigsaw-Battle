using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camp : MonoBehaviour
{
	/// original color: RGB:DFE3CB
	public Color[] campColor = new Color[]{Util.StringToColor("#F96900"), Util.StringToColor("#69D1E7")};
	public TargetArea[] campTarget;
	
	static public int GetCamp(int playerID)
	{
		return playerID > 0 ? 1 : playerID < 0 ? -1 : 0;
	}
	
	static public Color GetColor(int playerID)
	{
		return campColor[playerID < 0 ? 0 : 1];
	}
	
	static public TargetArea GetTarget(int playerID)
	{
		return campTarget[playerID < 0 ? 0 : 1];
	}
	
	static public void SetTargetArea(TargetArea target, int playerID)
	{
		if(playerID < 0) campTarget[0] = target;
		else campTarget[1] = target;
	}
}
