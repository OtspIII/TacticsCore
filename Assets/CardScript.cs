using System;
using TMPro;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    public SpriteRenderer Image;
    public TextMeshPro Title;
    public TextMeshPro Desc;

    public EventInfo Event;

    public void SetEvent(EventInfo e)
    {
        Event = e;
    }

    void OnMouseDown()
    {
        God.GM.CurrentPhase.TakeEvent(Event);
    }

    public void Imprint(Sprite s, string title, params string[] desc)
    {
        if(Image != null) Image.sprite = s;
        if(Title != null) Title.text = title;
        if (Desc != null)
        {
            string d = "";
            foreach (string l in desc)
            {
                if (d != "") d += "\n";
                d += l;
            }
            Desc.text = d;
        }
    }

    public void Wipe()
    {
        Imprint(null,"");
    }
}
