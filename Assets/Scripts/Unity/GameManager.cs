using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        God.GM = this;
    }
}
