using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{ 
    public bool open = false; 
    public float doorOpenAngle = 90f; 
    public float doorCloseAngle = 0f; 
    public float smoot = 2f; 

    // 문 상태 변경
    public void ChangeDoorState() 
    { 
        open = !open; 
    }

    // 문 열기/닫기
    void Update() 
    { 
        if (open) 
        { 
            Quaternion targetRotation = Quaternion.Euler(0, doorOpenAngle, 0); 
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoot * Time.deltaTime); 
        } 
        else 
        { 
            Quaternion targetRotation2 = Quaternion.Euler(0, doorCloseAngle, 0); 
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, smoot * Time.deltaTime); 
        } 
    }

}
