using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleBoxManager : MonoBehaviour
{
    // 들어온 공의 리스트를 확인
    private List<Transform> inputBall = new List<Transform>();
    
    [SerializeField] private Transform ClearBoxLastPos;
    [SerializeField] private Transform parentTr;
    [SerializeField] private Transform spawnTr;
    private Vector3 parentPos;

    // 아이템이 연속적으로 위치가 바뀌지 않게끔 이전 전달받은 값을 확인하는 변수
    private Vector3 currpos = Vector3.zero;

    private int goalInBallCount, currClearCount = 0;

    private float nowtime = 0f;
    private float clearValue = 0f;

    private float fallTime = .8f;  // 상자가 떨어지는데 필요한 시간
    private float time = 0f;        // 현재 상자의 시간

    private bool isGoal = false;
    private bool lerpBox = false;
    private bool boxMoveCall_bool, turnbox;
    
    private void Start() {
        parentPos = parentTr.position;
        StartCoroutine("valueCheck");
    }

    private void Update() {
    }

    private void LateUpdate() {
        if(lerpBox){
            if(goalInBallCount > clearValue){
                time += Time.deltaTime;

                // Debug.Log($"parentTr : {parentTr.position}");
                parentTr.position = Vector3.Lerp(parentPos, ClearBoxLastPos.position, time / fallTime);

                // GameManager.instance.clearCameraViewSet(parentTr);
            }
        }
    }

    public void setPos(Vector3 pos){
        // 현재 박스의 포지션 값
        Vector3 thisBoxPos = this.transform.position;

        if(currpos == Vector3.zero){
            Debug.Log("null");
            currpos = pos;
        }

        // 이동하는 박스의 포지션값
        Vector3 boxPos = new Vector3(thisBoxPos.x - (currpos.x - pos.x), thisBoxPos.y, thisBoxPos.z);
        this.transform.position = new Vector3(Mathf.Clamp(boxPos.x, -2.0f, 2.0f), boxPos.y, boxPos.z);
        boxMoveCall_bool = true;
        currpos = pos;
    }

    private void OnTriggerExit(Collider collider) {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Ball")){
            if(!detectingBallList(collider.transform)){
                addBallList(collider.transform);

                isGoal = true;
                goalInBallCount++;
                if(goalInBallCount > 25){
                    collider.gameObject.layer = LayerMask.NameToLayer("NonColBall");
                }
                // Debug.Log($"goalInBallCount : {goalInBallCount}");
            }
            
        }
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

    // 공이 전부 들어왔는지 체크
    private IEnumerator valueCheck(){
        // 박스로 카메라 위치 재설정
        yield return new WaitForSeconds(.1f);
        while(true){
            if(goalInBallCount > 0){
                if(currClearCount == goalInBallCount){
                    break;
                }
                currClearCount = goalInBallCount;
            }
            yield return new WaitForSeconds(1.5f);
            
        }
        lerpBox = true;
        GameManager.instance.clearCameraViewSet(parentTr);

        yield return new WaitForSeconds(.8f);

        lerpBox = false;

        foreach(Transform tr in inputBall){
            Destroy(tr.gameObject);
        }

        MovingBox MiddleBox = parentTr.GetComponent<MovingBox>();
        MiddleBox.enabled = true;
        GameManager.instance.movingBoxChange(MiddleBox, spawnTr);

    }

    
}
