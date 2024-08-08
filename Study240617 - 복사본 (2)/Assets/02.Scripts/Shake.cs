using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Shake : MonoBehaviour
{
    //쉐이크 효과 줄 카메라 
    public Transform shakeCamera;

    public bool shakeRotate = false;

    Vector3 originPos;
    Quaternion originRot;

    void Start()
    {
        //흔들 기 전 원래 위치 값과 회전값 저장해두기 
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
    }
    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f,
        float magnitudeRot = 0.1f)
    {
        float passTime = 0f;
        //duration 타임 동안  흔들기위해서 while 사용
        while (passTime < duration)
        {
            //반지름이 1인 구형의 공간ㅇ 안에서 랜덤한 3개의 좌표(x,y,z)추출 
            Vector3 shakePos = Random.insideUnitSphere;
            //위에서 뽑은 랜덤위치와 매개변수를 통해서 흔들기 
            shakeCamera.localPosition = shakePos * magnitudePos;
            //불규칙한 회전 사용 유무 
            if (shakeRotate)
            {
                //수학의 펄린 노이즈 함수로서 어떤 불규칙한 패턴을 가져오고자 함
                //랜덤  맵 생성 등에 쓰임. 불규칙한 잔디 및 구름에도 사용
                float z = Mathf.PerlinNoise(Time.time * magnitudeRot, 0f);
                Vector3 shakeRot = new Vector3(0, 0, z);

                shakeCamera.localRotation = Quaternion.Euler(shakeRot);

            }
            passTime += Time.deltaTime;
            yield return null;
        }
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;
    }

    void Update()
    {

    }
}
