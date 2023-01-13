using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[Serializable]
public struct BallsStruct{
    public Transform BallSpawnTr;
    public Transform BallStorage;
    public GameObject Ball;
}


public class GameManager : MonoBehaviour
{
    // 게임 매니저 인스턴트화
    public static GameManager instance = null;

    // 디버깅용 텍스트
    public TextMeshProUGUI text;

    // 카메라
    [SerializeField] private Camera camera;
    private Vector3 cameraPos;
    // 카메라가 따라갈 타겟
    private Transform targetTr;

    // 게임 시작하면 있는 상자
    [SerializeField] private MovingBox movingBox;

    // 떨어트릴 공 아이템의 오브젝트
    [Header("Balls Object")]
    public BallsStruct ballsStruct;
    private List<GameObject> ballList = new List<GameObject>();

    private bool touchUp = false;

    private void Awake() {
        if(instance == null){
            instance = this;
        }else{
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        // 카메라의 위치값 저장
        cameraPos = camera.transform.position;
    }

    // 마우스 위치는 0, 0, 0기준으로 잡힌다
    // 마우스를 우측으로 움직이면 +
    // 마우스를 좌측으로 움직이면 -
    private void Update()
    {
        // 터치 손을 뗀후 공이 떨어지고있는중에 박스 이동 불가
        if(!touchUp){
            if(Input.GetKey(KeyCode.Mouse0)){
                // 터치 위치를 지정
                // 맨 왼쪽부터 0~1크기
                // -0.5 를하게 되면 맨 왼쪽은 -0.5 중앙은 0 오른쪽은 0.5가 됨
                Vector3 touch = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                touch = new Vector3(touch.x - 0.5f, touch.y, touch.z);

                movingBox.setPos(touch);
            }
        }

        // 터치 손을 뗏을때
        if(Input.GetKeyUp(KeyCode.Mouse0)){
            Debug.Log("mouse Up");
            touchUp = true;
            movingBox.boolturnboxs();
            StartCoroutine("TouchUpReset");
        }
    }

    private void LateUpdate() {
        // 카메라의 움직임 제어
        if(targetTr != null){
            camera.transform.position = new Vector3(cameraPos.x, cameraPos.y + targetTr.position.y, cameraPos.z);

        }
    }

    /// <summary> 카메라의 자식을 공으로 해서 따라가도록 </summary>
    public void camera_posSet(Transform tr){
        targetTr = tr;
        // camera.transform.SetParent(tr);
    }

    /// <summary> 공 아이템 떨어지는 동작 <summary>
    public void DropBalls(){
        for(int i = 0; i < 4; i++){
            GameObject cloneBall = Instantiate(ballsStruct.Ball, ballsStruct.BallSpawnTr.position, Quaternion.identity);
            cloneBall.transform.SetParent(ballsStruct.BallStorage);
            ballList.Add(cloneBall);
            Debug.Log(i);
        }

        GameManager.instance.camera_posSet(ballList[0].transform);

        Debug.Log(ballList);
    }

    private void CopyBalls(int count){
        for(int i = 0; i < count; i++){

        }
    }

    // 손을 뗀후 bool 초기화
    private IEnumerator TouchUpReset(){
        yield return new WaitForSeconds(2f);
        touchUp = false;
    }

}
