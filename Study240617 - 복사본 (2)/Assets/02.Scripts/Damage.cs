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

    //��������Ʈ �� �̺�Ʈ ����
    public delegate void PlayerDieHandler();
    //��������Ʈ���� �Ļ��� �̺�Ʈ
    public static event PlayerDieHandler PlayerDieEvent;

    public Image bloodScreen;

    public Image hpBar;
    readonly Color initColor = new Color(0, 1f, 0f, 1f); //new Color ��� new Vector4�� ��밡��
    Color currColor;

    void Start()
    {
        currHp = initHp;

        //ü�¹� �ʱ� ���� ����
        hpBar.color=initColor;
        currColor = initColor;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(bulletTag))
        {
            Destroy(other.gameObject);

            //���� ȭ�� ȿ�� �ڷ�ƾ �Լ� ȣ�� 
            StartCoroutine(ShowBloodScreen());

            currHp -= 5f;
            DisplayHpBar();

           // print(currHp);

            if (currHp <= 0f)
            {
                // �÷��̾� ���� �޼��� ȣ��
                PlayerDie();
            }
        }
    }
    IEnumerator ShowBloodScreen()
    {
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //�ٽ� ���� 0 ���� 0 clear�� 0000
        bloodScreen.color = Color.clear;
    }
    void PlayerDie()
    {
    //    print("�÷��̾� ���");
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

    //    foreach(GameObject enemy in enemies)
    //    {
    //        //SendMessage("ȣ���� �޼ҵ� �̸�", ������)
    //        //SendMessage �޼ҵ�� Ư�� ��ũ��Ʈ�� �����ϴ� ���� �ƴ϶�
    //        //������Ʈ�� ���Ե� ��� ��ũ��Ʈ�� �ϳ��� �˻��ؼ� 
    //        //�ش��ϴ� �̸��� �����ϸ� �ش� �޼ҵ带 ȣ���ϴ� ���.
    //        enemy.SendMessage("E_PlayerDie", SendMessageOptions.DontRequireReceiver);
    //�̺�Ʈ ȣ��
        PlayerDieEvent();
        //�̱��� ������ �̿��Ͽ� �� ���� ����
        GameManager.instance.isGameOver = false;
    }

    void DisplayHpBar()
    {   //ü�º���
        float currHpRate = currHp / initHp;

        if (currHpRate > 0.5f)  //ü���� 50%���� ������
        {
            //�������� ���� => ���+����= ���
            currColor.r = (1 - currHpRate) * 2f;
        }
        else //ü���� 50% ���� �϶�
        {
            //����� ���� =>��� - ��� = ����
            currColor.g = currHpRate*2f;
        }
        hpBar.color = currColor;//ü�¹� ���� ����
        hpBar.fillAmount=currHpRate;//ü�¹� ũ�� ����
    }
}
