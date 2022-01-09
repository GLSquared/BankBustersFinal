using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void InvokeTriggerEvent(string triggerName)
    {
        triggerName = triggerName.ToLower();

        switch(triggerName)
        {
            case "test": 
                print("bruh");
                break;
            default:
                print("Unknown event name ?"+triggerName);
                break;
        }

        print("Triggering event: "+triggerName+".");
    }
}
