using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//����ü�� �޸��� ���� ������ �Ҵ�
//������ Ŭ�������� ����
//���� ��� �� Ȱ���� �ʿ��� ��� ���� ���� 
[Serializable]
public struct PlayerSfx //����ü ,������ ���� ���ÿ� �����
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    public enum WeaponType //������ ����������
    {
        RIFLE = 0,
        STHOTGUN
    }
    //���� ��� ���� ���� Ȯ�ο� ����
    public WeaponType currWeapon = WeaponType.RIFLE;
    public GameObject bulletPrefabs;
    public Transform firePos;
    public ParticleSystem cartridge; //ź�� ��ƼŬ �ý��ۿ� ����
    private ParticleSystem muzzleFlash;
    //public GameObject[] weapons;
    //public bool[] hasweapons;
    // public GameObject[] grenades;
    [Header("����ź ����")]
    public int maxGrenades = 5;

    public int currentGrenades;
    public GameObject grenadeObj;
    public Transform throwPos;

    public PlayerSfx playerSfx;
    AudioSource _audio;
    //ī�޶� ���� ��ũ��Ʈ ��������
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



    [Header("�ڵ����ݰ���")]
    bool isFire = false;
    float nextFire;
    public float fireRate = 0.1f;


    int enemyLayer;
    int obstacleLayer;
    int layerMask;


    void Start()
    {   //firePos�� �ڽĿ�����Ʈ �߿��� ParticleSystem ������Ʈ ȹ��
        //����Ƽ�� ��� ������Ʈ�� ��뼺�� ����
        //���� ��ũ��Ʈ�� ��ġ�� ������Ʈ�� ��ġ�� �ſ� �߿���
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
        //grenadeObj=GetComponent<GameObject>();

        //���̾ �����ؼ� ����
        enemyLayer = LayerMask.NameToLayer("ENEMY");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        //�� ���̾� ����
        //���̾ 2�� �̻� ���� �Ҷ��� | (OR ��Ʈ ������ )�̿�
        layerMask = 1 << obstacleLayer | 1 << enemyLayer;
        //���� ����ź ���� maxGrenades���� ����.
        currentGrenades=maxGrenades;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.DrawRay(firePos.position, firePos.forward * 20f, Color.red);

        //UI ���� Ŭ�� �Ǵ� ��ġ�ϰ� �Ǹ� True. �ƴϸ� False 
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        //����ĳ��Ʈ ���(�ڵ�����)
        RaycastHit hit;
        //Raycast�� �浹 ������ �Ǵ�. ���� �浹 ��ü ������ RaycastHit�� ���޵�.
        //�̶� out���� ��µǴ� ���� ���޹ޱ� ���� ������ �̸� ������.
        //���� �߻���ġ , ���� �߻� ���� , �浹�� ��ü ���� ��ȯ ���� ����, ���� ��Ÿ� . ���� ���̾� 
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f, layerMask))
        {
            //enemyLayer�� ���ؼ� ������ �Ǹ� 

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

        //GetMouseButton�� ���콺 ������ �ִ� ���� ���� �߻� 
        //GetMouseButtonDown�� ������ ���� 1���� 
        //GetMouseButtonUp�� ���콺�� ���� ���� 1���� 
        //0�� ��Ŭ�� 1�� ��Ŭ�� 
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            reamainingBullet--;

            //���� �޼ҵ� ȣ��
            Fire();

            if (reamainingBullet == 0)
            {
                //������ �ڷ�ƾ �Լ� ȣ��
                StartCoroutine(Reloading());
            }
        }


    }
    void Fire()
    {
        StartCoroutine(shake.ShakeCamera());

        ////�Ѿ� �������� �ѱ��� ��ġ�� ȸ������ ������ ���� ����
        //Instantiate(bulletPrefabs, firePos.position, firePos.rotation);

        //���� ���� ������ ������� �ʰ� ������Ʈ Ǯ ���
        var _bullet = GameManager.instance.GetBullet();
        if (_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }
        //��ƼŬ ���
        cartridge.Play();

        muzzleFlash.Play();
        FireSfx();

        magazineImg.fillAmount = (float)reamainingBullet / (float)maxBullet;
        //���� �Ѿ� �� �ؽ�Ʈ ���ſ� �Լ� ȣ��
        UpdateBulletText();
    }
    void FireSfx()
    {
        //���� ��� �ִ� ������ enum���� int�� ��ȯ�ؼ�
        //����ϰ��� �ϴ� ������ ����� Ŭ���� ������.
        var _sfx = playerSfx.fire[(int)currWeapon];
        //������ ������ ,1(100%)�������� ���
        _audio.PlayOneShot(_sfx, 1f);
    }
    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1f);
        //������ ������ ���� +0.3�� ��ŭ ���
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        isReloading = false;
        magazineImg.fillAmount = 1f;
        reamainingBullet = maxBullet;
        // ���� �Ѿ� �� �ؽ�Ʈ ���ſ� �Լ� ȣ��
        UpdateBulletText();
    }
    void UpdateBulletText()
    {
        // string str = string.Format("<color=#ff0000{0}</color>/{1}", reamainingBullet, maxBullet);
        //�ٸ� ��� 
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
        //���� ���� ���� ����ź�� ���� 0�̸� ����.
        if (currentGrenades <= 0)
            return;
        //����ź ����.
        GameObject grenade = Instantiate(grenadeObj, throwPos.position, throwPos.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        //���������� currentGrenades���� ���̱�.
        currentGrenades--;
       

        // ����ź�� ���� ����� �� ����
        Vector3 throwDirection = throwPos.forward + throwPos.up;
        rb.AddForce(throwDirection * 9f, ForceMode.VelocityChange);
        rb.AddTorque(Vector3.back * 10f);



    }
}
