using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//use to preserve information across levels
public class FlagManager : MonoBehaviour
{
    public static bool spawnEnemies = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void InitializeFlags()
    {
        spawnEnemies = false;

    }

    // Update is called once per frame
    void Update()
    {
    }
}
