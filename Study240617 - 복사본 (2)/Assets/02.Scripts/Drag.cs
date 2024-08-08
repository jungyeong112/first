using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler,IBeginDragHandler,IEndDragHandler
{
    Transform itemTr;
    Transform inventoryTr;
    public static GameObject draggingItem = null;

    Transform itemListTr;
    CanvasGroup canvasGroup;


    void Start()
    {
        itemTr = GetComponent<Transform>();
        inventoryTr = GameObject.Find("Inventory").GetComponent<Transform>();
        itemListTr=GameObject.Find("ItemList").GetComponent <Transform>();
        canvasGroup=GetComponent<CanvasGroup>();
    }

    
    void Update()
    {
        
    }

    //드래그 인터페이스의 메서드 구현
    //인터페이스 가 가지고 있는 메소드는 반드시 자식클래스에서 구현해줘야함
    //오버라이딩이다!!
    public void OnDrag(PointerEventData eventData)
    {
        itemTr.position=Input.mousePosition;
    }


    //드래그가 시작할 때 호출되는 메소드
    public void OnBeginDrag(PointerEventData eventData)
    {
        this.transform.SetParent(inventoryTr);
        //드래그가 시작될 때 드래그되는 아이템 정보 저장
        draggingItem = this.gameObject;


        //드래그가 시작되면 다른UI이벤트를 받지않도록 설정
        canvasGroup.blocksRaycasts = false;
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        //드래그가 끝날 떄 드래그 아이템을 null로 설정;
        draggingItem = null;

        canvasGroup.blocksRaycasts = true;
        //아이템 슬롯에 드래그 못했으면 다시 원복시켜주는 코드.
        if (itemTr.parent == (inventoryTr))
        {
            itemTr.SetParent(itemListTr);
        }
    }
}
