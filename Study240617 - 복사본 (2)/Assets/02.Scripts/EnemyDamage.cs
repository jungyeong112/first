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

    float iniHp = 100f; // 초기 피통
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
        //Resources.Load<GameObject>("파일경로")
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
        //hpBar를 동적 생성하면서 캔버스의 자식으로 넣어줌
        GameObject hpBar = Instantiate(hpBarPrefab, uiCanvas.transform);
        //hpBarImage=빨간색이미지
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        //생명 게이지가 따라가야할 대상과 오프셋 값 설정
        var enemyHpBar = hpBar.GetComponent<EnemyHpBar>();
        enemyHpBar.targetTr = this.gameObject.transform;
        enemyHpBar.offset = hpBaroffset;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(bulletTag))
        {
            //혈흔 효과 생성 함수 호출
            ShowBloodEffect(collision);

            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
            //BulletCtrl에 작성한 damage 변수의 값을 가져와서 체력에서 -해줌
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            //체력바의 빨간 색을 줄이기
            hpBarImage.fillAmount = hp / iniHp;
            if (hp <= 0f)
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;

                //게임 매니저의 킬 카운트 증가 함수 호출
                GameManager.instance.IncKillCount();
                //사망 후 콜라이더 비활성화
                GetComponent<CapsuleCollider>().enabled = false;
            }

        }
    }

    void ShowBloodEffect(Collision coll)
    {
        //충돌지점 위치 가져오기 
        Vector3 pos = coll.contacts[0].point;
        //충돌 지점의 법선 벡터 구하기
        Vector3 _normal = coll.contacts[0].normal;
        //총알의 탄두와 마주보는 방향 값 구하기
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
        //만약 죽었을때 Enemy 상태를 DIE로 변경
        //hpBar 변경
        //킬 카운트 늘리기
        //캡슐 콜라이더 false
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
        //총맞고 날아가지않게
        rb.isKinematic = true;
        hp -= 70;
        hpBarImage.fillAmount = hp / iniHp;
        lightEffcet.SetActive(true);
        //피가 0일때
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
    //Enemy 정지시킬 코루틴.
    IEnumerator StopSeconds(float duration)
    {
        //총을 못쏘게.
        //enemyAI.state = EnemyAI.State.PATROL;
        float time = 0.0f;
        //로직을 정의하자면 Update()함수는 매프레임 정의 되기때문에
        //1초가 될때 60번 호출됨을 알 수있다.코드또한 60번 호출되고(1초후)while문을 빠져나간다.

        while (time < 1.0f)
        {
            //Time.deltaTime은 1/60을 의미
            //time>=1.0f가 될때는  Time.deltaTime을 60번 이상 실행하면 됨.
            time += Time.deltaTime / duration;
            //Enemy의 moveAgent를 정지 시킴.
           
            moveAgent.Stopped();
          
           

            //yield return null을 통해 Update()함수가 진행된 다음 실행되도록.
            yield return null;
        }

        rb.isKinematic = false;
          animator.enabled= true;
        lightEffcet.SetActive(false);

        yield return new WaitForSeconds(3f);

    }
}


