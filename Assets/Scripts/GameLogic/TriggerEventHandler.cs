using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TriggerEventHandler : MonoBehaviour
{    
    [SerializeField] 
    public string TriggerName;
    [SerializeField] 
    private bool OneTime;
    [SerializeField] 
    public int TriggerLevel;

    private string TargetTag = "Player";
    
    [SerializeField] 
    public bool IsPopup;
    [SerializeField] 
    public string PopupText;

    GameManager gm;

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Activate()
    {
        gm.InvokeTriggerEvent(TriggerName);

        if (OneTime)
            GetComponent<BoxCollider>().enabled = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == TargetTag)
        {
            if (!IsPopup)
            {
                Activate();
            } else {
                ClientWeaponController client   = collider.gameObject.GetComponent<ClientWeaponController>();
                client.interactTrigger          = gameObject;
                client.SetPopupText(PopupText);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == TargetTag && IsPopup)
        {
            ClientWeaponController client   = collider.GetComponent<ClientWeaponController>();
            client.interactTrigger          = null;
            client.SetPopupText("");
        }
    }
}
