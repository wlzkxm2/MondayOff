using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// using System;

[System.Serializable]
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
    public int startDropBallCounts = 4;

    private List<Transform> ballList = new List<Transform>();

    [Header("Touch Sensitivity")]
    [SerializeField] private float sensitivity = 1f;

    private bool ballDrops = false;     // 상자에서 공을 떨궛나 체크
    private bool colorFlag = false;     // 색 체크
    private bool istouchUp = false;       // 터치 체크
    private bool isclearCheck = false;

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

        // 처음 드랍되는 공 카운트를 0보다 작은 수로 했을때
        if(startDropBallCounts <= 0) startDropBallCounts = 1;
    }

    // 마우스 위치는 0, 0, 0기준으로 잡힌다
    // 마우스를 우측으로 움직이면 +
    // 마우스를 좌측으로 움직이면 -
    private void Update()
    {
        // 터치 손을 뗀후 공이 떨어지고있는중에 박스 이동 불가
        if(!ballDrops)
        {
            if(!istouchUp){
                if(Input.GetKey(KeyCode.Mouse0)){
                    // 터치 위치를 지정
                    // 맨 왼쪽부터 0~1크기
                    // -0.5 를하게 되면 맨 왼쪽은 -0.5 중앙은 0 오른쪽은 0.5가 됨
                    Vector3 touch = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                    // text.SetText($"Debug text Touch Pos : {touch}");
                    touch = new Vector3(touch.x * sensitivity, touch.y, touch.z);
    
                    text.SetText($"Debug text Touch Pos : {touch}");

                    movingBox.setPos(touch);
                }
            }
            // 터치 손을 뗏을때
            if(Input.GetKeyUp(KeyCode.Mouse0)){
                Debug.Log("mouse Up");
                istouchUp = true;
                movingBox.boolturnboxs();
                StartCoroutine("TouchUpReset");
            }
        }else{
            if(Input.GetKeyDown(KeyCode.Mouse0)){
                // Debug.Log(ballList.Count);
                colorFlag = !colorFlag;
                foreach(Transform InItem in ballList){
                    InItem.GetComponent<BallController>().changeColor();
                }
            }
        }

    }

    private void FixedUpdate() {
        // 카메라의 움직임 제어
        if(targetTr != null){
            Vector3 movPos = new Vector3(cameraPos.x, targetTr.position.y, cameraPos.z);

            camera.transform.position = Vector3.Lerp(camera.transform.position, movPos, .2f);
            refreshTargetPos();
        }
    }

    /// <summary> 카메라의 자식을 공으로 해서 따라가도록 </summary>
    public void camera_posSet(Transform tr){
        targetTr = tr;
        // camera.transform.SetParent(tr);
    }

    /// <summary> 공 아이템 떨어지는 동작 <summary>
    public void DropBalls(){
        // 처음 공이 4개 떨어지는 동작
        for(int i = 0; i < startDropBallCounts; i++){
            GameObject cloneBall = Instantiate(ballsStruct.Ball, BallSpawnRandomPosition(), Quaternion.identity);
            Debug.Log($"{BallSpawnRandomPosition()}");
            // cloneBall.GetComponent<SphereCollider>().enabled = false;
            cloneBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            
            Transform cloneBallTr = cloneBall.GetComponent<Transform>();

            // Transform cloneBall = ballsStruct.BallStorage.GetChild(i).GetComponent<Transform>();

            addBallList(cloneBallTr);
            // Debug.Log(i);
        }
        camera_posSet(ballList[0]);

        Debug.Log(ballList);
    }

    /// <summary> 타겟의 위치를 계속 체크해서 추적할 위치를 초기화 </summary>
    private void refreshTargetPos(){
        // 공의 위치 값을 체크해줄 변수
        Vector3 nowPos = Vector3.zero;      // 현재 체크하는 포지션
        Vector3 highPos = Vector3.zero;     // 제일 선두에 있는 공의 위치
        if(!isclearCheck){
            foreach(Transform target in ballList){
                // 만약 아이템의 위치가 제일 선두에 잇다면
                nowPos = target.position;
                if(nowPos.y < highPos.y){
                    highPos = nowPos;
                    camera_posSet(target);
                }
            }
        }
    }

    public void clearCameraViewSet(Transform tr){
        isclearCheck = true;
        targetTr.position = new Vector3(tr.position.x, tr.position.y + 10f, tr.position.z);
    }

    // 공이 추가되면 공의 정보를 전부 리스트에 저장
    public void addBallList(Transform tr){
        tr.SetParent(ballsStruct.BallStorage);
        ballList.Add(tr);
    }

    // 공이 파괴되면 공으 정보를 리스트에서 삭제
    public void deleteBallList(Transform tr){
        ballList.Remove(tr);
        Debug.Log(ballList.Count);
    }

    /// <summary> 공의 색 정보를 반환 </summary>
    public bool getColorFlag(){
        return colorFlag;
    }

    /// <summary> 공이 스폰할 랜덤 범위를 반환해준다 </summary>
    private Vector3 BallSpawnRandomPosition(){
        Collider spawnCol = ballsStruct.BallSpawnTr.GetComponent<MeshCollider>();

        // 스폰 콜라이더의 사이즈를 불러온다
        float rangeX = spawnCol.bounds.size.x;
        float rangeZ = spawnCol.bounds.size.z;
        float rangeY;

        // Debug.Log($"rangeX {rangeX}, rangeZ {rangeZ}");

        // 사이즈의 랜덤 위치를 출력
        // 사이즈가 x 1 z 1 인 콜라이더가 있을때
        // /2 를 해서 0.5 즉 콜라이더의 중간 위치값을 잡고 -1 을 곱해서 시작위치 ~ 끝위치를 출력함
        rangeX = Random.Range((rangeX / 2) * -1, rangeX / 2);
        rangeY = Random.Range(-1.1f, -0.2f);
        rangeZ = Random.Range((rangeZ / 2) * -1, rangeZ / 2);
        // Debug.Log($"rangeX : {rangeX} / rangeZ : {rangeZ}");

        Vector3 randomPos = new Vector3(rangeX, rangeY, rangeZ);

        randomPos = randomPos + ballsStruct.BallSpawnTr.position;

        return randomPos;
    }

    // 손을 뗀후 bool 초기화
    private IEnumerator TouchUpReset(){
        yield return new WaitForSeconds(2f);
        istouchUp = false;
        ballDrops = true;
    }

}
