using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }
    void Update()
    {
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

            default:
                timer += Time.deltaTime;
                if(timer > 1/speed)
                {
                    Fire();
                    timer = 0;
                }
                break;
        }

    }


    public void Init(ItemData data)
    {
        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = data.baseDamage;
        speed = data.baseSpeed;
        count = data.baseCount;

        for(int i = 0; i < GameManager.instance.poolManager.prefabs.Length; i++)
        {
            if(data.projectile == GameManager.instance.poolManager.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        switch(id)
        {
            case 0:
                Arrange();
                break;

            default:
                break;
        }

        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void LevelUp(float _damage, int _count)
    {
        damage = _damage;
        count = _count;

        if (id == 0) Arrange();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Arrange()
    {
        for (int i=0; i< count; i++)
        {
            Transform bulletTrans;
            if (i < transform.childCount)
            {
                bulletTrans = transform.GetChild(i);
            }
            else
            {
                bulletTrans = GameManager.instance.poolManager.Get(prefabId).transform;
            }
            bulletTrans.parent = transform;
            bulletTrans.localPosition = Vector3.zero;
            bulletTrans.localRotation = Quaternion.identity;
            Vector3 rotVec = Vector3.forward * 360 / count * i;
            bulletTrans.Rotate(rotVec);
            bulletTrans.Translate(bulletTrans.up * 1f, Space.World);
            bulletTrans.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -1 is Infinity Per.
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget) return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dirVec = (targetPos - transform.position).normalized;

        Transform bulletTrans = GameManager.instance.poolManager.Get(prefabId).transform;
        bulletTrans.GetComponent<Bullet>().per = count;
        bulletTrans.position = transform.position;
        bulletTrans.rotation = Quaternion.FromToRotation(Vector3.up, dirVec);
        bulletTrans.GetComponent<Bullet>().Init(damage, count, dirVec);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
