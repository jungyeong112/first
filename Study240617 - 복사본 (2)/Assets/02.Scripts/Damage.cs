using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    const string bulletTag = "BULLET";
    const string enemyTag = "ENEMY";
    float initHp = 100f;
    public float currHp;

    //델리게이트 및 이벤트 생성
    public delegate void PlayerDieHandler();
    //델리게이트에서 파생된 이벤트
    public static event PlayerDieHandler PlayerDieEvent;

    public Image bloodScreen;

    public Image hpBar;
    readonly Color initColor = new Color(0, 1f, 0f, 1f); //new Color 대신 new Vector4도 사용가능
    Color currColor;

    void Start()
    {
        currHp = initHp;

        //체력바 초기 색상 설정
        hpBar.color=initColor;
        currColor = initColor;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(bulletTag))
        {
            Destroy(other.gameObject);

            //출혈 화면 효과 코루틴 함수 호출 
            StartCoroutine(ShowBloodScreen());

            currHp -= 5f;
            DisplayHpBar();

           // print(currHp);

            if (currHp <= 0f)
            {
                // 플레이어 죽음 메서드 호출
                PlayerDie();
            }
        }
    }
    IEnumerator ShowBloodScreen()
    {
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //다시 색상도 0 투명도 0 clear는 0000
        bloodScreen.color = Color.clear;
    }
    void PlayerDie()
    {
    //    print("플레이어 사망");
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

    //    foreach(GameObject enemy in enemies)
    //    {
    //        //SendMessage("호출할 메소드 이름", 응답모드)
    //        //SendMessage 메소드는 특정 스크립트를 지정하는 것이 아니라
    //        //오브젝트에 포함된 모든 스크립트를 하나씩 검색해서 
    //        //해당하는 이름이 존재하면 해당 메소드를 호출하는 방식.
    //        enemy.SendMessage("E_PlayerDie", SendMessageOptions.DontRequireReceiver);
    //이벤트 호출
        PlayerDieEvent();
        //싱글턴 패턴을 이용하여 손 쉽게 접근
        GameManager.instance.isGameOver = false;
    }

    void DisplayHpBar()
    {   //체력비율
        float currHpRate = currHp / initHp;

        if (currHpRate > 0.5f)  //체력이 50%보다 많을때
        {
            //빨간색을 증가 => 녹색+빨강= 노랑
            currColor.r = (1 - currHpRate) * 2f;
        }
        else //체력이 50% 이하 일때
        {
            //녹색을 감소 =>노랑 - 녹색 = 빨강
            currColor.g = currHpRate*2f;
        }
        hpBar.color = currColor;//체력바 색상 적용
        hpBar.fillAmount=currHpRate;//체력바 크기 조절
    }
}
