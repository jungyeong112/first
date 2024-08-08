using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoolDown : MonoBehaviour
{
    [Header("shock 관련")]
    public Image coolDownImage1;
    public float cooldownTime = 10f;
    Shock shock;
    public bool isCoolDown = false;
    Drag drag;
    [SerializeField]
    private TMP_Text textCoolDown;
    private float coolDownTimer = 0.0f;



    void Start()
    {
        shock = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Shock>();
        Debug.Log("shock 끝남");
        coolDownImage1.fillAmount = 0;
        drag = GameObject.Find("ImageShock").GetComponent<Drag>();
        textCoolDown.enabled = false;
    }


    void Update()
    {
       StartCoolDown();    
    }
    void CoolDownTimer()
    {
        //coolDownTimer에서 시간마다 초를 뺴준다
        coolDownTimer -= Time.deltaTime;
        //만약 쿨다운 타이머가 0초가 되면.
        if (coolDownTimer < 0.0f)
        {
            //isCoolDown = false;
            textCoolDown.enabled = false;
          //  coolDownImage1.fillAmount = 0.0f;
        }
        else 
        {
            textCoolDown.text=Mathf.RoundToInt(coolDownTimer).ToString(); 
            
        }
    }
   public void StartCoolDown()
    {
        //쇼크가 끝낫을때&&쿨다운 False상태일때.
        if (shock.isFinished&&isCoolDown == false)
        {
            
            //쿨다운 상태 변경
            isCoolDown=true;
            //쿨 돌아가게 fillAmount를 채워줌(검은색으로)
            coolDownImage1.fillAmount = 1;
            //text를 켜준다.
            textCoolDown.enabled=true;
            //timer와 cooldowntime를 같게 해줌
            coolDownTimer = cooldownTime;
        }
        //만약 쿨다운 상태라면
        if (isCoolDown)
        {
            CoolDownTimer();
            //fillAmount= fillAmount에서 시간 초에 따라 빼줌.
            coolDownImage1.fillAmount -=  1/cooldownTime * Time.deltaTime;
            //쿨타임 동안 드래그를 못하게.
            drag.enabled = false;


            Debug.Log(cooldownTime);
            
            //만약 fillAmount가 0이하이면
            if (coolDownImage1.fillAmount <= 0)
            {
                //fillAmount를 0으로 만들어 쿨타임이 지난거처럼 보이게 만들기
                coolDownImage1.fillAmount = 0;
                //쿨타임지나면 드래그 가능.
                drag.enabled = true;

            }
        }    
    }
    //IEnumerator StartCD(float cool)
    //{
       
    //    {
    //        isCoolDown = true;
    //        coolDownImage1.fillAmount = 1;
    //    }
    //    if (isCoolDown)
    //    {
    //        Debug.Log(coolDownImage1.fillAmount);
    //        coolDownImage1.fillAmount -= 1 / cooldownTime * Time.deltaTime;
    //        if (coolDownImage1.fillAmount <= 0)
    //        {
    //            coolDownImage1.fillAmount = 0;

    //        }
    //    }
    //    yield return null;
    //}
}
