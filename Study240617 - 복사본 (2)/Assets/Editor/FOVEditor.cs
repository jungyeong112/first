using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//EnemyFOV라는 스크립트에 사용될 커스텀 에디터라고 명시
[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        //EnemyFOV 클래스를 참조 
        EnemyFOV fov = (EnemyFOV)target;


        //원주 위의 시작점의 좌표를 계산(뷰앵글(120도의 1/2))
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        Handles.color = Color.white;
        //원점 좌표, 노멀 벡터 , 원의 반지름
        Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.viewRange);

        //흰색이지만 투명도 20퍼짜리 색상지정
        Handles.color = new Color(1, 1, 1, 0.2f);
        //원점 좌표, 노멀 벡터, 부채꼴 시작 위치 , 부채꼴 각도, 부채꼴 범위
        Handles.DrawSolidArc(fov.transform.position, Vector3.up, fromAnglePos, fov.viewAngle, fov.viewRange);
        //텍스트 출력
        Handles.Label(fov.transform.position + (fov.transform.forward * 2f),fov.viewAngle.ToString());
    }
}
