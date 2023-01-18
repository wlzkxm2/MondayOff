using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClearBoxManager : MonoBehaviour
{
    [SerializeField] private Transform ClearBoxLastPos;
    [SerializeField] private Transform parentTr;
    private Vector3 parentPos;

    // 들어온 공의 리스트를 확인
    private List<Transform> inputBall = new List<Transform>();

    // 들어온 공의 갯수를 확인하기 위한 text 오브젝트
    [SerializeField] private TextMeshProUGUI text01;
    [SerializeField] private TextMeshProUGUI text02;
    // 클리어 조건을 공 50개로
    private int clearValue = 50;

    // 골인한 아이템의 카운트
    private int goalInBallCount = 0;
    private int currClearCount = 0;     // 공이 모두 떨어졋는지 체크하기 위한 변수
    
    private float fallTime = 1.5f;  // 상자가 떨어지는데 필요한 시간
    private float time = 0f;        // 현재 상자의 시간

    private bool isGoal = false;    // 골인한 아이템이 있는지 체크

    private void Start() {
        parentPos = this.transform.parent.position;
        Debug.Log($"parentPos : {parentPos}");

        StartCoroutine("valueCheck");

    }

    private void Update() {
        text01.SetText($"{goalInBallCount} / {clearValue}");
        text02.SetText($"{goalInBallCount} / {clearValue}");
        if(isGoal) GameManager.instance.clearCameraViewSet(parentTr);
    }

    
    private void LateUpdate() {
        if(goalInBallCount > clearValue){
            time += Time.deltaTime;

            // Debug.Log($"parentTr : {parentTr.position}");
            parentTr.position = Vector3.Lerp(parentPos, ClearBoxLastPos.position, time / fallTime);

            // GameManager.instance.clearCameraViewSet(parentTr);

        }
       
    }

    private void OnTriggerExit(Collider collider) {
        // Debug.Log($"{collider.name}");
        if(collider.gameObject.layer == LayerMask.NameToLayer("Ball")){
            if(!detectingBallList(collider.transform)){
                addBallList(collider.transform);

                isGoal = true;
                goalInBallCount++;

                // Debug.Log($"goalInBallCount : {goalInBallCount}");
            }
            
        }
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.gameObject.layer);
    }

    private void addBallList(Transform tr){
        inputBall.Add(tr);
    }

    private bool detectingBallList(Transform tr){
        bool check = false;
        foreach(Transform listBall in inputBall){
            if(listBall.GetHashCode() == tr.GetHashCode()){
                check = true;
                break;
            }
        }

        return check;
    }

    private IEnumerator valueCheck(){
        yield return new WaitForSeconds(.1f);
        while(true){
            // 공이 더이상 들어오는지 체크
            if(goalInBallCount > 0){
                if(currClearCount == goalInBallCount){
                    break;
                }

                currClearCount = goalInBallCount;
            }
            yield return new WaitForSeconds(1.5f);
            
        }

        if(goalInBallCount >= clearValue)   GameManager.instance.clearStageSet();
        else GameManager.instance.gameOverStageSet();
    }
}
