using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect;//폭발 효과 프리팹
        int hitCount = 0; //총알 맞은 횟수
    Rigidbody rb;
    AudioSource _audio;
    public AudioClip exp;

    //찌그러진 드럼통의 메쉬를 저정할 배열
    public Mesh[] meshes;
    MeshFilter meshFilter;
    public Texture[] textures;
    MeshRenderer _renderer;

    public float expRadius=10.0f;
    Shake shake;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.mainTexture = textures[Random.Range(0, textures.Length)];
        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    private void OnCollisionEnter(Collision collision)
    {    //만약 부딪힌 대상의 태그가 BULLET이라면
        if (collision.collider.CompareTag("BULLET"))
        {
            hitCount++; //맞은 횟수 증가
            if (hitCount == 3)
            {
                //폭발 메소드 호출
                ExpBarrel();
            }
        }
    }
    void ExpBarrel()
    {   //폭발 이펙트를 드럼통의 위치(transform.position)
        //자체 회전값을 가지고 동적 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        // Destroy를 그냥 사용하면 바로 없어지기에 2f후에 없어지도록. 
        Destroy(effect, 2f);
        //드럼통이 잘 날라가도록 질량값을 1로 변경
        //폭발하면 up방향으로 날아가게
        // rb.mass = 1f;
        rb.AddForce(Vector3.up * 1000f);
        IndirectDamage(transform.position);

        //난수(랜덤값 발생)
        int idx=Random.Range(0,meshes.Length);
        //위에서 추출한 랜덤 값을 통해서 메쉬 배열에 있는 메쉬 랜덤하게 골라오기
        meshFilter.sharedMesh = meshes[idx];
        _audio.PlayOneShot(exp, 1.0f);
        RaycastHit[] rayhits = Physics.SphereCastAll(transform.position, 7, Vector3.up, 0f,
           LayerMask.GetMask("ENEMY")); //ENEMY 레이어가 걸릴떄 가져옴
        foreach (RaycastHit hitObj in rayhits)
        {
            if (hitObj.transform.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                EnemyDamage enemy = hitObj.transform.GetComponent<EnemyDamage>();
                if (enemy != null)
                {
                    Vector3 explosionForceDirection = (hitObj.transform.position - transform.position).normalized;

                    enemy.HitGrenade(); // 적의 체력을 100 감소시킵니다.
                }

            }
        }

        //드럼통 폭발의 경우 흔들리는 경우가 더 커야하니
        // 매개변수를 큰 값으로 지정.
        StartCoroutine(shake.ShakeCamera(0.1f,0.2f,0.5f));
    }
    void IndirectDamage(Vector3 pos)
    {  //OverlapSphere(시작위치 ,반경, 검출 레이어)
        //위치로 부터 반경 사이의 검출 레이어에 해당디는
        //오브젝트의 충돌체 정보를 모두 가져옴
        // 1<<8의 의미는 8번 레이어를 켠다라는뜻
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8);

        foreach (var coll in colls)
        {
            var _rb= coll.GetComponent<Rigidbody>();
            _rb.mass = 1f;
            //AddExplosionForce(횡폭발력, 시작위치, 반경, 종 폭발력)
            //횡-가로, 종-세로
            _rb.AddExplosionForce(10000f, pos, expRadius, 600f);
        }
    }
}
    