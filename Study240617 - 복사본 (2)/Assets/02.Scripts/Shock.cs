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
        //Corutine 무한 동작을 막기위한 bool
        isCoroutineActive = true;
        _audio = GetComponent<AudioSource>();
        
        //Drop 스크립트에서 가져올건데 오브젝트가 달라 Find 사용.
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
        //만약 Slot에 자식의 수가 0이상일 때 =Slot에 자식 오브젝트가 있을때.
       if(drop.transform.childCount>0)
        {
            //그 자식의 오브젝트 Tag가 ItemShock이고 isCorutine이 동작할때.
            if (drop.childTag == "ItemShock" && isCoroutineActive)
            {
                Debug.Log($"태그 검사 bool: {isCoroutineActive}");
                //ShorkEft 코루틴을 실행.
                
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
        //코루틴 실행하고 있을 때 다시 실행하지않도록 비활성화.
        //moveAgent.Stop();
        //Enemy 정지
        //오디오 플레이 
        _audio.PlayOneShot(skSfx, 1f);
        //번개 효과 재생
        effectObj.SetActive(true);
        //3초후에 없어지도록
        Invoke("DestroyMesh", 3f);
        //레이캐스트 구형 모양의 레이캐스트(위치,반지름,방향,길이)
        RaycastHit[] rayhits = Physics.SphereCastAll(transform.position, 150, Vector3.up, 0f,
          LayerMask.GetMask("ENEMY")); //ENEMY 레이어가 걸릴떄 가져옴
        
        foreach (RaycastHit hitObj in rayhits)
        {
            //만약  닿은 레이어가 ENEMY일떄
            if (hitObj.transform.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                //EnemyDamage에서 레이어에 닿은 enemy의 정보를 가져옴.
                EnemyDamage enemy = hitObj.transform.GetComponent<EnemyDamage>();
                //닿은 enemy가 있을때.
                if (enemy != null)
                {
                    enemy.Shock(); // Enemy Damage의 Shock함수 실행
                }   

            }

        }
        //무한동작을 막기위해 끝나면 false로 비활성화.
        yield return new WaitForSeconds(1f);
        //아이템 없애기.
        
        StartCoroutine(DestoryChild());
    }
    void DestroyMesh()
    {
        //효과 끄기.
        effectObj.SetActive(false);
        //isCoroutineActive = true;
        
        //_audio.mute = true;
        //Debug.Log("Dsy");
    }
    IEnumerator DestoryChild()
    {
        yield return new WaitForSeconds(4.1f);
        // Destroy(drop.transform.GetChild(0).gameObject);
        //다시 슬롯에서 ItemList의 자식으로 변경 (원래자리로 복구)
        drag.transform.SetParent(itemListTr);
        isCoroutineActive = true;
        //다시 드래그가 되도록
        drag.enabled = true;
        //쇼크가 실행되었다는걸 알려주기 위해서.
        isFinished = true;
        coolDown.isCoolDown = false;
    }

    
}
