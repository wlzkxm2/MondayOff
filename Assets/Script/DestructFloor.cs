using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DestructFloor : MonoBehaviour
{
    // 블록 파괴 이펙트를 관리할 오브젝트
    [SerializeField] private GameObject Explosion;

    // 바닥이 버틸 수 있는 무게를 표시해줄 텍스트
    [SerializeField] private TextMeshProUGUI text;

    // 들어온 공의 데이터를 체크하기 위한 리스트
    // private List<Collider> inputBall = new List<Collider>();

    private int limitWeight;
    private int weight;

    private void Start() {
        limitWeight = Random.Range(2, 5) * 10;
        Debug.Log(limitWeight);
        text.SetText($"{limitWeight}");
    }

    private void OnTriggerEnter(Collider collider) {
        foreach(Transform tr in GameManager.instance.getBallList()){
            if(collider.GetHashCode() == tr.GetHashCode()){
                weight++;
            }
        }

        Debug.Log(weight);
    }
}
