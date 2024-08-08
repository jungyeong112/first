using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    public string childTag;


    void Start()
    {
        
    }

    void Update()
    {
    }
    public void OnDrop(PointerEventData eventData)
    {
        childTag = Drag.draggingItem.tag;

        Debug.Log(childTag);
        if (childTag==("ItemShock"))
        {
            Debug.Log("Shock");
           
        }
        //slot�� �ڽ��� ������ 0�̶�� �ǹ̴� ���� �ڽĿ�����Ʈ�� ���ٴ� ���.
        if (transform.childCount == 0)
        {
            //�巡�� ���� �������� slot�� �ڽ����� ���
            Drag.draggingItem.transform.SetParent(this.transform);
        }
    }
}
