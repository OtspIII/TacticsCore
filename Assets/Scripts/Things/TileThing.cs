public class TileThing
{
    public int X;
    public int Y;
    public ActorThing Contents;
    public TileController Body;

    public TileThing(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public TileThing Neighbor(int x, int y)
    {
        return God.GM.Level.GetTile(X + x, Y + y);
    }
}