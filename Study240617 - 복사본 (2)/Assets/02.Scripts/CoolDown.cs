using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoolDown : MonoBehaviour
{
    [Header("shock ����")]
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
        Debug.Log("shock ����");
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
        //coolDownTimer���� �ð����� �ʸ� ���ش�
        coolDownTimer -= Time.deltaTime;
        //���� ��ٿ� Ÿ�̸Ӱ� 0�ʰ� �Ǹ�.
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
        //��ũ�� ��������&&��ٿ� False�����϶�.
        if (shock.isFinished&&isCoolDown == false)
        {
            
            //��ٿ� ���� ����
            isCoolDown=true;
            //�� ���ư��� fillAmount�� ä����(����������)
            coolDownImage1.fillAmount = 1;
            //text�� ���ش�.
            textCoolDown.enabled=true;
            //timer�� cooldowntime�� ���� ����
            coolDownTimer = cooldownTime;
        }
        //���� ��ٿ� ���¶��
        if (isCoolDown)
        {
            CoolDownTimer();
            //fillAmount= fillAmount���� �ð� �ʿ� ���� ����.
            coolDownImage1.fillAmount -=  1/cooldownTime * Time.deltaTime;
            //��Ÿ�� ���� �巡�׸� ���ϰ�.
            drag.enabled = false;


            Debug.Log(cooldownTime);
            
            //���� fillAmount�� 0�����̸�
            if (coolDownImage1.fillAmount <= 0)
            {
                //fillAmount�� 0���� ����� ��Ÿ���� ������ó�� ���̰� �����
                coolDownImage1.fillAmount = 0;
                //��Ÿ�������� �巡�� ����.
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
