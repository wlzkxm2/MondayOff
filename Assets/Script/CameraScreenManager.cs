using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScreenManager : MonoBehaviour
{
    private Camera camera;      // 화면의 비율을 조정해줄 카메라 컴포넌트
    
    private void Awake() {
        // camera = GetComponent<Camera>();

        // Rect rect = camera.rect;

        // float scaleH = ((float) Screen.width / Screen.height) / ((float) 9 / 18);       // 가로 세로
        // float scaleW = 1f / scaleH;

        // if(scaleH < 1){
        //     rect.height = scaleH;
        //     rect.y = (1f - scaleH) / 2f;
        // }else{
        //     rect.width = scaleW;
        //     rect.x = (1f - scaleW) / 2f;
        // }

        // camera.rect = rect;
    }
}
