using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect;//���� ȿ�� ������
        int hitCount = 0; //�Ѿ� ���� Ƚ��
    Rigidbody rb;
    AudioSource _audio;
    public AudioClip exp;

    //��׷��� �巳���� �޽��� ������ �迭
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
    {    //���� �ε��� ����� �±װ� BULLET�̶��
        if (collision.collider.CompareTag("BULLET"))
        {
            hitCount++; //���� Ƚ�� ����
            if (hitCount == 3)
            {
                //���� �޼ҵ� ȣ��
                ExpBarrel();
            }
        }
    }
    void ExpBarrel()
    {   //���� ����Ʈ�� �巳���� ��ġ(transform.position)
        //��ü ȸ������ ������ ���� ����
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        // Destroy�� �׳� ����ϸ� �ٷ� �������⿡ 2f�Ŀ� ����������. 
        Destroy(effect, 2f);
        //�巳���� �� ���󰡵��� �������� 1�� ����
        //�����ϸ� up�������� ���ư���
        // rb.mass = 1f;
        rb.AddForce(Vector3.up * 1000f);
        IndirectDamage(transform.position);

        //����(������ �߻�)
        int idx=Random.Range(0,meshes.Length);
        //������ ������ ���� ���� ���ؼ� �޽� �迭�� �ִ� �޽� �����ϰ� ������
        meshFilter.sharedMesh = meshes[idx];
        _audio.PlayOneShot(exp, 1.0f);
        RaycastHit[] rayhits = Physics.SphereCastAll(transform.position, 7, Vector3.up, 0f,
           LayerMask.GetMask("ENEMY")); //ENEMY ���̾ �ɸ��� ������
        foreach (RaycastHit hitObj in rayhits)
        {
            if (hitObj.transform.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                EnemyDamage enemy = hitObj.transform.GetComponent<EnemyDamage>();
                if (enemy != null)
                {
                    Vector3 explosionForceDirection = (hitObj.transform.position - transform.position).normalized;

                    enemy.HitGrenade(); // ���� ü���� 100 ���ҽ�ŵ�ϴ�.
                }

            }
        }

        //�巳�� ������ ��� ��鸮�� ��찡 �� Ŀ���ϴ�
        // �Ű������� ū ������ ����.
        StartCoroutine(shake.ShakeCamera(0.1f,0.2f,0.5f));
    }
    void IndirectDamage(Vector3 pos)
    {  //OverlapSphere(������ġ ,�ݰ�, ���� ���̾�)
        //��ġ�� ���� �ݰ� ������ ���� ���̾ �ش���
        //������Ʈ�� �浹ü ������ ��� ������
        // 1<<8�� �ǹ̴� 8�� ���̾ �Ҵٶ�¶�
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8);

        foreach (var coll in colls)
        {
            var _rb= coll.GetComponent<Rigidbody>();
            _rb.mass = 1f;
            //AddExplosionForce(Ⱦ���߷�, ������ġ, �ݰ�, �� ���߷�)
            //Ⱦ-����, ��-����
            _rb.AddExplosionForce(10000f, pos, expRadius, 600f);
        }
    }
}
    