using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultipleAreaController : MonoBehaviour
{
    // 메테리얼의 리스트 정보를 저장
    [SerializeField] private List<Material> colorMaterial = new List<Material>();
    
    [SerializeField] private TextMeshProUGUI multipleText;
    [SerializeField] private GameObject coneCollection;        // 콘 콜렉션을 추가

    private MeshRenderer meshRenderer;

    private List<Transform> inputBall = new List<Transform>();

    private string materialName;
    private bool color;     // true orange. false blue
    private int multipleSize;

    private void Start() {
        meshRenderer = GetComponent<MeshRenderer>();

        coneCollection.SetActive(false);

        // 시작하면 랜덤으로 얼마나 복제해줄지 사이즈 지정
        multipleSize = Random.Range(2, 5);

        materialName = meshRenderer.material.name;

        multipleText.SetText($"X {multipleSize}");


        for(int i = 0; i < colorMaterial.Count; i++){
            if(materialName.Contains(colorMaterial[i].name)){
                if(i == 0) color = false;
                else color = true;
            }
        }

    }

    private void Update() {
        coneSpawn();
    }

    public void InputListBall(Transform tr){
        inputBall.Add(tr);
    }

    // 전달받은 아이템의 정보가 있는지 확인
    public bool InspectionList(Transform tr){
        bool check = false;
        foreach(Transform listItem in inputBall){
            if(tr.GetHashCode() == listItem.GetHashCode()){
                check = true;
                break;
            }
        }
        return check;
    }

    private void coneSpawn(){
        bool colorFlag = GameManager.instance.getColorFlag();

        bool active = colorFlag != color ? false : true;
        coneCollection.SetActive(active);
    }

    public int getMultiplesize(){
        return multipleSize;
    }

    public bool getAreaColor(){
        return color;
    }
}
