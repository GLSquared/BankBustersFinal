using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using InfimaGames.LowPolyShooterPack;

public class ClientWeaponController : MonoBehaviour
{
    //Get the fps controller script to set the current weapon transform.
    [SerializeField]
    private GameObject InvObject; 

    //create a variable for both primary and secondary weapons. They are public variables so that the weapons can be changed from other classes in runtime
    public GameObject secondary;
    public GameObject primary;
    public GameObject hotbarUI;
    GameObject gunDrop;
    bool isOnPrimary;
    
    GameObject PopupText;
    public GameObject interactTrigger;

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    // Sets the popup text for the Client
    public void SetPopupText(string str)
    {
        PopupText.GetComponent<TextMeshProUGUI>().text = str;
    }

    public void SetHotbar(int hotbarIndex, string itemName, bool isWeapon) {
        if (itemName != null)
        {
            Transform hotbarCell = hotbarUI.transform.Find("Item" + hotbarIndex.ToString());
            hotbarCell.Find("itemName").GetComponent<TextMeshProUGUI>().text = itemName;
            if (hotbarCell.Find("preview").Find("previewItem") != null)
            {
                Destroy(hotbarCell.Find("preview").Find("previewItem").gameObject);
            }

            GameObject newPreviewItem;

            if (!isWeapon)
            {
                newPreviewItem = (GameObject)Instantiate(Resources.Load("Hotbar Previews/" + itemName));
            }
            else {

                newPreviewItem = (GameObject)Instantiate(InvObject.transform.Find(itemName).gameObject);
                newPreviewItem.SetActive(true);
                SetLayerRecursively(newPreviewItem, 5);

            }
            Destroy(newPreviewItem.GetComponent<ParticleSystem>());
            newPreviewItem.transform.parent = hotbarCell.Find("preview");
            newPreviewItem.transform.localPosition = new Vector3();
            newPreviewItem.transform.localRotation = Quaternion.identity;
            newPreviewItem.name = "previewItem";
        }
        else {
            Transform hotbarCell = hotbarUI.transform.Find("Item" + hotbarIndex.ToString());
            hotbarCell.Find("itemName").GetComponent<TextMeshProUGUI>().text = "";
            if (hotbarCell.Find("preview").Find("previewItem") != null)
            {
                Destroy(hotbarCell.Find("preview").Find("previewItem").gameObject);
            }
        }
    }

    //A method to switch between primary and secondary
    void SwitchWeapons(bool isPrimary){
        if (isPrimary){
            isOnPrimary = true;
            Transform hotbarCell = hotbarUI.transform.Find("Item2").Find("Image");
            hotbarCell.GetComponent<Image>().color = new Color(0.1694108f, 0.3140696f, 0.4433962f, 0.2392157f);
            hotbarCell = hotbarUI.transform.Find("Item1").Find("Image");
            hotbarCell.GetComponent<Image>().color = new Color(.4f, .6f, .8f, .5f);

            StartCoroutine(GetComponent<Character>().Equip(0));
        }else{
            isOnPrimary = false;
            Transform hotbarCell = hotbarUI.transform.Find("Item1").Find("Image");
            hotbarCell.GetComponent<Image>().color = new Color(0.1694108f, 0.3140696f, 0.4433962f, 0.2392157f);
            hotbarCell = hotbarUI.transform.Find("Item2").Find("Image");
            hotbarCell.GetComponent<Image>().color = new Color(.4f, .6f, .8f, .5f);

            StartCoroutine(GetComponent<Character>().Equip(1));
        }
    }

    void Equip(string type, GameObject equippingGun)
    {
        int gunId = 0;

        equippingGun.tag = "Untagged";
                    
        equippingGun.transform.parent = InvObject.transform;
        equippingGun.transform.localPosition = new Vector3();
        equippingGun.transform.localRotation = Quaternion.identity;
        SetLayerRecursively(equippingGun, 9);

        if (type == "Primary")
        {
            gunId = 0;
            primary = equippingGun;
        } 
        else if (type == "Secondary")
        {
            gunId = 1;
            secondary = equippingGun;
        }

        primary.transform.SetSiblingIndex(0);
        secondary.transform.SetSiblingIndex(1);
        
        if (gunId == 0)
            SetHotbar(1, primary.name, true);
        else
            SetHotbar(2, secondary.name, true);

        InvObject.GetComponent<Inventory>().Init(-1);

        StartCoroutine(GetComponent<Character>().Equip(gunId));

        if (gunId == 0)
            primary.SetActive(true);
        else
            secondary.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        primary = InvObject.transform.Find("Assault Rifle").gameObject;
        SetHotbar(1, primary.name, true);
        secondary = InvObject.transform.Find("Handgun").gameObject;
        SetHotbar(2, secondary.name, true);
        //switch to primary weapon by default
        SwitchWeapons(true);

        PopupText = hotbarUI.transform.parent.Find("EquipText").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Transform closestWeaponDrop = null;

        foreach (GameObject weaponDrop in GameObject.FindGameObjectsWithTag("WeaponDrops")) {
            if ((closestWeaponDrop == null) || (Vector3.Distance(closestWeaponDrop.position, transform.position) > Vector3.Distance(weaponDrop.transform.position, transform.position))) {
                closestWeaponDrop = weaponDrop.transform;
            }
        }

        if (interactTrigger == null)
        {
            if (closestWeaponDrop && (Vector3.Distance(closestWeaponDrop.position, transform.position) < 1f))
            {
                gunDrop = closestWeaponDrop.gameObject;
                SetPopupText("V - Equip " + closestWeaponDrop.name);
            }
            else {
                gunDrop = null;
                SetPopupText("");
            }
        }

        //Keybinds for switching weapons
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            SwitchWeapons(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)){
            SwitchWeapons(false);
        }

        //Equipping weapon drop
        if (Input.GetKeyDown(KeyCode.V)) {
            if (interactTrigger) {
                interactTrigger.GetComponent<TriggerEventHandler>().Activate();
                interactTrigger = null;
            } else if (gunDrop) {
                GameObject equippingGun = gunDrop;

                equippingGun.SetActive(false);

                Destroy(equippingGun.GetComponent<HoverEffect>());
                Destroy(equippingGun.GetComponent<ParticleSystem>());

                if (isOnPrimary)
                {
                    Equip("Primary", equippingGun);
                }
                else {
                    Equip("Secondary", equippingGun);
                }

                equippingGun.GetComponent<WeaponDestroy>().isPickedUp = true;
            }
        }
    }
}
