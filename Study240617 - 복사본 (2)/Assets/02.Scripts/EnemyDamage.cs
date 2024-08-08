using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    const string bulletTag = "BULLET";

    float iniHp = 100f; // �ʱ� ����
    public float hp = 100f;
    GameObject bloodEffect;

    public GameObject hpBarPrefab;
    public Vector3 hpBaroffset = new Vector3(0f, 2.2f, 0f);
    public GameObject lightEffcet;
    EnemyFire enemyFire;
    EnemyAI enemyAI;

    Canvas uiCanvas;
    Image hpBarImage;
    Rigidbody rb;
    MoveAgent moveAgent;
    NavMeshAgent navMeshAgent;
    MoveAgent agent;
    Animator animator;
    readonly int hashMove = Animator.StringToHash("IsMove");


    void Start()
    {
        //Resources.Load<GameObject>("���ϰ��")
        bloodEffect = Resources.Load<GameObject>("Blood");
        SetHpBar();
        rb = GetComponent<Rigidbody>();
        moveAgent = GetComponent<MoveAgent>();
        // enemy = GetComponent<EnemyDamage>();    
        navMeshAgent = GetComponent<NavMeshAgent>();
        agent = GetComponent<MoveAgent>();
        animator = GetComponent<Animator>();    
        enemyFire=GetComponent<EnemyFire>();
        enemyAI=GetComponent<EnemyAI>();


    }

    private void Update()
    {
        if (hp <= 0)
        {
            GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
        }
    }

    void SetHpBar()
    {
        uiCanvas = GameObject.Find("UI_Canvas").GetComponent<Canvas>();
        //hpBar�� ���� �����ϸ鼭 ĵ������ �ڽ����� �־���
        GameObject hpBar = Instantiate(hpBarPrefab, uiCanvas.transform);
        //hpBarImage=�������̹���
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        //���� �������� ���󰡾��� ���� ������ �� ����
        var enemyHpBar = hpBar.GetComponent<EnemyHpBar>();
        enemyHpBar.targetTr = this.gameObject.transform;
        enemyHpBar.offset = hpBaroffset;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(bulletTag))
        {
            //���� ȿ�� ���� �Լ� ȣ��
            ShowBloodEffect(collision);

            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
            //BulletCtrl�� �ۼ��� damage ������ ���� �����ͼ� ü�¿��� -����
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            //ü�¹��� ���� ���� ���̱�
            hpBarImage.fillAmount = hp / iniHp;
            if (hp <= 0f)
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;

                //���� �Ŵ����� ų ī��Ʈ ���� �Լ� ȣ��
                GameManager.instance.IncKillCount();
                //��� �� �ݶ��̴� ��Ȱ��ȭ
                GetComponent<CapsuleCollider>().enabled = false;
            }

        }
    }

    void ShowBloodEffect(Collision coll)
    {
        //�浹���� ��ġ �������� 
        Vector3 pos = coll.contacts[0].point;
        //�浹 ������ ���� ���� ���ϱ�
        Vector3 _normal = coll.contacts[0].normal;
        //�Ѿ��� ź�ο� ���ֺ��� ���� �� ���ϱ�
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, _normal);

        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1f);
    }
    public void HitGrenade()
    {
        {
            //rb.mass = 0.1f;
            //Debug.Log("rb");
            // rb.freezeRotation = false;
            // rb.AddForce(Vector3.up + explosionForceDirection, ForceMode.Impulse);
            // rb.AddTorque(Vector3.back * 45, ForceMode.Impulse);
        }
        hp -= 100;
        hpBarImage.fillAmount = hp / iniHp;
        //���� �׾����� Enemy ���¸� DIE�� ����
        //hpBar ����
        //ų ī��Ʈ �ø���
        //ĸ�� �ݶ��̴� false
        if (hp <= 0f)
        {
            GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
            hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;
            GameManager.instance.IncKillCount();
            GetComponent<CapsuleCollider>().enabled = false;

        }
    }
    public void Shock()
    {
        //�Ѹ°� ���ư����ʰ�
        rb.isKinematic = true;
        hp -= 70;
        hpBarImage.fillAmount = hp / iniHp;
        lightEffcet.SetActive(true);
        //�ǰ� 0�϶�
        if (hp <= 0f)
        {
            GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
            hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;
            GameManager.instance.IncKillCount();
            GetComponent<CapsuleCollider>().enabled = false;
            lightEffcet.SetActive(false);

        }
        else
        {

            StartCoroutine(StopSeconds(5f));

        }

    }
    //Enemy ������ų �ڷ�ƾ.
    IEnumerator StopSeconds(float duration)
    {
        //���� �����.
        //enemyAI.state = EnemyAI.State.PATROL;
        float time = 0.0f;
        //������ �������ڸ� Update()�Լ��� �������� ���� �Ǳ⶧����
        //1�ʰ� �ɶ� 60�� ȣ����� �� ���ִ�.�ڵ���� 60�� ȣ��ǰ�(1����)while���� ����������.

        while (time < 1.0f)
        {
            //Time.deltaTime�� 1/60�� �ǹ�
            //time>=1.0f�� �ɶ���  Time.deltaTime�� 60�� �̻� �����ϸ� ��.
            time += Time.deltaTime / duration;
            //Enemy�� moveAgent�� ���� ��Ŵ.
           
            moveAgent.Stopped();
          
           

            //yield return null�� ���� Update()�Լ��� ����� ���� ����ǵ���.
            yield return null;
        }

        rb.isKinematic = false;
          animator.enabled= true;
        lightEffcet.SetActive(false);

        yield return new WaitForSeconds(3f);

    }
}


