using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterAnim : MonoBehaviour
{


    public bool animPlaying = false;
    float rotorSpeed = 100f;
    float tilt = 0f;

    [SerializeField]
    GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animPlaying) {


            if (!cam.activeSelf) {
                cam.SetActive(true);
            }

            cam.transform.position -= new Vector3(Time.deltaTime/3f, 0, Time.deltaTime / 3f);

            cam.transform.LookAt(transform.position);

            rotorSpeed = Mathf.Min(1500f, rotorSpeed + Time.deltaTime * 150f);

            transform.Find("SM_Veh_Helicopter_Blades_Back_01").localRotation = Quaternion.Euler(Time.time * rotorSpeed, 0, 0);
            transform.Find("SM_Veh_Helicopter_Blades_Main_01").localRotation = Quaternion.Euler(0, Time.time * rotorSpeed, 0);

            if (rotorSpeed >= 1000f) {
                transform.position += new Vector3(0, Time.deltaTime*((rotorSpeed-1000)/100f), 0);

                transform.rotation = Quaternion.Euler((Mathf.Sin(Time.time * 5f) * 1.5f) + tilt, 90f, Mathf.Sin(Time.time*5f)*1.5f);

            }

            if (rotorSpeed >= 1400) {
                transform.position += new Vector3(Time.deltaTime * (tilt/1.5f), 0, 0);
                tilt = Mathf.Lerp(tilt, 13, Time.deltaTime);
            }



        }
    }
}
