using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("리스폰 관련 내용")]
    public Transform[] points;
    public GameObject enemy;
    public float createTime = 2f;//생성주기 
    public int maxEnemy = 10;//최대 생성 수

    public bool isGameOver = false;

    [Header("오브젝트 풀 관련 내용")]
    public GameObject bulletPrefab;
   
    public int maxPool = 10;
    public List<GameObject> bulletPool=new List<GameObject>();
    //싱글턴 패턴을 활용하여 해당 스크립트에 접근하기 위한 변수
    public static GameManager instance=null;

    public CanvasGroup inventoryCG;

    [HideInInspector]
    public int KillCount;
    public Text killCountTxt;

    void LoadGameData()
    {
        //지정된 값으로 저장소에서 저장된 값을 가져온다.
        //이때 정수나 실수는 초기값을 지정해줘도 됨
        //GetInt("키 값", 초기 값)
        KillCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        killCountTxt.text = "KILL" + KillCount.ToString(" 0000");
    }
    public void IncKillCount()
    {
        KillCount++;
        killCountTxt.text = "KILL" + KillCount.ToString(" 0000");

        //저장소에 KILL_COUNT라는 키값으로 KillCount 값을 저장.
        PlayerPrefs.SetInt("KILL_COUNT", KillCount);
    }
 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        //씬 변경이 발생해도 해당 게임 오브젝트 파괴안함
        //게임이 종료될 때까 지 쭉 감.
        DontDestroyOnLoad(this.gameObject);

        //저장된 게임 데이터 불러오기 
        LoadGameData();

        //오브젝트 풀 함수 호출
        CreatObjectPool();
    }



    void Start()
    {
        OnInventoryOpen(false);

        //배열이라 인덱스 첫번째 자리를 지울 수 없기때문에
       points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        if (points.Length > 0)
        {
            //생성 코루틴 함수 호출
            StartCoroutine(CreateEnemy());
        }
    }
    IEnumerator CreateEnemy()
    {
        while (!isGameOver)
        {
            //ENEMY 태그를 지닌 오브젝트를 검사해서 길이(크기 ) 값을 저장
            int enemyCount = GameObject.FindGameObjectsWithTag("ENEMY").Length;
            if (enemyCount < maxEnemy)
            {
                //생성 주기 만큼 대기
                yield return new WaitForSeconds(createTime);

                //저장된 리스폰 포인트의 길이 만큼 랜덤한 숫자 추출 
                //해당 인덱스를 통하여 생성 위치를 랜덤하게 설정함
                int idx=Random.Range(1, points.Length);

                Instantiate(enemy, points[idx].position, points[idx].rotation);
            }
            else //리스폰 된 Enemy의 수가 Max에 도달했을 때
            {
                yield return null;
            }
        }
    }
    //오브젝트 풀 관련 메소드
    public void CreatObjectPool()
    {
        //ObjectPools 라는 이름의 빈 오브젝트를 생성
        GameObject objectPools = new GameObject("ObjectPools");

        //풀링 갯수만큼 총알을 생성하기 위한 반복문
        for (int i = 0; i < maxPool; i++)
        {
            //총알 프리팹을 동적 생성 하면서 
            //위에서 생성한 ObjectPools의 자식으로 바로 넣어줌 
            var bullet = Instantiate<GameObject>(bulletPrefab, objectPools.transform);
            bullet.name="Bullet_"+i.ToString("00");
            //총알 비활성화
            bullet.SetActive(false);
            //오브젝트풀(리스트)에 생성된 총알 추가 
            bulletPool.Add(bullet);
        }
    }
    //오브젝트 풀에서 놀고 있는 총알을 골라서 반환
    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            //풀에서 선택한 총알의 상태가 Active False일 경우
            if (!bulletPool[i].activeSelf)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    bool isPaused;
    public void OnPauseClick()
    {
        isPaused = !isPaused;


        //bool 변수가 true면 0 아니면 1
        //timeScale은 1을 기준으로 작아지면 느려지고 커지면 빨라진다 
        //0이되면 일시정지이나 최대 4이상 높이지는 않는것이 좋다 
        //모바일 기준 발열 및 기타 문제가 있기 때문이다.
        Time.timeScale = (isPaused) ? 0f : 1f;

        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        //플레잉어에 추가된 스크립트 모두 가져오기
        //MonoBehaviour를 가진 스크립트를 전부 다 가져옴
        var scripts =playerObj.GetComponents<MonoBehaviour>();

        //일시정지일때 모든 스크립트 정지 
        //일시정 지 해제되면 다시동작
        foreach (var script in scripts)
        {
            script.enabled=!isPaused;
        }

        //무기 교체 UI의 CanvasGroup을 제어하기 위한 코드 추가
        var canvasGroup=GameObject.Find("Panel_Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }
    public void OnInventoryOpen(bool isOpened)
    {
        inventoryCG.alpha = (isOpened) ? 1 : 0f;
        //투명도가 0이 되어서 UI가 뵈지 않더라도
        //레이캐스트에 의한 터치 이벤트트 발생하기 때문에 아래코드를 통해서 터치이벤트를 무시하도록 설정.
        inventoryCG.interactable = isOpened;
        inventoryCG.blocksRaycasts = isOpened;

    }
}
