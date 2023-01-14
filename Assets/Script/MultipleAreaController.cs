using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleAreaController : MonoBehaviour
{
    private int multipleSize;

    private void Start() {
        // 시작하면 랜덤으로 얼마나 복제해줄지 사이즈 지정
        multipleSize = Random.Range(2, 5);
    }

    public int getMultiplesize(){
        return multipleSize;
    }
}
