using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBoxManager : MonoBehaviour
{
    // 클리어 조건을 공 50개로
    private int clearValue = 50;

    // 골인한 아이템의 카운트
    private int goalInBallCount;

    private void OnTriggerEnter(Collider collider) {
        Debug.Log($"{collider.name}");
        goalInBallCount++;
    }
    
    private void LateUpdate() {
        if(goalInBallCount > clearValue){
            // gameObject.transform.position = 
        }
    }
}
