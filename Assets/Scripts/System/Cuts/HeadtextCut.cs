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
    
    public HeadtextCut(ActorThing who,string txt,float time=1)
    {
        Type = Cutscenes.Headtext;
        Who = who;
        Duration = time;
        Text = txt;
    }
    
    public HeadtextCut(ActorThing who,string txt,int hp, int def,float time=1)
    {
        Type = Cutscenes.Headtext;
        Who = who;
        Duration = time;
        Text = txt;
        HP = hp;
        Def = def;
    }

    public override void OnBegin()
    {
        Who.Body.SetHeadtext(Text);
    }

    public override void OnEnd()
    {
        Who.Body.SetHeadtext("");
    }

}