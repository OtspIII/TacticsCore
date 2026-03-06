
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DieRoll{
	public string Desc;
	public Number Rolls;
	public Number Size;
	public Number Bonus;
	public bool Setup;

	public DieRoll (int r, int s , int b){
		Desc = "";
		Rolls = God.N(r);
		Size = God.N(s);
		Bonus = God.N(b);
		Setup = true;
	}

	public DieRoll (string desc,ActorThing who=null)
	{
		Desc = desc;
		Rolls = God.N(1);
		Size = God.N(1);
		Bonus = God.N(0);
		Setup = true;
		string rl = "";
		string sz = "";
		string b = "";
			
		string sub = "";
		int stage = 0;
		try {
			for (int n = 0; n < desc.Length; n++) {
				string c = desc.Substring(n,1);
				if (c == "d") { // || c == "W" || c == "M" || c == "H"
					stage = 1;
					rl = sub;
					// if (Rolls == 0)
					// 	Rolls = 1;
					sub = "";
					
				} else if (c == "+" || c == "-") {
					stage = 2;
					sz = sub;// == "" ? 0 : int.Parse (sub);
					sub = c == "-" ? "-" : "";
				} else
					sub += c;
			}
			if (stage == 2 || stage == 0)
				b = sub;// == "" ? 0 : int.Parse (sub);
			else
				sz = sub;// == "" ? 0 : int.Parse (sub);
		} catch (System.Exception ex) {
			Debug.LogError (desc + " / " + rl + " / " + sz +
				" / " + b + " / " + sub + "." + ex.Message); 
		}
		if (b == "W")
		{
			sz = "W";
			b = "";
		}
		Rolls = ParseN(rl,who);
		if (Rolls.N == 0) Rolls.N = 1;
		Size = ParseN(sz,who,out int rm,out int bm);
		Bonus = ParseN(b,who);
		Bonus.N += bm * rm; //These mults are maybe weird. Should test once more stuff is in
		Rolls.Mult *= rm;
	}

	public Number ParseN(string s, ActorThing who)
	{
		return ParseN(s, who, out int a, out int b);
	}
	
	public Number ParseN(string s, ActorThing who,out int diem, out int bm)
	{
		diem = 1;
		bm = 0;
		int n = 0;
		IntStats st = IntStats.None;
		int m = 1;
		if (s.Contains("W"))
		{
			DieRoll dr = who.BaseDamage;
			n = dr.Size.V(who);
			diem = dr.Rolls.V(who);
			bm = dr.Bonus.V(who);
			if (bm == 0) bm = 1;
			s = s.Substring(0,s.IndexOf("W"));
		}
		else if (s.Contains("M"))
		{
			st = IntStats.MoveLeft;
			s = s.Substring(0,s.IndexOf("M"));
		}
		else if (s.Contains("D"))
		{
			st = IntStats.MoveLeft;
			s = s.Substring(0,s.IndexOf("D"));
		}
		else if (s.Contains("H"))
		{
			st = IntStats.HP;
			s = s.Substring(0,s.IndexOf("H"));
		}

		int tempN = 0;
		if (int.TryParse(s, out int num)) tempN = num;
		if (st == IntStats.None && s != "") n = tempN;
		else m = tempN;
		if (m == 0) m = 1;
		if (st == IntStats.None) return God.N(n, m);
		else return God.N(who,st, m);
	}

	
	public int[] Calculate(ActorThing who,ActionScript act=null,GEvents msg=GEvents.None){
		int[] r = new int[3]{Rolls.V(who),Size.V(who),Bonus.V(who)};
		return r;
	}

	public int Roll(ActorThing who=null,EventInfo e=null, ActionScript act=null, RollMode mode=RollMode.Normal){

		int[] nums = Calculate (who,act);//e!=null?e.Type:GEvents.None
//		Debug.Log(nums[0] + " / " + nums[1] + " / " + nums[2] + " / " + who + " / " + e + " / " + act);
		switch (mode)
		{
			case RollMode.Min:
				return nums[2] + nums[0];
			case RollMode.Max:
				return nums[2] + (nums[0] * nums[1]);
				
		}
		return God.Roll(nums[0],nums[1],nums[2]);
	}

	public override string ToString()
	{
		return "Die Roll[" + Desc +" / " + Rolls.V() + " / " + Size.V() + " / " + Bonus.V() + "\n/" + Rolls + " / " + Size + " / " + Bonus + "]";
	}
}

public class Number
{
	public int N = 0;
	public IntStats Stat = IntStats.None;
	public ActorThing Who=null;
	public int Mult = 1;

	public Number(int n,int m=1)
	{
		N = n;
		Mult = m;
	}

	public Number(ActorThing who,IntStats s,int m=1)
	{
		Stat = s;
		Who = who;
		Mult = m;
	}
	
	public Number(IntStats s,int m=1)
	{
		Stat = s;
		Mult = m;
	}

	
	
	public int V(ActorThing who=null)
	{
		if (who == null) who = Who;
		int st = Stat != IntStats.None ? who.Get(Stat) : 0;
		return (N + st) * Mult;
	}
	
	public static Number Zero
	{
		get { return new Number(0); }
	}

	public override string ToString()
	{
		return "N[" + N + " / " + Stat + " / " + Mult + " / " + (Who != null ? Who.Class : "")+"]";
	}
}