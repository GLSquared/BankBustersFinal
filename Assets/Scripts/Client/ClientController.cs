using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientController : MonoBehaviour
{
    List<string> invItems = new List<string>();

    float MaxHealthBarHeight = 261.34f;
    float Health = 100;
    public bool Dead = false;
    public GameObject HealthBar;
    GameObject HackingBar;
    GameObject DeathScreen;

    float eatTime;
    GameObject currentDonut;

    ClientWeaponController cwc;

    // Start is called before the first frame update
    void Start()
    {
        HackingBar = GameObject.FindGameObjectWithTag("HackingBar");
        HackingBar.SetActive(false);

        DeathScreen = HackingBar.transform.parent.Find("DeathScreen").gameObject;
        DeathScreen.SetActive(false);

        cwc = gameObject.GetComponent<ClientWeaponController>();

    }

    // Update is called once per frame
    void Update()
    {
        //;
        HealthBarUpdate();


        //Reduce damage screen opacity
        HealthBar.transform.parent.parent.Find("DamageScreen").GetComponent<Image>().color = new Color(1, 1, 1, HealthBar.transform.parent.parent.Find("DamageScreen").GetComponent<Image>().color.a - (Time.deltaTime * 5));

        if (Input.GetKeyDown(KeyCode.C)) {
            //CALL RESPAWN METHOD
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseItem(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UseItem(2);
        }

        if (currentDonut != null)
        {
            currentDonut.transform.localPosition = new Vector3(-.15f, -.1f + Mathf.Sin(Time.time * 30) / 100f, .15f);
            currentDonut.transform.localRotation = Quaternion.Euler(40, 40, 0);
            if (eatTime < Time.time)
            {
                Destroy(currentDonut);
            }
        }

    }

    public void Die() {
        if (!Dead) {
            Dead = true;
            DeathScreen.SetActive(true);
            gameObject.GetComponent<CapsuleCollider>().height = .5f;
            
        }
    }

    public void ReduceHealth(float damage)
    {
        Health = Mathf.Max(0, Health - damage);
        HealthBar.transform.parent.parent.Find("DamageScreen").GetComponent<Image>().color = new Color(1, 1, 1, 1);

        if (Health <= 0)
        {
            Die();
        }

    }

    IEnumerator IncreaseBar(float progressTime) {
        float maxFrames = Mathf.Floor(progressTime / Time.deltaTime);
        for (int i = 0; i < maxFrames; i++) {
            HackingBar.transform.Find("Bar").localScale = new Vector3(i / maxFrames, 1, 1);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        HackingBar.SetActive(false);
    }

    public void ShowHackingProgress(float progressTime) {
        HackingBar.SetActive(true);
        StartCoroutine(IncreaseBar(progressTime));

    }

    void UseItem(int itemIndex)
    {
        if (invItems.Count > itemIndex)
        {
            string currentItem = invItems[itemIndex];

            if (currentItem == "Health Kit")
            {
                Health = Mathf.Min(100, Health + 60);
            }

            if (currentItem == "Donut")
            {
                Health = Mathf.Min(100, Health + 30);
                EatDonut();
            }

            invItems.RemoveAt(itemIndex);
            UpdateHotbars();

        }
    }

    void EatDonut()
    {
        if (currentDonut == null)
        {
            currentDonut = (GameObject)Instantiate(Resources.Load("ViewportItems/Donut"));
            currentDonut.transform.parent = Camera.main.transform;
            eatTime = Time.time + 2;
        }
    }

    void HealthBarUpdate()
    {
        float HealthBarHeight = (Health / 100f) * MaxHealthBarHeight;
        HealthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(26.18f, HealthBarHeight);
        HealthBar.GetComponent<RectTransform>().localPosition = new Vector3(0, (HealthBarHeight / 2f) - (MaxHealthBarHeight / 2), 0);
        HealthBar.GetComponent<Image>().color = Color.Lerp(new Color(0, 1, 0, .4f), new Color(1, 0, 0, .4f), 1 - (Health / 100f));


    }

    void UpdateHotbars()
    {
        for (int i = 0; i < 3; i++)
        {
            if (invItems.Count >= (i + 1))
            {
                cwc.SetHotbar(i + 3, invItems[i], false);
            }
            else
            {
                cwc.SetHotbar(i + 3, null, false);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Collectible"))
        {

            //add item to inventory
            if (invItems.Count < 3)
            {
                GameObject item = collision.gameObject;
                invItems.Add(item.name);
                UpdateHotbars();
                Destroy(item);
            }

        }
    }
}
