using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DestructFloor : MonoBehaviour
{
    // 블록 파괴 이펙트를 관리할 오브젝트
    [SerializeField] private GameObject particle;
    [SerializeField] private List<Transform> pariclePos = new List<Transform>();        // 파티클이 나올 위치

    // 바닥이 버틸 수 있는 무게를 표시해줄 텍스트
    [SerializeField] private TextMeshProUGUI text;

    // 들어온 공의 데이터를 체크하기 위한 리스트
    private List<Transform> inputBall = new List<Transform>();

    private int limitWeight;
    private int weight;

    private void Start() {
        limitWeight = Random.Range(2, 5) * 10;
        Debug.Log(limitWeight);
        text.SetText($"{limitWeight}");
    }

    public void addWeight(Transform tr){
        // bool isFlag = false;     // 중복 통과를 방지하는 플래그 기본값이 false
        // if(inputBall.Count == 0) inputBall.Add(tr);
        // foreach(Transform inputBallTr in inputBall){
        //     if(tr.GetHashCode() == inputBallTr.GetHashCode()){
        //         // Debug.Log("true");
        //         isFlag = true;
        //     }
        // }
        // weight = isFlag ? weight : weight + 1;
        // isFlag = false;
        
        // inputBall.Add(tr);
        weight++;

        Debug.Log(weight);
    }

    private void Update() {
        int value = limitWeight - weight;
        if(value <= 0){
            value = 0;
            text.SetText($"{0}");
        }else{
            text.SetText($"{value}");

        }
        if(weight >= limitWeight){
            StartCoroutine("destroydBlocks");
        }
    }

    private IEnumerator destroydBlocks(){
        yield return new WaitForSeconds(.5f);

        for(int i = 0; i < this.transform.GetChild(0).childCount; i++){
            Rigidbody rigidbody = this.transform.GetChild(0).GetChild(i).GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;

            StartCoroutine("destroydBlocks");
        }

        foreach(Transform tr in pariclePos){
            Instantiate(particle, tr.position, Quaternion.identity);
        }
        for(int i = 0; i < this.transform.GetChild(0).childCount; i++){
                GameObject meshs = this.transform.GetChild(0).GetChild(i).gameObject;
                Destroy(meshs);
        }
        this.gameObject.SetActive(false);
        text.SetText("");
    }

    
}
