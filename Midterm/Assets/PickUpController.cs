using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, FirstPlayerController.Singleton.transform.position) < 2f)
        {
            transform.position =
                Vector3.Lerp(transform.position, FirstPlayerController.Singleton.transform.position, 0.3f);
        }
    }
}
