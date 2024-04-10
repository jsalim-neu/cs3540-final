using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//use to preserve information across levels
public class FlagManager : MonoBehaviour
{
    public static bool 
        spawnEnemies = false,
        playerHasGrenades = true,
        playerHasPulse = false,
        playerHasHoming = true
        ;
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void InitializeFlags()
    {
        spawnEnemies = false;
        playerHasGrenades = false;
        playerHasPulse = false;
        playerHasHoming = false;
    }

    public static void SetAllTrue()
    {
        spawnEnemies = true;
        playerHasGrenades = true;
        playerHasPulse = true;
        playerHasHoming = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
