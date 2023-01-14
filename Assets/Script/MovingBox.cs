using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 박스 움직임에 대한 스크립트
public class MovingBox : MonoBehaviour
{
    // 아이템이 연속적으로 위치가 바뀌지 않게끔 이전 전달받은 값을 확인하는 변수
    private Vector3 currpos;

    private bool boxMoveCall_bool;
    private bool turnbox;

    private float nowtime = 0;

    public void setPos(Vector3 pos){
        // 현재 박스의 포지션 값
        Vector3 thisBoxPos = this.transform.position;

        // 이동하는 박스의 포지션값
        Vector3 boxPos = new Vector3(thisBoxPos.x - (currpos.x - pos.x), thisBoxPos.y, thisBoxPos.z);
        this.transform.position = new Vector3(Mathf.Clamp(boxPos.x, -2.3f, 2.3f), boxPos.y, boxPos.z);
        boxMoveCall_bool = true;
        currpos = pos;
    }

    public void boolturnboxs(){
        turnbox = true;
        
        StartCoroutine("TrunBox");
        // rotation();
    }

    private IEnumerator TrunBox(){
        yield return new WaitForSeconds(0.1f);
        // 상자의 목표 회전값
        Quaternion endrotation = Quaternion.Euler(0, 0, 180f);

        while(true){
            yield return new WaitForSeconds(0.1f);
            // 박스를 돌린다
            if(turnbox){       
                nowtime += Time.deltaTime * 30f;
                Debug.Log(nowtime);
                // 1.5초 동안 박스가 돌아간다
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, endrotation, nowtime);
            }


            if(Mathf.Abs(this.transform.rotation.z) == 1){
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, this.transform.rotation.y, 180f);
                GameManager.instance.DropBalls();
                yield break;
            }

        }
        
    }

}
