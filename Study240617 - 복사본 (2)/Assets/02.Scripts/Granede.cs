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
        //��ġ, ������, ���� , ���̸� ��� ����
        RaycastHit[] rayhits = Physics.SphereCastAll(transform.position, 10, Vector3.up, 0f,
            LayerMask.GetMask("ENEMY")); //ENEMY ���̾ �ɸ��� ������

        //������ �ǰ��Լ� ȣ��
        foreach (RaycastHit hitObj in rayhits)
        {
            if (hitObj.transform.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                EnemyDamage enemy = hitObj.transform.GetComponent<EnemyDamage>();
                if (enemy != null)
                {
                    //���� ���⼳��.
                    // Vector3 explosionForceDirection= (hitObj.transform.position - transform.position).normalized;

                    enemy.HitGrenade(); //HitGrenade �Լ� ȣ�� 
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
    { //���� Slot�� �ڽ��� ���� 0�̻��� �� =Slot�� �ڽ� ������Ʈ�� ������.
        if (drop.transform.childCount > 0)
        {
            //�� �ڽ��� ������Ʈ Tag�� ItemGranade�϶�.
            if (drop.childTag == "ItemGrenade" && isActive)
            {
                // ���� �ݺ����� �ʰ� ����.
                isActive = false;
                Debug.Log(isActive);

                //������ �巡�װ� �ȵǵ���.
                drag.enabled = false;
                //����ź ���� ����.
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
        //�ٽ� ���Կ��� ItemList�� �ڽ����� ���� (�����ڸ��� ����)
        drag.transform.SetParent(itemListTr);
        //�ٽ� �巡�װ� �ǵ���
        drag.enabled = true;

        //isActive = true;
        // coolDown.isCoolDown = false;
    }
}
