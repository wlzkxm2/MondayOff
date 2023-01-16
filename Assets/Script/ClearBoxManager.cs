using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBoxManager : MonoBehaviour
{
    [SerializeField] private Transform ClearBoxLastPos;
    private Vector3 parentPos;
    // 클리어 조건을 공 50개로
    private int clearValue = 50;

    // 골인한 아이템의 카운트
    private int goalInBallCount;
    private float time = 0f;

    private void Start() {
        parentPos = this.transform.parent.position;
    }

    private void OnTriggerEnter(Collider collider) {
        Debug.Log($"{collider.name}");
        goalInBallCount++;
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.gameObject.layer);
    }

    
    private void LateUpdate() {
        if(goalInBallCount > clearValue){
            Transform parentTr = this.transform.parent.GetComponent<Transform>();
            time += Time.deltaTime * 10f;


            parentTr.position = Vector3.Lerp(parentPos, ClearBoxLastPos.position, time);

            GameManager.instance.clearCameraViewSet(parentTr);
        }
    }
}
