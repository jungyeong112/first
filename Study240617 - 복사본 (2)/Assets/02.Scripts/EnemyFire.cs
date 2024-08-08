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
    //�ڵ� ���� ���� ����
    [Header("�ڵ�����")]
    float nextFire = 0f;
    readonly float fireRate = 0.1f; //�߻� ����
    readonly float damping = 10f;

    [Header("������")]
    readonly float reloadTime = 2f;
    readonly int maxBullet = 10;
    int currBullet = 10;
    bool isReload = false;
    WaitForSeconds wsReload;

    public  bool isFire = false;
    public AudioClip fireSfx;
    public AudioClip reloadSfx;
    //�Ѿ� �߻� ����
    public GameObject bullet;
    public Transform firePos;
    //�ѱ�ȭ�� ����
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
                //�Ѿ� �߻� �Լ� ȣ��
                nextFire = Time.time + Random.Range(0f, 0.3f);
            }
            //A-B= B�� A�� �ٶ󺸴� ����
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation=Quaternion.Slerp(enemyTr.rotation, rot,Time.deltaTime*damping);
        }
    }
    void Fire()
    {
        //�ѱ� ȭ�� �ڷ�ƾ �Լ� ȣ��
        StartCoroutine(ShowMuzzleFlash());


        animator.SetTrigger(hashFire);
        _audio.PlayOneShot(fireSfx, 1f);

        GameObject _bullet = Instantiate(bullet, firePos.position, firePos.rotation);
        Destroy(_bullet, 3f);
        currBullet--;
        //���� �Ѿ� ������ ���ؼ� ������ ��������
        //=������ ������ ���Ͽ� ���� ��� true �ƴϸ� false
        isReload = (currBullet % maxBullet == 0);
        if (isReload)
        {
            //������ �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(Reload());
        }
    }
    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.enabled = true;
        //���� �÷��� ���带 0~360�� ȸ�� ��Ű�� ����
        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;
        //�����÷����� Scale ���� xyz ��� 1~2�� �ø���
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1f, 2f);
        //�ؽ��� ������ �����ϱ�
        //Random.Range�� ���ؼ� 0�Ǵ� 1���� �����µ� 
        //0.5�� ���ؼ� 0�Ǵ� 0.5���� �������� ���
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        //�����÷����� ��Ƽ������ offset ���� ����
        //��Ȯ���� ��Ƽ������ �����ϴ� Shader�� ������Ƽ ���� ����
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);
        //���ð��� 0.05 ~ 0.2�� ���� �����ϰ� ����
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));


        muzzleFlash.enabled = false;
    }
    IEnumerator Reload()
    {
        animator.SetTrigger(hashReload);
        _audio.PlayOneShot(reloadSfx, 1f);
        //������ �ϴ� ���� ����� �纸
        //�ִϸ��̼� ���� �ð����� ��ٸ��°�
        yield return wsReload;

        currBullet = maxBullet;
        isReload = false;
    }
}
