using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Input BallMaterials")]
    // 전환할 공의 색 리스트
    [SerializeField] private List<Material> ballMaterial = new List<Material>();

    private MeshRenderer meshRenderer;
    private MultipleAreaController multipleArea;

    private bool colorFlag;     // 공의 색을 설정해줄 플래그 0과 1 로 확인 가능 0은 false 1은 true
                                // true orange. false blue

    // 전달받을 mumtiple 값
    private int multipleSize;

    private void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        // 처음 공의 색을 첫번째 리스트 메테리얼로 설정
        colorFlag = GameManager.instance.getColorFlag();
        changeColor();
    }

    private void setAreaController(Collider collider){
        multipleArea = collider.GetComponent<MultipleAreaController>();
    }
    
    private void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Multiple")){
            setAreaController(collider);

            if(colorFlag != multipleArea.getAreaColor()){
                Destroy(this.gameObject);
                GameManager.instance.deleteBallList(this.transform);
            }

        }
    }

    // 볼이 나올때 해당 에리어에서 전달받은 multiple 값만큼 복제되서 나옴
    private void OnTriggerExit(Collider collider) {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Multiple")){

            setAreaController(collider);

            multipleSize = multipleArea.getMultiplesize();

            StartCoroutine("copyBallsCour");
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
