using System.Collections.Generic;
using UnityEngine;

public class LibraryController : MonoBehaviour
{
    public ActorController ActorPrefab;
    public TileController TilePrefab;
    
    protected Dictionary<string,Sprite> ClassPortraits = new Dictionary<string, Sprite>();
    
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
    }

    public Sprite GetPortrait(Actors a)
    {
	    return ClassPortraits.TryGetValue(a.ToString(), out Sprite r) ? r : null;
    }
    public Sprite GetPortrait(Classes a)
    {
	    return ClassPortraits.TryGetValue(a.ToString(), out Sprite r) ? r : null;
    }
}

[System.Serializable]
public class CharInfo
{
    public string Name;
    public Sprite Art;
}