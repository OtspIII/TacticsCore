using TMPro;
using UnityEngine;

public class HeadtextController : MonoBehaviour
{
    public ActorController Who;
    public TextMeshPro Text;
    public float Timer;
    public float MaxTimer;
    public float Dist = 1;
    public Vector3 Offset;

    public void Setup(ActorController ac, string txt,Colors c,float time=1)
    {
        Who = ac;
        transform.position = Who.transform.position + Offset;
        Text.text = txt;
        Text.color = God.Library.GetColor(c);
        Timer = time;
        MaxTimer = time;
    }

    void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer <= MaxTimer / 2)
        {
            transform.position += new Vector3(0, Time.deltaTime*Dist*2, 0);
            if(Timer <= 0) Destroy(gameObject);
        }
        
    }
    
    
}
