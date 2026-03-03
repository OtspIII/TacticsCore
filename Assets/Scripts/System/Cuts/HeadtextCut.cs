using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadtextCut : Cutscene
{
    ActorThing Who;
    string Text="";
    private int HP=-1;
    private int Def=-1;
    private int MaxHP=-1;
    int Injury=-1;
    public Colors C=Colors.Info;
    
    public HeadtextCut(ActorThing who,string txt,Colors c,float time=1)
    {
        Type = Cutscenes.Headtext;
        Who = who;
        Duration = time;
        Text = txt;
        C = c;
    }
    
    public HeadtextCut(ActorThing who,string txt,int hp, int def,int mhp,int inj,Colors c,float time=1)
    {
        Type = Cutscenes.Headtext;
        Who = who;
        Duration = time;
        Text = txt;
        HP = hp;
        Def = def;
        MaxHP = mhp;
        Injury = inj;
        C = c;
    }

    public override void OnBegin()
    {
        Who.Body.SetHeadtext(Text,C,Duration);
        if(HP != -1) Who.Body.HP.SetHP(HP,MaxHP,Injury);
        if(Def != -1) Who.Body.HP.SetArmor(Def,Who.Get(IntStats.Armor));
    }

}