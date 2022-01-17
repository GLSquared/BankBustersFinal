using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDestroy : MonoBehaviour
{

    private float waitTime = 20f;
    public bool isPickedUp;

    // Update is called once per frame
    void Update()
    {
        if (waitTime <= 0 && !isPickedUp)
        {
            Destroy(gameObject);
        }
        else if (waitTime >= 0 && isPickedUp)
        {
            this.enabled = false;
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
    }
}
