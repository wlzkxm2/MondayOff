using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDestroyController : MonoBehaviour
{
    public Transform object01;
    public Transform object02;
    public Transform object03;

    float time = 0f;

    public int counts;

    private void Start() {
        Debug.DrawLine(object01.position, object02.position, Color.red);
        setPos();
    }

    private void Update() {
        Debug.DrawLine(object01.position, object02.position, Color.red);

        float y = Mathf.Lerp(object01.position.y, object03.position.y, .3f);

        Vector3 objVec = new Vector3(object03.position.x, y, object03.position.y);


        // Debug.DrawLine(object01.position, objVec, Color.red);
        // Debug.DrawLine(object03.position, object02.position, Color.red);

        setPos();
        
    }

    private void setPos(){
        List<Vector3> lerpPos = new List<Vector3>();

        Vector3 pointPos = new Vector3(object01.position.x, object03.position.y, object03.position.z);

        Debug.DrawLine(pointPos, object03.position, Color.red);
        Debug.DrawLine(pointPos, object01.position, Color.red);

        for(int i = 0; i <= counts; i++){
            // lerpPos.Add(Vector3.Slerp(object01.position, object03.position, i / 10));

            // pointPos를 중심으로 원의 위치를 확인 
            float x = pointPos.x + (object03.position.x - object01.position.x) * Mathf.Cos(i);
            float y = pointPos.y + (object03.position.y - object01.position.y) * Mathf.Sin(i);
            
            Vector3 radiusVec = new Vector3(x, y, object03.position.z);
            if(object03.position.x >= radiusVec.x && radiusVec.x >= object01.position.x && object03.position.y <= radiusVec.y && radiusVec.y <= object01.position.x ){
                Debug.DrawLine(pointPos, new Vector3(x, y, pointPos.z), Color.blue);
                // Debug.DrawLine(object01.position, new Vector3(x, y, pointPos.z), Color.red);
                lerpPos.Add(radiusVec);
            }
            
        }

        for(int i = 0; i < lerpPos.Count; i++){
            if(i == lerpPos.Count - 1){
                Debug.DrawLine(lerpPos[i], object03.position, Color.red);
            }else{
                Debug.DrawLine(lerpPos[i], lerpPos[i+1], Color.red);
            }

        }
    }
}
