
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DieRoll{
	public string Desc;
	public int Rolls;
	public int Size;
	public int Bonus;

	public DieRoll (int r, int s , int b){
		Desc = "";
		Rolls = r;
		Size = s;
		Bonus = b;
	}

	public DieRoll (string desc)
	{
		Desc = desc;
		Rolls = 0;
		Size = 0;
		Bonus = 0;
			
		string sub = "";
		int stage = 0;
//		bool bonusTime = false;
		try {
			for (int n = 0; n < desc.Length; n++) {
				string c = desc.Substring(n,1);
				if (c == "d" || c == "W" || c == "M" || c == "H") {
					stage = 1;
					Rolls = sub == "" ? 0 : int.Parse (sub);
					if (Rolls == 0)
						Rolls = 1;
					sub = "";
					
				} else if (c == "+" || c == "-") {
					stage = 2;
					Size = sub == "" ? 0 : int.Parse (sub);
					sub = c == "-" ? "-" : "";
				} else
					sub += c;
			}
			if (stage == 2 || stage == 0)
				Bonus = sub == "" ? 0 : int.Parse (sub);
			else
				Size = sub == "" ? 0 : int.Parse (sub);
		} catch (System.Exception ex) {
			Debug.LogError (desc + " / " + Rolls + " / " + Size +
				" / " + Bonus + " / " + sub + "." + ex.Message); 
		}

//		Debug.Log (desc + " / " + Rolls + " / " + Size + " / " + Bonus + " / " + sub + "."); 
	}

	
	public int[] Calculate(ActorThing who,ActionScript act=null,GEvents msg=GEvents.None){
		int[] r = new int[3]{Rolls,Size,Bonus};
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
		return "Die Roll[" + Desc + "]";
	}
}
