using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            // ����ũ ����Ʈ �����ϴ� �޼ҵ� ȣ��
            ShowEffect(collision);

            // collider.tag == "" �� ����� �� Ÿ�⶧���� CompareTag �ص���
           // Destroy(collision.gameObject);

            //������Ʈ Ǯ�� �Ѿ��� ��Ȱ��ȭ ���·� ����.
           collision.gameObject.SetActive(false);
        }
    }

    void ShowEffect(Collision collision)
    {
        // �浹 ������ �ϳ��� �ƴ� �� �����Ƿ� �� �� �ϳ��� �޾ƿ´ٴ� �ǹ�
        ContactPoint contactPoint = collision.contacts[0];

        // ���� ���Ͱ� �̷�� ������ ������ (�������Ͷ� �浹������ ������ �̷�� �ݴ������)
        // �׷��Ƿ� Vector3.forward�� ���̳ʰ� ����
        // FromToRotation(A, B) = A�� ������ B �������� �����ٴ� �ǹ�
        // ��, ���� ������ �Ǵ� �븻������ �������� �����شٴ� �ǹ�
        Quaternion rotation = Quaternion.FromToRotation(-Vector3.forward, contactPoint.normal);

        // �浹 �������� ����ũ ����Ʈ�� ������(Z�� ������ ����������)�� ����������
        GameObject effect=Instantiate(sparkEffect, contactPoint.point, rotation);
        //���������� ź�� ȿ���� �θ� �������־�
        //���� �����̵�����.
        effect.transform.SetParent(this.transform);
    }
}