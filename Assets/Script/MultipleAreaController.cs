using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleAreaController : MonoBehaviour
{
    // 메테리얼의 리스트 정보를 저장
    [SerializeField] private List<Material> colorMaterial = new List<Material>();

    private MeshRenderer meshRenderer;

    private string materialName;
    private bool color;     // true orange. false blue
    private int multipleSize;

    private void Start() {
        meshRenderer = GetComponent<MeshRenderer>();

        // 시작하면 랜덤으로 얼마나 복제해줄지 사이즈 지정
        multipleSize = Random.Range(2, 5);

        materialName = meshRenderer.material.name;


        for(int i = 0; i < colorMaterial.Count; i++){
            if(materialName.Contains(colorMaterial[i].name)){
                if(i == 0) color = false;
                else color = true;
            }
        }

    }

    public int getMultiplesize(){
        return multipleSize;
    }

    public bool getAreaColor(){
        return color;
    }
}
