using TMPro;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    public SpriteRenderer BG;
    public SpriteRenderer HP;
    public SpriteRenderer Healable;
    public SpriteRenderer Injury;
    public SpriteRenderer Def;
    // public SpriteRenderer DropHP;
    // public SpriteRenderer TempHP;
    // public SpriteRenderer Armor;
    // public SpriteRenderer DropDef;
    public ActorController Who;

    // public Color ArmorC;
    // public Color MArmorC;
    // public Color DArmorC;
    // public Color MDamC;
    // public Color DDamC;
    
    public float HPFlat = 1;
    public float HPFlatHeal = 1;
    public float HPFlatMax = 1;
    public float Max = 1;
    public float DFlat = 0;
    public float DFlatMax = 1;
    public float Perc = 1;
    public float HealPerc = 1;
    public float DPerc = 0;
    public float PercShown = 1;
    public float DPercShown = 0;

    void Start()
    {
        SetHP(Who.Info.Get(IntStats.HP),Who.Info.Get(IntStats.MaxHP),Who.Info.Get(IntStats.Injury));
        SetArmor(Who.Info.Get(IntStats.Defense),Who.Info.Get(IntStats.Armor));
        Calc();
        PercShown = Perc;
        DPercShown = DPerc;
        HP.transform.localScale = new Vector3(Perc, 1,1);
        Def.transform.localScale = new Vector3(DPerc, 0.5f,1);
        Healable.transform.localScale = new Vector3(1, 1,1);
    }
    
    void Update()
    {
        if (Perc != PercShown)
        {
            PercShown = Mathf.Lerp(PercShown,Perc, 0.01f);
            PercShown = Mathf.MoveTowards(PercShown,Perc, 0.001f);
            HP.transform.localScale = new Vector3(PercShown, 1,1);
        }
        if (DPerc != DPercShown)
        {
            DPercShown = Mathf.Lerp(DPercShown,DPerc, 0.01f);
            DPercShown = Mathf.MoveTowards(DPercShown,DPerc, 0.001f);
            Def.transform.localScale = new Vector3(DPercShown, 0.5f,1);
        }
    }
    

    public void SetHP(float current,float max,float inj)
    {
        HPFlat = current;
        HPFlatMax = max;
        HPFlatHeal = max - inj;
        Calc();
    }
    
    public void SetArmor(float current,float max)
    {
        DFlat = current;
        DFlatMax = max;
        Calc();
    }

    public void Calc()
    {
        Max = Mathf.Max(HPFlatMax, DFlatMax,1);
        Perc = HPFlat/Max;
        HealPerc = HPFlatHeal/Max;
        DPerc = DFlat/Max;
        Healable.transform.localScale = new Vector3(HealPerc, 1,1);
        if (HPFlatMax < Max)
        {
            Injury.transform.localScale = new Vector3(HPFlatMax/Max, 1,1);
        }
    }
    
}
