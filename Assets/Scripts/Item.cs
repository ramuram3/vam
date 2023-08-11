using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text levelText;
    Text nameText;
    Text descText;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        //텍스트 순서 바꾸면 안됨!
        Text[] texts = GetComponentsInChildren<Text>();
        levelText = texts[0];
        nameText = texts[1];
        descText = texts[2];

        nameText.text = data.itemName;
    }

    
    void OnEnable()
    {
        switch(data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0)
                    descText.text = data.initItemDesc;
                else
                {
                    descText.text = string.Format(data.itemDesc, (data.damages[level] - 1) * 100);
                }
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                descText.text = string.Format(data.itemDesc, (data.damages[level] - 1) * 100);
                break;

            case ItemData.ItemType.Heal:
                descText.text = data.itemDesc;
                break;


        }
        levelText.text = string.Format("Lv.{0:D2}", level);

    }

    public void ResetLevel()
    {
        level = 0;

        switch(data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (weapon != null)
                    Destroy(weapon.gameObject);
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if(gear!=null)
                Destroy(gear.gameObject);
                break;
        }

    }
    public void OnClick()
    {
        switch(data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if(level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else if(level > 0)
                {
                    float nextDamage = data.baseDamage * data.damages[level];
                    int nextCount = data.counts[level];

                    weapon.LevelUp(nextDamage,nextCount);
                }
                level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if(level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[0] * data.damages[level];
                    gear.LevelUp(nextRate);
                }
                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }

            
        if(level == data.damages.Length)
        {
            GetComponentInParent<LevelUp>().maxLevelNum++;
            GetComponent<Button>().interactable = false;
        }
    }

    public void SettingCharacter()
    {
        switch(GameManager.instance.playerId)
        {
            case 0:
                if(data.itemType == ItemData.ItemType.Shoe)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                    level++;
                }
                break;
            case 1:
                if (data.itemType == ItemData.ItemType.Glove)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                    level++;
                }
                break;
            case 2:
                if(data.itemType == ItemData.ItemType.Range)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                    level++;
                }
                break;
            case 3:
                if (data.itemType == ItemData.ItemType.Melee)
                {
                    float nextDamage = data.baseDamage * data.damages[level];
                    int nextCount = data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }
                break;
        }
    }
}
