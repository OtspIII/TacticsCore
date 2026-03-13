using System;
using TMPro;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public ActorThing Info;
    public SpriteRenderer Portrait;
    public SpriteRenderer SR;
    public SpriteRenderer Ring;
    public TextMeshPro HeadText;
    public HPBar HP;
    
    public void Setup(ActorThing src)
    {
        Info = src;
        Info.Body = this;
        TileController loc = God.GM.GetTile(Info.Location);
        // Debug.Log(Info.Class);
        if (Info.Class != CharClass.None)
        {
            Sprite s = God.Library.GetPortrait(Info.Class);
            if (s != null) Portrait.sprite = s;
            
            // Debug.Log("SPRITE: " + s);
        }
        else
        {
            Sprite s = God.Library.GetPortrait(Info.Type);
            if (s != null) SR.sprite = s;
            Ring.gameObject.SetActive(false);
            SR.gameObject.SetActive(true);
        }
        Audit();
    }

    private void Awake()
    {
        if(!God.GM.AllActors.Contains(this))
            God.GM.AllActors.Add(this);
    }

    private void OnDestroy()
    {
        God.GM.AllActors.Remove(this);
    }

    public void Audit()
    {
        TileController loc = God.GM.GetTile(Info.Location);
        transform.parent = loc.transform;
        transform.position = loc.GetContentPos(Info);
        HP.SetHP(Info.Get(IntStats.HP),Info.Get(IntStats.MaxHP),Info.Get(IntStats.Injury));
        HP.SetArmor(Info.Get(IntStats.Defense),Info.Get(IntStats.Armor));
        FakeDie(Info.Has(CTags.Corpse));
        SetRing();
    }

    public void Destruct()
    {
        Destroy(gameObject);
    }

    public void SetHeadtext(string txt,Colors c,float timer=1)
    {
        Instantiate(God.Library.HeadtextPrefab).Setup(this,txt,c,timer);
    }

    public void SetRing()
    {
        Color c = Ring.color;
        switch (Info.Team)
        {
            case GameTeam.None: c = Color.black; break;
            case GameTeam.Player: c = Color.forestGreen; break;
            case GameTeam.Enemy: c = Color.red; break;
            case GameTeam.Berserk: c = Color.darkMagenta; break;
        }
        Ring.color = c;
    }

    public void FakeDie(bool dead=true)
    {
        if (dead)
        {
            HP.gameObject.SetActive(false);
            Portrait.color = Color.gray;
        }
        else
        {
            HP.gameObject.SetActive(true);
            Portrait.color = Color.white;
        }
    }
}
