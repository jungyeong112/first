using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Granede : MonoBehaviour
{
    public GameObject meshObj;
    //public GameObject rb;
    public GameObject effectObj;
    AudioSource _audio;
    public AudioClip expGrd;
    Drag drag;
    Drop drop;
    FireCtrl fireCtrl;
    bool isActive = true;
    Transform itemListTr;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
        StartCoroutine(Explosion());
        drop = GameObject.Find("Slot").GetComponent<Drop>();
        drag = GameObject.Find("ImageGrenade").GetComponent<Drag>();
        fireCtrl = GameObject.Find("Player").GetComponent<FireCtrl>();
        itemListTr = GameObject.Find("ItemList").GetComponent<Transform>();

        // coolDown = GameObject.Find("Cooldown_Image").GetComponent<CoolDown>();
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(4.0f);
        //meshObj.SetActive(false);
        _audio.PlayOneShot(expGrd, 1f);
        effectObj.SetActive(true);

        Invoke("DestoryMesh", 0.3f);
        //위치, 반지름, 방향 , 레이를 쏘는 길이
        RaycastHit[] rayhits = Physics.SphereCastAll(transform.position, 10, Vector3.up, 0f,
            LayerMask.GetMask("ENEMY")); //ENEMY 레이어가 걸릴떄 가져옴

        //적들의 피격함수 호출
        foreach (RaycastHit hitObj in rayhits)
        {
            if (hitObj.transform.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                EnemyDamage enemy = hitObj.transform.GetComponent<EnemyDamage>();
                if (enemy != null)
                {
                    //폭발 방향설정.
                    // Vector3 explosionForceDirection= (hitObj.transform.position - transform.position).normalized;

                    enemy.HitGrenade(); //HitGrenade 함수 호출 
                }

            }
        }
    }







    void Update()
    {
        DragGrenade();
    }
    void DestoryMesh()
    {
        meshObj.SetActive(false);
        Destroy(GetComponent<TrailRenderer>());
    }
    public void DragGrenade()
    { //만약 Slot에 자식의 수가 0이상일 때 =Slot에 자식 오브젝트가 있을때.
        if (drop.transform.childCount > 0)
        {
            //그 자식의 오브젝트 Tag가 ItemGranade일때.
            if (drop.childTag == "ItemGrenade" && isActive)
            {
                // 무한 반복하지 않게 설정.
                isActive = false;
                Debug.Log(isActive);

                //실행중 드래그가 안되도록.
                drag.enabled = false;
                //수류탄 갯수 증가.
                fireCtrl.currentGrenades++;
                Debug.Log(fireCtrl.currentGrenades);
            }
            


        }
        StartCoroutine(DestoryChild());



    }
    IEnumerator DestoryChild()
    {
        yield return new WaitForSeconds(4.0f);
        // Destroy(drop.transform.GetChild(0).gameObject);
        //다시 슬롯에서 ItemList의 자식으로 변경 (원래자리로 복구)
        drag.transform.SetParent(itemListTr);
        //다시 드래그가 되도록
        drag.enabled = true;

        //isActive = true;
        // coolDown.isCoolDown = false;
    }
}
