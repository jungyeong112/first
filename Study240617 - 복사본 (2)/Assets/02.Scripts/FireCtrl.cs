using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//구조체는 메모리의 스택 영역에 할당
//성능이 클래스보다 좋음
//빠른 계산 및 활용이 필요한 경우 쓰면 좋음 
[Serializable]
public struct PlayerSfx //구조체 ,성능이 좋은 스택에 저장됨
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    public enum WeaponType //열거형 직관적으로
    {
        RIFLE = 0,
        STHOTGUN
    }
    //현재 사용 중인 무기 확인용 변수
    public WeaponType currWeapon = WeaponType.RIFLE;
    public GameObject bulletPrefabs;
    public Transform firePos;
    public ParticleSystem cartridge; //탄피 파티클 시스템용 변수
    private ParticleSystem muzzleFlash;
    //public GameObject[] weapons;
    //public bool[] hasweapons;
    // public GameObject[] grenades;
    [Header("수류탄 관련")]
    public int maxGrenades = 5;

    public int currentGrenades;
    public GameObject grenadeObj;
    public Transform throwPos;

    public PlayerSfx playerSfx;
    AudioSource _audio;
    //카메라 흔드는 스크립트 가져오기
    Shake shake;

    public Image magazineImg;
    public Text magazineText;

    public int maxBullet = 10;
    public int reamainingBullet = 10;
    public float reloadTime = 2f;
    bool isReloading = false;

    public Sprite[] weaponIcons;
    public Image weaponImage;

    bool gDown;



    [Header("자동공격관련")]
    bool isFire = false;
    float nextFire;
    public float fireRate = 0.1f;


    int enemyLayer;
    int obstacleLayer;
    int layerMask;


    void Start()
    {   //firePos의 자식오브젝트 중에서 ParticleSystem 컴포넌트 획득
        //유니티의 모든 오브젝트는 상대성을 지님
        //따라서 스크립트의 위치나 오브젝트의 위치가 매우 중요함
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
        //grenadeObj=GetComponent<GameObject>();

        //레이어값 추출해서 저장
        enemyLayer = LayerMask.NameToLayer("ENEMY");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        //두 레이어 결합
        //레이어를 2개 이상 병합 할때는 | (OR 비트 연산자 )이용
        layerMask = 1 << obstacleLayer | 1 << enemyLayer;
        //현재 수류탄 수는 maxGrenades수와 같다.
        currentGrenades=maxGrenades;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.DrawRay(firePos.position, firePos.forward * 20f, Color.red);

        //UI 등을 클릭 또는 터치하게 되면 True. 아니면 False 
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        //레이캐스트 방식(자동공격)
        RaycastHit hit;
        //Raycast는 충돌 유무만 판단. 실제 충돌 객체 정보는 RaycastHit에 전달됨.
        //이때 out으로 출력되는 값을 전달받기 위한 변수를 미리 선언함.
        //레이 발사위치 , 레이 발사 방향 , 충돌한 객체 정보 반환 받을 변수, 레이 사거리 . 검출 레이어 
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f, layerMask))
        {
            //enemyLayer에 의해서 검출이 되면 

            isFire = hit.collider.CompareTag("ENEMY");

        }
        else
        {
            isFire = false;
        }
        if (!isReloading && isFire)
        {
            if (Time.time > nextFire)
            {
                reamainingBullet--;
                Fire();
                // FireInTheHole();
                if (reamainingBullet == 0)
                {
                    StartCoroutine(Reloading());
                }
                nextFire = Time.time + fireRate;
            }


        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            ThrowGrenade();
        }

        //GetMouseButton은 마우스 누르고 있는 동안 지속 발생 
        //GetMouseButtonDown은 누르는 순간 1번만 
        //GetMouseButtonUp은 마우스를 때는 순간 1번만 
        //0은 좌클릭 1은 우클릭 
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            reamainingBullet--;

            //공격 메소드 호출
            Fire();

            if (reamainingBullet == 0)
            {
                //재장전 코루틴 함수 호출
                StartCoroutine(Reloading());
            }
        }


    }
    void Fire()
    {
        StartCoroutine(shake.ShakeCamera());

        ////총알 프리팹을 총구의 위치와 회전값을 가지고 동적 생성
        //Instantiate(bulletPrefabs, firePos.position, firePos.rotation);

        //위의 동적 생성을 사용하지 않고 오브젝트 풀 사용
        var _bullet = GameManager.instance.GetBullet();
        if (_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }
        //파티클 재생
        cartridge.Play();

        muzzleFlash.Play();
        FireSfx();

        magazineImg.fillAmount = (float)reamainingBullet / (float)maxBullet;
        //남은 총알 수 텍스트 갱신용 함수 호출
        UpdateBulletText();
    }
    void FireSfx()
    {
        //현재 들고 있는 무기의 enum값을 int로 변환해서
        //재생하고자 하는 무기의 오디오 클립을 가져옴.
        var _sfx = playerSfx.fire[(int)currWeapon];
        //지정된 음원을 ,1(100%)볼륨으로 재생
        _audio.PlayOneShot(_sfx, 1f);
    }
    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1f);
        //재장전 음원의 길이 +0.3초 만큼 대기
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        isReloading = false;
        magazineImg.fillAmount = 1f;
        reamainingBullet = maxBullet;
        // 남은 총알 수 텍스트 갱신용 함수 호출
        UpdateBulletText();
    }
    void UpdateBulletText()
    {
        // string str = string.Format("<color=#ff0000{0}</color>/{1}", reamainingBullet, maxBullet);
        //다른 방법 
        string str2 = $"<color=#ff0000>{reamainingBullet}</color>/{maxBullet}";
        // string str3 = "<color=#ff0000>" + reamainingBullet + "</color>/" + maxBullet;


        magazineText.text = str2;
    }
    public void OnChangeWeapon()
    {
        currWeapon++;
        currWeapon = (WeaponType)((int)currWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currWeapon];
    }
    void ThrowGrenade()
    {
        //만약 현재 가진 수류탄의 수가 0이면 리턴.
        if (currentGrenades <= 0)
            return;
        //수류탄 생성.
        GameObject grenade = Instantiate(grenadeObj, throwPos.position, throwPos.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        //던질때마다 currentGrenades개수 줄이기.
        currentGrenades--;
       

        // 수류탄을 던질 방향과 힘 설정
        Vector3 throwDirection = throwPos.forward + throwPos.up;
        rb.AddForce(throwDirection * 9f, ForceMode.VelocityChange);
        rb.AddTorque(Vector3.back * 10f);



    }
}
