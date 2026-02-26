using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadtextCut : Cutscene
{
    ActorThing Who;
    string Text="";
    
    public HeadtextCut(ActorThing who,string txt,float time=1)
    {
        Type = Cutscenes.Headtext;
        Who = who;
        Duration = time;
        Text = txt;
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