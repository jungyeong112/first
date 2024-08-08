using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    AudioSource _audio;
    Animator animator;
    Transform playerTr;
    Transform enemyTr;

    readonly int hashFire = Animator.StringToHash("Fire");
    readonly int hashReload = Animator.StringToHash("Reload");
    //자동 공격 관련 변수
    [Header("자동공격")]
    float nextFire = 0f;
    readonly float fireRate = 0.1f; //발사 간격
    readonly float damping = 10f;

    [Header("재장전")]
    readonly float reloadTime = 2f;
    readonly int maxBullet = 10;
    int currBullet = 10;
    bool isReload = false;
    WaitForSeconds wsReload;

    public  bool isFire = false;
    public AudioClip fireSfx;
    public AudioClip reloadSfx;
    //총알 발사 관련
    public GameObject bullet;
    public Transform firePos;
    //총구화염 관련
    public MeshRenderer muzzleFlash;

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        _audio=GetComponent<AudioSource>();

        wsReload = new WaitForSeconds(reloadTime);

        muzzleFlash.enabled = false;
    }

    
    void Update()
    {
        if (!isReload && isFire)
        {
            if (Time.time > nextFire)
            {
                Fire();
                //총알 발사 함수 호출
                nextFire = Time.time + Random.Range(0f, 0.3f);
            }
            //A-B= B가 A를 바라보는 방향
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation=Quaternion.Slerp(enemyTr.rotation, rot,Time.deltaTime*damping);
        }
    }
    void Fire()
    {
        //총구 화염 코루틴 함수 호출
        StartCoroutine(ShowMuzzleFlash());


        animator.SetTrigger(hashFire);
        _audio.PlayOneShot(fireSfx, 1f);

        GameObject _bullet = Instantiate(bullet, firePos.position, firePos.rotation);
        Destroy(_bullet, 3f);
        currBullet--;
        //현재 총알 갯수를 비교해서 재장전 유무설정
        //=우측의 조건을 비교하여 참일 경우 true 아니면 false
        isReload = (currBullet % maxBullet == 0);
        if (isReload)
        {
            //재장전 코루틴 함수 호출
            StartCoroutine(Reload());
        }
    }
    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.enabled = true;
        //머즐 플래시 쿼드를 0~360도 회전 시키기 위함
        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;
        //머즈플래시의 Scale 값을 xyz 모두 1~2배 늘리기
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1f, 2f);
        //텍스쳐 오프셋 조정하기
        //Random.Range에 의해서 0또는 1값이 나오는데 
        //0.5를 곱해서 0또는 0.5값이 나오도록 계산
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        //머즐플래시의 머티리얼의 offset 값을 전달
        //정확히는 머티리얼이 제어하는 Shader의 프로퍼티 값을 변경
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);
        //대기시간을 0.05 ~ 0.2초 사이 랜덤하게 설정
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));


        muzzleFlash.enabled = false;
    }
    IEnumerator Reload()
    {
        animator.SetTrigger(hashReload);
        _audio.PlayOneShot(reloadSfx, 1f);
        //재장전 하는 동안 제어권 양보
        //애니메이션 동작 시간동안 기다리는것
        yield return wsReload;

        currBullet = maxBullet;
        isReload = false;
    }
}
