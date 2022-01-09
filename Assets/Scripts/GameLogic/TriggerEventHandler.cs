using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TriggerEventHandler : MonoBehaviour
{    
    [SerializeField] string TriggerName;
    [SerializeField] string TargetTag;
    [SerializeField] bool OneTime;

    GameManager gm;

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == TargetTag)
        {
            gm.InvokeTriggerEvent(TriggerName);

            if (OneTime)
                Destroy(gameObject);
        }
    }
}
