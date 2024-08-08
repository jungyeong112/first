using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    float RotationSpeed = 30f;
    Transform EnemyAI;
    private void Awake()
    {
        
        EnemyAI = GetComponent<Transform>();  
    }
    void Start()
    {  
       transform.position = EnemyAI.transform.position + new Vector3(0,1,0);
        transform.Rotate(new Vector3(90,0,0));
    }

  
    void Update()
    {
        transform.Rotate(new Vector3(0,0, Time.deltaTime * RotationSpeed));
    }
}
