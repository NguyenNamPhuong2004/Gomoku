using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScale : MonoBehaviour
{ 
     private void Start()
     {
          ScaleObject();
     }

     private void ScaleObject()
     {
        float width = ((float)16/9) / ((float)Screen.width / (float)Screen.height);
       // float height = ((float)9/16) / ((float)Screen.height / (float)Screen.width);
        transform.localScale = new Vector3(transform.localScale.x / width, transform.localScale.y, transform.localScale.z);
     }
}
  