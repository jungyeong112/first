using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    //Enemy is Die를 가져오기 위해서
    EnemyAI enemyAI;
    public GameObject gold;
    void Start()
    {
        //EnemyAI를 가져옴.
       enemyAI = GetComponent<EnemyAI>();
    }

   
    void Update()
    {
        if (enemyAI.isDie == true)
        {
            StartCoroutine(DropCoin());
        }
    }
    IEnumerator DropCoin()
    {
        
        enemyAI.isDie = false;
       
        Instantiate(gold, transform.position, transform.rotation);
       
        yield return new WaitForSeconds(5f);
    }
}
