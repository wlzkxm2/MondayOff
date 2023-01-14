using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private int multipleSize;

    private void OnTriggerExit(Collider collider) {
        // GameManager.instance.CopyBalls(this.gameObject, collider);
        multipleSize = collider.GetComponent<MultipleAreaController>().getMultiplesize();

        copyBalls();
    }

    private void copyBalls(){
        for(int i = 0; i < multipleSize; i++){
            GameObject copyBalls = Instantiate(this.gameObject, this.transform.position, Quaternion.identity);
            GameManager.instance.addBallList(copyBalls.transform);
        }
    }
}
