using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dictionary<int, Dictionary<int, TileController>> Map = new Dictionary<int, Dictionary<int, TileController>>();
    public List<TileController> AllTiles = new List<TileController>();
    public List<ActorController> AllActors = new List<ActorController>();
    public LevelThing Level;

    public PhaseScript CurrentPhase;
    public Cutscene CurrentCut;
    public List<Cutscene> Cuts = new List<Cutscene>();
    public GameObject BotBar;
    public GameObject BBCursor;
    public Vector3 CursorStart;
    public List<CardScript> CardLine = new List<CardScript>();
    public CardScript MainCard;

    void Awake()
    {
        God.GM = this;
        Parser.Init();
        ThingBuilder.Setup();
        CursorStart = BBCursor.transform.position;
        MainCard.Wipe();
    }

    void Start()
    {
        WipeLevel();
        StartPhase(new BuildLevelPhase());
    }
    
    void Update(){
        if (CurrentCut != null)
        {
            CurrentCut.Run();
        }
        else if (Cuts.Count > 0)
        {
            CurrentCut = Cuts[0];
            Cuts.Remove(CurrentCut);
            CurrentCut.Begin();
        }
        else if(CurrentPhase != null)  CurrentPhase.Run();
    }

    public void AddCut(Cutscene c)
    {
        if (Cuts.Count > 0)
        {
            if (Cuts[Cuts.Count - 1].Merge(c)) return;
        }
        Cuts.Add(c);
    }

    public void EndCut(Cutscene c)
    {
        if (CurrentCut == c)
        {
            CurrentCut = null;
            if (Cuts.Count == 0)
            {
                Audit();
                CurrentPhase.Resume();
            }
                
        }
    }

    public void StartPhase(PhaseScript p=null)
    {
        if (p == null)
        {
            if (CurrentPhase != null) p = CurrentPhase.NextPhase();
            if(p == null) p = new PlayerTurnPhase();
        }
        if(CurrentPhase != null) CurrentPhase.End();
        CurrentPhase = p;
        CurrentPhase.Begin();
    }

    public void Audit()
    {
        foreach (ActorController a in AllActors)
            a.Audit();
        foreach (TileController a in AllTiles)
            a.Audit();
    }


    public void WipeLevel()
    {
        foreach (ActorController a in AllActors.ToArray()) Destroy(a);
        foreach (TileController t in AllTiles.ToArray()) Destroy(t);
        AllActors.Clear();
        AllTiles.Clear();
        Map.Clear();
    }

    public TileController SpawnTile(GameTile t)
    {
        TileController r = Instantiate(God.Library.TilePrefab);
        r.Setup(t);
        if(!Map.ContainsKey(t.X)) Map.Add(t.X,new Dictionary<int, TileController>());
        Map[t.X].Add(t.Y,r);
        return r;
    }
    
    public ActorController SpawnActor(ActorThing t)
    {
        ActorController r = Instantiate(God.Library.ActorPrefab);
        r.Setup(t);
        return r;
    }
    
    public TileController GetTile(int x, int y)
    {
        if (!Map.ContainsKey(x)) return null;
        return Map[x].TryGetValue(y, out TileController r) ? r : null;
    }

    public TileController GetTile(GameTile t)
    {
        return GetTile(t.X, t.Y);
    }

    public ActorController GetActor(ActorThing a)
    {
        return a.Body;
    }

    public List<ActorThing> GetActors()
    {
        List<ActorThing> r = new List<ActorThing>();
        foreach (ActorController a in AllActors)
        {
            r.Add(a.Info);
        }

        return r;
    }

    public void TileClick(GameTile t)
    {
        if (CurrentCut != null) CurrentCut.TileClick(t);
        else CurrentPhase.TileClick(t);
    }
    
    public void TileMouseEnter(GameTile t)
    {
        t.GetThing()?.ImprintCard(MainCard);
    }
    
    public void TileMouseExit(GameTile t)
    {
        MainCard.Wipe();
    }

    public void TakeEvent(EventInfo e)
    {
        if(CurrentPhase.Listeners.Contains(e.Type))
            CurrentPhase.TakeEvent(e);
    }

    public void SpawnCard(Thing t)
    {
        CardScript c = Instantiate(God.Library.CardPrefab, BBCursor.transform.position, Quaternion.identity);
        c.transform.localScale = new Vector3(God.CardSize, God.CardSize, 1);
        c.transform.parent = transform;
        t.ImprintCard(c);
        c.SetEvent(God.E(EventTypes.SelectCard).Set(t));
        BBCursor.transform.position += new Vector3(God.CardSize, 0, 0);
        CardLine.Add(c);
    }

    public void WipeCards()
    {
        foreach (CardScript c in CardLine)
        {
            Destroy(c.gameObject);
        }
        CardLine.Clear();
        BBCursor.transform.position = CursorStart;
    }

    public void CalcMapPDist(GameTeam team=GameTeam.Enemy)
    {
        foreach (TileController t in AllTiles)
        {
            t.Info.PDistance.Clear();
            t.Info.BestPDistance = 999;
        }
        foreach (ActorController ac in AllActors)
        {
            ActorThing a = ac.Info;
            if (a.Team == GameTeam.None || (a.Team == team && team != GameTeam.Berserk)) continue;
            List<GameTile> active = new List<GameTile>() { a.Location };
            a.Location.PDistance.Add(a,0);
            int safety = 999;
            while (active.Count > 0 && safety > 0)
            {
                safety--;
                GameTile t = active[0];
                active.RemoveAt(0);
                int dist = t.PDistance[a];
                foreach (GameTile n in t.Neighbors())
                {
                    if (n.PDistance.ContainsKey(a))
                    {
                        if (n.PDistance[a] > dist + 1)
                        {
                            if(!active.Contains(n))active.Add(n);
                            n.PDistance[a] = dist + 1;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if(!active.Contains(n))active.Add(n);
                        n.PDistance.Add(a, dist + 1);
                    }
                }
            }
        }
        foreach (TileController t in AllTiles)
        {
            t.Info.BestPDistance = Mathf.Min(t.Info.PDistance.Values.ToArray());
            // t.DebugTxt.text = t.Info.BestPDistance.ToString();
        }
    }
}
