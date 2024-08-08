using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20f;//�Ѿ� ���ݷ�
    public float speed = 2000f;//�Ѿ��� �̵��ӵ�

    Transform tr;
    Rigidbody rb;
    TrailRenderer trail;
    
    void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();

       // GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        //������ �ٵ� ���� �ؽ�Ʈ�� UI���� ģ������ �̷��� �ٷ� ����� �ȵɼ��־
        // rb = GetComponent<Rigidbody>();ó�� �����ؾ���
    }
    private void OnEnable()
    {
        rb.AddForce(transform.forward * speed);
    }
    private void OnDisable()
    {
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
