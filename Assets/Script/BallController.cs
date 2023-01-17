using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BallParticleStruct{
    public GameObject bounceParticle;
    public GameObject destroyParticle;
}

public class BallController : MonoBehaviour
{
    [Header("Input BallMaterials")]
    // 전환할 공의 색 리스트
    [SerializeField] private List<Material> ballMaterial = new List<Material>();

    // 공의 파티클을 저장할 스트럭트
    [SerializeField] private BallParticleStruct ballParticle;

    private MeshRenderer meshRenderer;
    private Rigidbody rigidbody;

    private MultipleAreaController multipleArea;
    private DestructFloor destructFloor;

    private bool colorFlag;     // 공의 색을 설정해줄 플래그 0과 1 로 확인 가능 0은 false 1은 true
                                // true orange. false blue

    // 전달받을 mumtiple 값
    private int multipleSize;

    private void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
        // 처음 공의 색을 첫번째 리스트 메테리얼로 설정
        colorFlag = GameManager.instance.getColorFlag();
        changeColor();
    }

    private void Update() {
        // rigidbody.velocity = new Vector3(2,2,2);
        // if(rigidbody.velocity.y > 1.5f){
        //     Debug.Log("velo High!!!!!!!!!!");
        //     rigidbody.velocity = new Vector3(1.5f, 1.5f, 1.5f);
        // }
    }

    private void FixedUpdate() {
        
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y, -15f, -3f), 0f);
    }

    private void setAreaController(Collider collider){
        multipleArea = collider.GetComponent<MultipleAreaController>();
    }

    private void setBrokenFllorController(Collider collider){
        destructFloor = collider.GetComponent<DestructFloor>();
    }

    // 장애물과 부딪혔을때 파티클 효과 설정
    private void OnCollisionEnter(Collision collision) {
        if(collision.collider.gameObject.CompareTag("Obstacle")){
            // Debug.Log(collision.contacts[0].point);      // 충돌한 위치에 대한 정보를 추출
            Vector3 contactPos = collision.contacts[0].point;
            Instantiate(ballParticle.bounceParticle, contactPos, Quaternion.identity);
        }   
        
    }
    
    // 트리거 콜라이더에 진입했을때
    private void OnTriggerEnter(Collider collider) {
        // 레이어가 복제 해주는 레이어라면
        if(collider.gameObject.layer == LayerMask.NameToLayer("Multiple")){
            // 에리어 컴포넌트 호출
            setAreaController(collider);

            // 에리어와 현재 공의 색이 다를경우 현재 공 파괴
            if(colorFlag != multipleArea.getAreaColor()){
                Instantiate(ballParticle.destroyParticle, this.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
                GameManager.instance.deleteBallList(this.transform);
            }

        }

        if(collider.gameObject.CompareTag("OutWorld")){
            Destroy(this.gameObject);
        }
    }

    // 볼이 나올때 해당 에리어에서 전달받은 multiple 값만큼 복제되서 나옴
    private void OnTriggerExit(Collider collider) {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Multiple")){

            setAreaController(collider);

            multipleSize = multipleArea.getMultiplesize();

            StartCoroutine("copyBallsCour");
        }

        if(collider.gameObject.layer == LayerMask.NameToLayer("BrokenFloor")){
            setBrokenFllorController(collider);

            destructFloor.addWeight(this.transform);

        }
    }

    public void changeColor(){
        if(colorFlag){
            // colorFlag가 false 값일땐 파란색으로 변경 해주기
            colorFlag = false;
            meshRenderer.material = ballMaterial[0];
        }else{
            // colorFlag가 true 주황색으로 변경 해주기
            colorFlag = true;
            meshRenderer.material = ballMaterial[1];
        }
    }

    private IEnumerator copyBallsCour(){
        yield return new WaitForSeconds(.08f);
        
        Vector3 copySpawnPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        
        if(!multipleArea.InspectionList(this.transform)){
            for(int i = 0; i < multipleSize; i++){
                GameObject copyBalls = Instantiate(this.gameObject, copySpawnPos, Quaternion.identity);
                GameManager.instance.addBallList(copyBalls.transform);
                multipleArea.InputListBall(copyBalls.transform);

                yield return new WaitForSeconds(.05f);
            }
        }
    }
}
