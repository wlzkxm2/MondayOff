using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Input BallMaterials")]
    // 전환할 공의 색 리스트
    [SerializeField] private List<Material> ballMaterial = new List<Material>();
    private bool colorFlag;     // 공의 색을 설정해줄 플래그 0과 1 로 확인 가능 0은 false 1은 true
                                // true orange. false blue

    private MeshRenderer meshRenderer;
    private MultipleAreaController multipleArea;

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
        setAreaController(collider);

        if(colorFlag != multipleArea.getAreaColor()){
            Destroy(this.gameObject);
            GameManager.instance.deleteBallList(this.transform);
        }
    }

    // 볼이 나올때 해당 에리어에서 전달받은 multiple 값만큼 복제되서 나옴
    private void OnTriggerExit(Collider collider) {
        setAreaController(collider);

        multipleSize = multipleArea.getMultiplesize();

        copyBalls();
    }

    private void copyBalls(){
        for(int i = 0; i < multipleSize; i++){
            GameObject copyBalls = Instantiate(this.gameObject, this.transform.position, Quaternion.identity);
            GameManager.instance.addBallList(copyBalls.transform);
        }
    }

    public void changeColor(){
        if(colorFlag){
            // colorFlag가 True 값일땐 파란색으로 변경 해주기
            colorFlag = false;
            meshRenderer.material = ballMaterial[0];
        }else{
            // colorFlag가 False값일땐 주황색으로 변경 해주기
            colorFlag = true;
            meshRenderer.material = ballMaterial[1];
        }
    }
}
