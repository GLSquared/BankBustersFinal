using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientController : MonoBehaviour
{
    List<string> invItems = new List<string>();

    float MaxHealthBarHeight = 261.34f;
    float Health = 100;
    public GameObject HealthBar;
    GameObject HackingBar;
    GameObject DeathScreen;
    GameObject BossHealthPanel;
    GameObject ObjectivesPanel;

    
    public float currentMoney = 0f;
    float targMoney;

    public bool Dead;

    float eatTime;
    GameObject currentDonut;

    ClientWeaponController cwc;
    
    private IEnumerator hackCoroutine;

    [SerializeField]
    GameObject MoneyEffect;

    // Start is called before the first frame update
    void Start()
    {

        targMoney = 5000000f;

        HackingBar = GameObject.FindGameObjectWithTag("HackingBar");
        HackingBar.SetActive(false);

        DeathScreen = HackingBar.transform.parent.Find("DeathScreen").gameObject;
        DeathScreen.SetActive(false);

        BossHealthPanel = HackingBar.transform.parent.Find("BossHealth").gameObject;
        BossHealthPanel.SetActive(false);

        ObjectivesPanel = HackingBar.transform.parent.Find("ObjectivePanel").gameObject;

        cwc = gameObject.GetComponent<ClientWeaponController>();
    }

    // Update is called once per frame
    void Update()
    {
        //;
        HealthBarUpdate();

        currentMoney = Mathf.Lerp(currentMoney, targMoney, Time.deltaTime * 10f);

        GameObject.FindGameObjectWithTag("MoneyText").GetComponent<TextMeshProUGUI>().text = "$" + string.Format("{0:n0}", Mathf.Ceil(currentMoney));

        GameObject Boss = null;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            if (enemy.name.Contains("Boss")) {
                Boss = enemy;
            }
        }

        if (Boss != null) {
            BossHealthPanel.SetActive(true);
            float bHealth = Boss.GetComponent<Enemy>().health;
            float mbHealth = Boss.GetComponent<Enemy>().MaxHealth;
            BossHealthPanel.transform.Find("Bar").transform.localScale = new Vector3(bHealth / mbHealth, 1, 1);
            BossHealthPanel.transform.Find("Health").GetComponent<TextMeshProUGUI>().text = bHealth + "/" + mbHealth;
            ObjectivesPanel.transform.Find("ObjectiveText").GetComponent<TextMeshProUGUI>().text = "Fight off the security!";
        }
        else
        {
            if (GameObject.Find("Helicopter") == null) {
                ObjectivesPanel.transform.Find("ObjectiveText").GetComponent<TextMeshProUGUI>().text = "Find and hack the console!";
            }
            else
            {
                ObjectivesPanel.transform.Find("ObjectiveText").GetComponent<TextMeshProUGUI>().text = "Escape with the helicopter!";
            }
            BossHealthPanel.SetActive(false);
        }


        //OBJECTIVES
        Vector2 objPanelPos = new Vector2(-260, 190);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            objPanelPos = new Vector2(0, 190);
            ObjectivesPanel.transform.Find("Hint").GetComponent<TextMeshProUGUI>().enabled = false;
            ObjectivesPanel.transform.Find("HintB").GetComponent<TextMeshProUGUI>().enabled = false;
        }
        else {
            ObjectivesPanel.transform.Find("Hint").GetComponent<TextMeshProUGUI>().enabled = true;
            ObjectivesPanel.transform.Find("HintB").GetComponent<TextMeshProUGUI>().enabled = true;
        }

        ObjectivesPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(ObjectivesPanel.GetComponent<RectTransform>().anchoredPosition, objPanelPos, Time.deltaTime * 5f);

        //Reduce damage screen opacity
        HealthBar.transform.parent.parent.Find("DamageScreen").GetComponent<Image>().color = new Color(1, 1, 1, HealthBar.transform.parent.parent.Find("DamageScreen").GetComponent<Image>().color.a - (Time.deltaTime * 5));

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
        if (Input.GetKeyDown(KeyCode.C) && Dead)
        {
            StopHackingProgress();
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().InvokeTriggerEvent("restartlevel");
            Dead = false;
            Health = 100f;
            DeathScreen.SetActive(false);
            GetComponent<CapsuleCollider>().height = 1.5f;
            GetComponent<Movement>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SelfDestroy();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            gm.LoadMainMenu();
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

    void Die() {
        if (!Dead) {
            Dead = true;
            DeathScreen.SetActive(true);
            GetComponent<CapsuleCollider>().height = .8f;
            GetComponent<Movement>().enabled = false;
        }
    }

    private IEnumerator NoMyMoney()
    {
        GameObject m = Instantiate(MoneyEffect, transform.position + new Vector3(0, 2, 0), Quaternion.identity);

        float maxFrames = Mathf.Floor(15 / Time.deltaTime);

        for (int i = 0; i < maxFrames; i++) {
            m.transform.localPosition -= new Vector3(0, Time.deltaTime, 0);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Destroy(m);
    }

    public void ReduceHealth(float damage)
    {   
        StartCoroutine(NoMyMoney());
        targMoney -= Random.Range(10000f, 50000f);

        Health = Mathf.Max(0, Health - damage);
        HealthBar.transform.parent.parent.Find("DamageScreen").GetComponent<Image>().color = new Color(1, 1, 1, 1);

        if (Health <= 0) {
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

    IEnumerator SpeedBoost()
    {
        Movement mov = GetComponent<Movement>();
        mov.speedWalking = 8f;
        mov.speedRunning = 14f;
        yield return new WaitForSeconds(20);
        mov.speedWalking = 5f;
        mov.speedRunning = 9f;
    }
    
    public void ShowHackingProgress(float progressTime) {
        HackingBar.SetActive(true);
        hackCoroutine = IncreaseBar(progressTime);
        StartCoroutine(hackCoroutine);
    }

    public void StopHackingProgress() {
        HackingBar.SetActive(false);
        if (hackCoroutine != null)
        {
            StopCoroutine(hackCoroutine);
            hackCoroutine = null;
        }
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

            if (currentItem == "Soda")
            {
                StartCoroutine(SpeedBoost());
                DrinkSoda();
            }          

            invItems.RemoveAt(itemIndex);
            UpdateHotbars();
        }
    }

    void DrinkSoda()
    {
        if (currentDonut == null)
        {
            currentDonut = (GameObject)Instantiate(Resources.Load("ViewportItems/Soda"));
            currentDonut.transform.parent = Camera.main.transform;
            eatTime = Time.time + 2;
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

    void SelfDestroy()
    {
        ReduceHealth(300);
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
