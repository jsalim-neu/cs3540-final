using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class ObjectiveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

*/

//types of objectives
public enum ObjectiveType
{
    MONEY,
    INTERACTION
}

//catch-all interface for different level objectives
public abstract class Objective
{

    //what is this objective's type?
    public ObjectiveType objType{get; set;}

    //current number of "objective points"
    public int objectiveCounter {get; set;}

    //goal number of "objective points"
    public int objectiveCountGoal {get; set;}

    //update objective count (if the event is of the correct type)
    public void ObjectiveUpdate(ObjectiveType type, int objectiveChange)
    {
        if (type == objType)
        {
            objectiveCounter += objectiveChange;
        }
    }

    public bool CheckAchieved()
    {
        return objectiveCounter >= objectiveCountGoal;
    }

}

// objective involving collecting money
public class MoneyObjective : Objective
{
    
    //constructor
    public MoneyObjective(int goalMoney)
    {
        objType = ObjectiveType.MONEY;
        objectiveCounter = 0;
        objectiveCountGoal = goalMoney;
        Debug.Log(objectiveCounter + ", " + objectiveCountGoal);
    }

    //if money is gained, increase objective count by that number
    

}

//objective involving interacting with something in the game world
public class InteractObjective : Objective
{
    
    //constructor with single interactable
    public InteractObjective()
    {
        objType = ObjectiveType.INTERACTION;
        objectiveCounter = 0;
        objectiveCountGoal = 1;
    }

    //constructor with multiple interactables
    public InteractObjective(int objectiveCountGoal)
    {
        objType = ObjectiveType.INTERACTION;
        objectiveCounter = 0;
        objectiveCountGoal = objectiveCountGoal;
    }


}