using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20f;//총알 공격력
    public float speed = 2000f;//총알의 이동속도

    Transform tr;
    Rigidbody rb;
    TrailRenderer trail;
    
    void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();

       // GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        //리지드 바디 말고 텍스트나 UI같은 친구들은 이렇게 바로 사용이 안될수있어서
        // rb = GetComponent<Rigidbody>();처럼 선언해야함
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
