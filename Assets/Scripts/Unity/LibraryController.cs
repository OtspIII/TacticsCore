using System.Collections.Generic;
using UnityEngine;

public class LibraryController : MonoBehaviour
{
    public ActorController ActorPrefab;
    public TileController TilePrefab;
    public CardScript CardPrefab;
    public HeadtextController HeadtextPrefab;
    
    protected Dictionary<string,Sprite> FeatureArt = new Dictionary<string, Sprite>();
    protected Dictionary<string,Sprite> ClassPortraits = new Dictionary<string, Sprite>();
    protected Dictionary<string,Sprite> ClassArt = new Dictionary<string, Sprite>();
    protected Dictionary<string,Sprite> Icons = new Dictionary<string, Sprite>();
    
    void Awake()
    {
		God.Library = this;
		LoadResources ();
    }

    void LoadResources()
    {
	    Object[] res = Resources.LoadAll ("ClassPortraits", typeof(Sprite));
	    foreach (Object o in res) {
		    Sprite s = (Sprite)o;
		    ClassPortraits.Add(s.name,s);
	    }
	    res = Resources.LoadAll ("Features", typeof(Sprite));
	    foreach (Object o in res) {
		    Sprite s = (Sprite)o;
		    FeatureArt.Add(s.name,s);
	    }
	    res = Resources.LoadAll ("Classes", typeof(Sprite));
	    foreach (Object o in res) {
		    Sprite s = (Sprite)o;
		    ClassArt.Add(s.name,s);
	    }
	    res = Resources.LoadAll ("Icons", typeof(Sprite));
	    foreach (Object o in res) {
		    Sprite s = (Sprite)o;
		    Icons.Add(s.name,s);
	    }
    }

    public Sprite GetPortrait(Actors a)
    {
	    return FeatureArt.TryGetValue(a.ToString(), out Sprite r) ? r : null;
    }
    public Sprite GetPortrait(CharClass a)
    {
	    // if (ClassArt.TryGetValue(a.ToString(), out Sprite art)) return art;
	    return ClassPortraits.TryGetValue(a.ToString(), out Sprite r) ? r : null;
    }
    public Sprite GetArt(CharClass a)
    {
	    if (ClassArt.TryGetValue(a.ToString(), out Sprite art)) return art;
	    return null;
    }
    
    public Sprite GetIcon(string a)
    {
	    return Icons.TryGetValue(a.ToString(), out Sprite r) ? r : null;
    }

    public Color GetColor(Colors c)
    {
	    switch (c)
	    {
		    case Colors.Damage: return Color.red;
		    case Colors.Resist: return Color.cornflowerBlue;
		    case Colors.Healing: return Color.limeGreen;
	    }
	    return Color.white;
    }
}

[System.Serializable]
public class CharInfo
{
    public string Name;
    public Sprite Art;
}