using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
// using 

[System.Serializable]
public struct BallsStruct{
    public Transform BallSpawnTr;
    public Transform BallStorage;
    public GameObject Ball;
}


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject DONTDESTROY_OBJECTS;
    // 게임 매니저 인스턴트화
    public static GameManager instance = null;

    // 디버깅용 텍스트
    public TextMeshProUGUI text;

    // 카메라
    [SerializeField] private Camera camera;
    private Vector3 cameraPos;
    // 카메라가 따라갈 타겟
    private Transform cameraTargetTr;

    // 게임 시작하면 있는 상자
    [SerializeField] private MovingBox movingBox;
    private MovingBox currMovingBox;

    // 떨어트릴 공 아이템의 오브젝트
    [Header("Balls Object")]
    public BallsStruct ballsStruct;
    private Transform currBallSpawnTr;
    public int startDropBallCounts = 4;

    private List<Transform> ballList = new List<Transform>();

    [Header("Touch Sensitivity")]
    [SerializeField] private float sensitivity = 1f;

    private int sceneLevel = 1;         // 처음 시작 씬 레벨

    private bool ballDrops = false;     // 상자에서 공을 떨궛나 체크
    private bool colorFlag = false;     // 색 체크
    private bool istouchUp = false;       // 터치 체크
    private bool isclearCheck = false;
    private bool isMiddleBox = false;     // 현재 상자가 중간 박스인지 아닌지

    private void Awake() {
        if(instance == null){
            instance = this;
        }else{
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(DONTDESTROY_OBJECTS);
    }

    private void Start() {
        SceneManager.LoadScene("Level01");

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
        if(cameraTargetTr != null){
            Debug.Log($"{cameraTargetTr.position}");
            Vector3 movPos;
            // if(!isclearCheck){
            //     movPos = new Vector3(cameraPos.x, cameraTargetTr.position.y, cameraPos.z);
            // }else{
            //     movPos = new Vector3(cameraPos.x, cameraTargetTr.position.y + 10f, cameraPos.z);
            // }
            movPos = new Vector3(cameraPos.x, cameraTargetTr.position.y + 10f, cameraPos.z);

            camera.transform.position = Vector3.Lerp(camera.transform.position, movPos, .2f);
            // refreshTargetPos();
        }
        refreshTargetPos();
    }

    // 게임 클리어시 씬로드
    public void clearStageSet(){
        StageManagement();
        
        Debug.Log($"CallStage");
        SceneManager.UnloadScene("Level0" + sceneLevel);
        sceneLevel++;
        // if()
        Debug.Log($"{sceneLevel}");
        SceneManager.LoadScene("Level0" + sceneLevel);
        
    }

    // 개임 오버시 씬로드
    public void gameOverStageSet(){
        Debug.Log($"GameOver");

        StageManagement();
        SceneManager.UnloadScene("Level0" + sceneLevel);
        SceneManager.LoadScene("Level01");
    }

    private void StageManagement(){
        isclearCheck = false;
        ballDrops = false;
        
        foreach(Transform tr in ballsStruct.BallStorage){
            Destroy(tr.gameObject);
        }

        ballList.Clear();

        movingBox.resetPos();
        cameraTargetTr = movingBox.transform;
    }

    /// <summary> 카메라의 자식을 공으로 해서 따라가도록 </summary>
    public void camera_posSet(Transform tr){
        cameraTargetTr = tr;
    }

    /// <summary> 공 아이템 떨어지는 동작 <summary>
    public void DropBalls(){
        if(!isMiddleBox)
        {
            StartCoroutine("DropBall");
            /*
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
            */
        }else{
            StartCoroutine("MiddleBoxDropBall");

            // // 클리어한게 아니므로 false
            // isclearCheck = false;
            // // 박스안에 있는 모든공 제거
            // for(int i = 0; i < ballsStruct.BallStorage.childCount; i++){
            //     Destroy(ballsStruct.BallStorage.GetChild(i).gameObject);
            // }
            // List<Transform> middleBoxList = new List<Transform>();
            // foreach(Transform tr in ballList){
            //     GameObject cloneBall = Instantiate(ballsStruct.Ball, BallSpawnRandomPosition(), Quaternion.identity);
            //     Debug.Log($"{BallSpawnRandomPosition()}");
            //     // cloneBall.GetComponent<SphereCollider>().enabled = false;
            //     cloneBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //     cloneBall.transform.parent = ballsStruct.BallStorage;
                
            //     Transform cloneBallTr = cloneBall.GetComponent<Transform>();
            //     middleBoxList.Add(cloneBallTr);
            // }

            // ballList.Clear();

            // ballList = middleBoxList;
        }
        // StopCoroutine("MiddleBoxDropBall");
        camera_posSet(ballList[0]);
        isMiddleBox = false;

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
                if(target != null){
                    nowPos = target.position;
                    // y가 -로 가기때문에
                    if(nowPos.y < highPos.y){
                        highPos = nowPos;
                        camera_posSet(target);
                    }
                }
            }
        }
    }

    // 카메라의 위치를 전달받은 Transform 으로 재설정
    public void clearCameraViewSet(Transform tr){
        isclearCheck = true;
        cameraTargetTr = tr;
    }

    // movingBox 함수를 재설정
    public void movingBoxChange(MovingBox movingBox, Transform spawnTr, List<Transform> listItems){
        // 현재 전달받은 moving box 데이터를 백업
        currMovingBox = this.movingBox;
        currBallSpawnTr = spawnTr;
        // ballList = listItems;

        // 현재 movingbox 를 전달받은 box 로 재설정
        this.movingBox = movingBox;
        this.ballsStruct.BallSpawnTr = spawnTr;

        ballDrops = false;
        istouchUp = false;
        isMiddleBox = true;

        StartCoroutine("MovingBoxReset");
    }

    // 공이 추가되면 공의 정보를 전부 리스트에 저장
    public void addBallList(Transform tr){
        tr.SetParent(ballsStruct.BallStorage);
        ballList.Add(tr);
    }

    // 공이 파괴되면 공으 정보를 리스트에서 삭제
    public void deleteBallList(Transform tr){
        // ballList.Remove(tr);
        // Debug.Log(ballList.Count);
        Debug.Log($"삭제전 {ballList.Count}");
        for(int i = 0; i < ballList.Count; i++){
            if(ballList[i].GetHashCode() == tr.GetHashCode()){
                ballList.RemoveAt(i);
                break;
            }
        }

        Debug.Log($"삭제후 {ballList.Count}");

        if(ballsStruct.BallStorage.childCount <= 0){
            gameOverStageSet();
        }
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
        yield return new WaitForSeconds(1f);
        istouchUp = false;
        ballDrops = true;
    }

    private IEnumerator MovingBoxReset(){
        yield return new WaitForSeconds(1.5f);
        while(true){
            if(ballDrops){
                movingBox = currMovingBox;
                ballsStruct.BallSpawnTr = currBallSpawnTr;
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    private IEnumerator DropBall(){
        int count = 0;
        // 처음 공이 4개 떨어지는 동작
        for(int i = 0; i < startDropBallCounts; i++){
            GameObject cloneBall = Instantiate(ballsStruct.Ball, BallSpawnRandomPosition(), Quaternion.identity);
            Debug.Log($"{BallSpawnRandomPosition()}");
            // cloneBall.GetComponent<SphereCollider>().enabled = false;
            cloneBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            
            Transform cloneBallTr = cloneBall.GetComponent<Transform>();

            // Transform cloneBall = ballsStruct.BallStorage.GetChild(i).GetComponent<Transform>();

            addBallList(cloneBallTr);
            count++;
            if(count%7 == 0){
                yield return new WaitForSeconds(.2f);
            }
            // Debug.Log(i);
        }
    }

    private IEnumerator MiddleBoxDropBall(){
        int count = 0;
         // 클리어한게 아니므로 false
        isclearCheck = false;
        // 박스안에 있는 모든공 제거
        for(int i = 0; i < ballsStruct.BallStorage.childCount; i++){
            Destroy(ballsStruct.BallStorage.GetChild(i).gameObject);
        }
        List<Transform> middleBoxList = new List<Transform>();
        foreach(Transform tr in ballList){
            GameObject cloneBall = Instantiate(ballsStruct.Ball, BallSpawnRandomPosition(), Quaternion.identity);
            Debug.Log($"{BallSpawnRandomPosition()}");
            // cloneBall.GetComponent<SphereCollider>().enabled = false;
            cloneBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            cloneBall.transform.parent = ballsStruct.BallStorage;
            cloneBall.layer = LayerMask.NameToLayer("Ball");
            
            Transform cloneBallTr = cloneBall.GetComponent<Transform>();
            middleBoxList.Add(cloneBallTr);
            count++;
            
            ballList = middleBoxList;
            
            if(count%7 == 0){
                yield return new WaitForSeconds(.15f);
            }
        }

        
        yield return 0;
    }
}
