using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : MonoBehaviour
{
    public GameObject effectObj;
    AudioSource _audio;
    public AudioClip skSfx;
    Rigidbody rb;
    Drop drop;

    Transform itemListTr;
    Drag drag;

    CoolDown coolDown;

    bool isCoroutineActive;
    public bool isFinished = false;
    
   // MoveAgent moveAgent;

    private void Awake()
    {
        //Corutine ���� ������ �������� bool
        isCoroutineActive = true;
        _audio = GetComponent<AudioSource>();
        
        //Drop ��ũ��Ʈ���� �����ðǵ� ������Ʈ�� �޶� Find ���.
        drop = GameObject.Find("Slot").GetComponent<Drop>();
        itemListTr = GameObject.Find("ItemList").GetComponent<Transform>();
        drag = GameObject.Find("ImageShock").GetComponent<Drag>();
        coolDown = GameObject.Find("Cooldown_Image").GetComponent<CoolDown>();


    }

    void Start()
    {
        
    }


    void Update()
    {
        //���� Slot�� �ڽ��� ���� 0�̻��� �� =Slot�� �ڽ� ������Ʈ�� ������.
       if(drop.transform.childCount>0)
        {
            //�� �ڽ��� ������Ʈ Tag�� ItemShock�̰� isCorutine�� �����Ҷ�.
            if (drop.childTag == "ItemShock" && isCoroutineActive)
            {
                Debug.Log($"�±� �˻� bool: {isCoroutineActive}");
                //ShorkEft �ڷ�ƾ�� ����.
                
                StartCoroutine(ShockEft(3f));
            }

        }
        
        //if (drop.childTag== ("ItemShock"))
        //{
        //    StartCoroutine(ShockEft(3f));
        //}
    }
    IEnumerator ShockEft(float i)
    {
        isCoroutineActive = false;
        drag.enabled = false;
        //�ڷ�ƾ �����ϰ� ���� �� �ٽ� ���������ʵ��� ��Ȱ��ȭ.
        //moveAgent.Stop();
        //Enemy ����
        //����� �÷��� 
        _audio.PlayOneShot(skSfx, 1f);
        //���� ȿ�� ���
        effectObj.SetActive(true);
        //3���Ŀ� ����������
        Invoke("DestroyMesh", 3f);
        //����ĳ��Ʈ ���� ����� ����ĳ��Ʈ(��ġ,������,����,����)
        RaycastHit[] rayhits = Physics.SphereCastAll(transform.position, 150, Vector3.up, 0f,
          LayerMask.GetMask("ENEMY")); //ENEMY ���̾ �ɸ��� ������
        
        foreach (RaycastHit hitObj in rayhits)
        {
            //����  ���� ���̾ ENEMY�ϋ�
            if (hitObj.transform.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                //EnemyDamage���� ���̾ ���� enemy�� ������ ������.
                EnemyDamage enemy = hitObj.transform.GetComponent<EnemyDamage>();
                //���� enemy�� ������.
                if (enemy != null)
                {
                    enemy.Shock(); // Enemy Damage�� Shock�Լ� ����
                }   

            }

        }
        //���ѵ����� �������� ������ false�� ��Ȱ��ȭ.
        yield return new WaitForSeconds(1f);
        //������ ���ֱ�.
        
        StartCoroutine(DestoryChild());
    }
    void DestroyMesh()
    {
        //ȿ�� ����.
        effectObj.SetActive(false);
        //isCoroutineActive = true;
        
        //_audio.mute = true;
        //Debug.Log("Dsy");
    }
    IEnumerator DestoryChild()
    {
        yield return new WaitForSeconds(4.1f);
        // Destroy(drop.transform.GetChild(0).gameObject);
        //�ٽ� ���Կ��� ItemList�� �ڽ����� ���� (�����ڸ��� ����)
        drag.transform.SetParent(itemListTr);
        isCoroutineActive = true;
        //�ٽ� �巡�װ� �ǵ���
        drag.enabled = true;
        //��ũ�� ����Ǿ��ٴ°� �˷��ֱ� ���ؼ�.
        isFinished = true;
        coolDown.isCoolDown = false;
    }

    
}
