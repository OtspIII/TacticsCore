using System;
using System.Collections.Generic;
using UnityEngine;

public static class God
{
   public static GameManager GM;
   public static LibraryController Library;
   
   public static Directions[] Cardinal = new[] {Directions.Up, Directions.Right, Directions.Down, Directions.Left}; 
   public static Directions[] EightDir = new[] {Directions.Up, Directions.Right, Directions.Down, Directions.Left,Directions.UR,Directions.UL,Directions.DL,Directions.DR};
   
   public static bool CoinFlip(float odds=0.5f)
   {
	   return UnityEngine.Random.value < odds;
   }
   
   public static Vector2 MouseLoc(Camera cam=null)
   {
	   if (cam == null)
		   cam = UnityEngine.Camera.main;
	   return cam.ScreenToWorldPoint(Input.mousePosition);
   }
   
   public static int Roll(int rolls, int size, int bonus=0){
	   int r = 0;
	   if (size > 0) {
		   for (int n = 0; n < Mathf.Abs(rolls); n++)
			   r += UnityEngine.Random.Range (1, size+1);
	   }
	   r += bonus;
	   if (r < 0)
		   r = 0;
	   if (rolls < 0)
		   return -r;
	   return r;
   }
   
   public static int Random(int max){
	   return UnityEngine.Random.Range(0,max);
   }

   public static float Random(float max){
	   return UnityEngine.Random.Range(0,max);
   }
   
   public static T Random<T>(this List<T> l) where T:class
   {
	   if (l.Count == 0)
		   return null;
	   return l[UnityEngine.Random.Range(0, l.Count)];
   }
   public static T RandomS<T>(this List<T> l) where T:struct
   {
	   if (l.Count == 0)
		   return new T();
	   return l[UnityEngine.Random.Range(0, l.Count)];
   }
   public static T RandomE<T>(this List<T> l) where T:Enum
   {
	   // if (l.Count == 0)
		  //  return Enum.Parse<T>("");
	   return l[UnityEngine.Random.Range(0, l.Count)];
   }
   
   public static List<T> Shuffle<T>(this List<T> l) where T:class
	{
		if (l.Count == 0)
			return l;
		List<T> temp = new List<T>();
		temp.AddRange(l);
		List<T> r = new List<T>();
		while (temp.Count > 0)
		{
			T chosen = temp.Random();
			temp.Remove(chosen);
			r.Add(chosen);
		}
		return r;
	}
	public static List<T> ShuffleStruct<T>(this List<T> l) where T:struct
	{
		if (l.Count == 0)
			return l;
		List<T> temp = new List<T>();
		temp.AddRange(l);
		List<T> r = new List<T>();
		while (temp.Count > 0)
		{
			T chosen = temp.RandomS();
			temp.Remove(chosen);
			r.Add(chosen);
		}
		return r;
	}

	public static T WeightedRandom<T>(Dictionary<T, float> dict,bool deb=false) where T : class
	{
		string debug = "WR [";
		float total = 0;
		foreach (float w in dict.Values)
			total += w;
		if (deb)
			foreach (T t in dict.Keys)
				debug += t.ToString() + "." + dict[t] + " / ";
		float roll = UnityEngine.Random.Range(0f, total);
		debug += roll + "::" + total;
		foreach (T opt in dict.Keys)
		{
			roll -= dict[opt];
			if (roll <= 0)
			{
				if (deb)
					Debug.Log(debug + " :: " + opt + "]");
				return opt;
			}
		}
		return null;
	}
	
	public static T WeightedRandomS<T>(Dictionary<T, float> dict,bool deb=false) where T : struct
	{
		string debug = "WR [";
		float total = 0;
		foreach (float w in dict.Values)
			total += w;
		if (deb)
			foreach (T t in dict.Keys)
				debug += t.ToString() + "." + dict[t] + " / ";
		float roll = UnityEngine.Random.Range(0f, total);
		debug += roll + "::" + total;
		foreach (T opt in dict.Keys)
		{
			roll -= dict[opt];
			if (roll <= 0)
			{
				if (deb)
					Debug.Log(debug + " :: " + opt + "]");
				return opt;
			}
		}
		return new T();
	}
	
	public static T WeightedRandomE<T>(Dictionary<T, float> dict,bool deb=false) where T : struct, IConvertible
	{
		string debug = "WR [";
		float total = 0;
		foreach (float w in dict.Values)
			total += w;
		if (deb)
			foreach (T t in dict.Keys)
				debug += t.ToString() + "." + dict[t] + " / ";
		float roll = UnityEngine.Random.Range(0f, total);
		debug += roll + "::" + total;
		foreach (T opt in dict.Keys)
		{
			roll -= dict[opt];
			if (roll <= 0)
			{
				if (deb)
					Debug.Log(debug + " :: " + opt + "]");
				return opt;
			}
		}
		return (T) Enum.ToObject(typeof(T), 0);
	}

	public static Dictionary<T, TU> Clone<T, TU>(this Dictionary<T, TU> d)
	{
		Dictionary<T,TU> r = new Dictionary<T, TU>();
		foreach(T k in d.Keys)
			r.Add(k,d[k]);
		return r;
	}

	public static List<T> Clone<T>(this List<T> l)
	{
		List<T> r = new List<T>();
		r.AddRange(l);
		return r;
	}
   
   public static int RoundRand(float n)
   {
	   int r = (int) n;
	   float rem = n - r;
	   if (UnityEngine.Random.Range(0f, 1f) <= rem)
		   r++;
	   return r;
   }
   
   public static T ToEnum<T>(int v) where T:struct, IConvertible
   {
	   return (T) Enum.ToObject(typeof(T),v);
   }
   
   public static Vector2Int Dir2Vector2Int(Directions d)
	{
		switch (d) {
		case Directions.Down:
			return new Vector2Int (0, -1);
		case Directions.Up:
			return new Vector2Int (0, 1);
		case Directions.Left:
			return new Vector2Int (-1, 0);
		case Directions.Right:
			return new Vector2Int (1, 0);
		case Directions.UL:
			return new Vector2Int (-1, 1);
		case Directions.UR:
			return new Vector2Int (1, 1);
		case Directions.DL:
			return new Vector2Int (-1, -1);
		case Directions.DR:
			return new Vector2Int (1, -1);
		}
		return new Vector2Int(0,0);
	}

	public static Directions GetDir(Vector2 start, Vector2 end, bool allowEightDir=false)
	{ return GetDir(end-start, out Directions b,allowEightDir); }

	public static Directions GetDir(Vector2 start, Vector2 end, out Directions backup, bool allowEightDir=false)
	{ return GetDir(end - start, out backup,allowEightDir); }

	public static Directions GetDir(Vector2 dir, bool allowEightDir=false)
	{ return GetDir(dir, out Directions b,allowEightDir); }

	public static Directions GetDir(Vector2 dir, out Directions backup, bool allowEightDir=false){
		dir = dir.normalized;
		backup = Directions.None;
//		Debug.Log("DIR: " + start + " / " + end + " / " + dir);
		if (dir.x >= Mathf.Abs(dir.y))
		{
			if (dir.y > 0) backup = Directions.Up;
			else if (dir.y < 0) backup = Directions.Down;
			if (allowEightDir && backup == Directions.Up) return Directions.UR;
			if (allowEightDir && backup == Directions.Down) return Directions.DR;
			return Directions.Right;
		}

		if (dir.x <= -Mathf.Abs(dir.y))
		{
			if (dir.y > 0) backup = Directions.Up;
			else if (dir.y < 0) backup = Directions.Down;
			if (allowEightDir && backup == Directions.Up) return Directions.UL;
			if (allowEightDir && backup == Directions.Down) return Directions.DL;
			return Directions.Left;
		}

		if (dir.y >= Mathf.Abs(dir.x))
		{
			if (dir.x > 0) backup = Directions.Right;
			else if (dir.x < 0) backup = Directions.Left;
			if (allowEightDir && backup == Directions.Right) return Directions.UR;
			if (allowEightDir && backup == Directions.Left) return Directions.UL;
			return Directions.Up;
		}

		if (dir.y <= -Mathf.Abs(dir.x))
		{
			if (dir.x > 0) backup = Directions.Right;
			else if (dir.x < 0) backup = Directions.Left;
			if (allowEightDir && backup == Directions.Right) return Directions.DR;
			if (allowEightDir && backup == Directions.Left) return Directions.DL;
			return Directions.Down;
		}

		return Directions.None;
	}
}

public enum Directions{
	None,
	Up,
	Down,
	Left,
	Right,
	UL,
	UR,
	DL,
	DR
}