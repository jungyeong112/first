using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
//클래스의 경우는 Serialized(직렬화) 라는 속성을 반드시 명시해줘야함
//그래야만 인스펙터에 노출이됨
[Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runB;
    public AnimationClip runF;
    public AnimationClip runL;
    public AnimationClip runR;
 
}

public class PlayerCtrl : MonoBehaviour
{
    float h = 0f;
    float v = 0f;
    float r = 0f;//회전값을 저장할 변수

    Transform tr;
    public float moveSpeed = 10f;//이동 속도 계수
    public float rotSpeed = 100f;//회전 속도 계수

    //애니메이션 관련 변수들
    public PlayerAnim playerAnim;
    Animation anim;
    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        //현재 재생해야될 애니메이션 클립을 idle로 설정
        anim.clip = playerAnim.idle;
        anim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        moveDir = moveDir.normalized; //벡터정규화
        //Translate(방향*속도,기준좌표)
        //기준좌표=Local(Self)/Global
        //플레이어 입장에서는 Local좌표계를 기준으로
        //앞뒤 좌우로 움직이도록
        tr.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);
        //Rotate(축 방향*속도)
        //ex) 양꼬치 or 회오리 감자
        tr.Rotate(Vector3.up * rotSpeed * r * Time.deltaTime);
        //애니메이션 전환
        if (v >= 0.1f) //v에 값이 들어왔다는건 앞으로 나아갔다라는뜻
        {
            anim.CrossFade(playerAnim.runF.name, 0.3f);
        }
        else if (v <= -0.1f)//뒤
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f);
        }
        else if (h >= 0.1f)//오
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f);
        }
        else if (h <= -0.1f)//왼 
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f);
        }
        
        else //움직임이 없을때
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f);
        }
    }
}
