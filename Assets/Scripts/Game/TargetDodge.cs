using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDodge : MonoBehaviour {
    
    void Update() {
        if (Input.GetKey(KeyCode.Space))
            transform.position = Input.mousePosition;
    }
}
