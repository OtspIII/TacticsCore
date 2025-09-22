public class ActorThing
{
    public Actors Type;
    public Classes Class = Classes.None;
    public TileThing Location;
    public ActorController Body;

    public ActorThing(Actors type,TileThing l)
    {
        Type = type;
        Setup(l);
    }
    
    public ActorThing(Classes type,TileThing l)
    {
        Type = Actors.Character;
        Class = type;
        Setup(l);
    }

    private void Setup(TileThing l)
    {
        SetLocation(l);
    }

    public void SetLocation(TileThing l)
    {
        Location = l;
        l.Contents = this;
    }
}
