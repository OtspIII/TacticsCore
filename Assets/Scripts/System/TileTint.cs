using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileTint
{
    public List<GameTile> Tiles = new List<GameTile>();
    public TileTints C;
    public int Priority;
    
    public TileTint(TileTints c,params GameTile[] tiles)
    {
        Tiles.AddRange(tiles);
        C = c;
        Priority = (int)c;
        Apply();
    }
    
    public TileTint(TileTints c,List<GameTile> tiles)
    {
        Tiles = tiles;
        C = c;
        Priority = (int)c;
        Apply();
    }
    
    public void Apply()
    {
        foreach (GameTile g in Tiles)
        {
            g.AddTint(this);
        }
    }
    
    public void End()
    {
        foreach (GameTile g in Tiles)
        {
            g.RemoveTint(this);
        }
    }

    public Color GetColor()
    {
        switch (C)
        {
            case TileTints.GoodOption: return Color.green;
            case TileTints.OkayOption: return Color.yellow;
            case TileTints.ActiveThing: return Color.cornflowerBlue;
            case TileTints.Harmful: return Color.red;
            case TileTints.Path: return Color.teal;
        }
        return Color.magenta;
    }
}