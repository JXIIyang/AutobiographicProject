using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    public Vector2 inputAxis;
    public Vector2 inputMouse;

    public Camera mainCamera;
    
    
    public float forceMultiplier;
    public float mouseSensitivity;
    public float maxSpeed;
    public float jumpForce;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        inputAxis.y = Input.GetAxis("Vertical");
        inputAxis.x = Input.GetAxis("Horizontal");

        inputMouse.x = Input.GetAxis("Mouse X");
        inputMouse.y = Input.GetAxis("Mouse Y");
/*        if (mainCamera.transform.eulerAngles.x >= 270 || mainCamera.transform.eulerAngles.x <= 65 )
        {*/
            mainCamera.transform.Rotate(-inputMouse.y * mouseSensitivity * Time.deltaTime, 0, 0);
            Debug.Log(mainCamera.transform.eulerAngles.x);
/*        }
        else 
        {
            if (mainCamera.transform.eulerAngles.x < 270 && mainCamera.transform.eulerAngles.x >= 150)
            {
                mainCamera.transform.rotation = Quaternion.Euler(270, mainCamera.transform.eulerAngles.y, mainCamera.transform.eulerAngles.z);
                Debug.Log(mainCamera.transform.eulerAngles.x);
            }
            if (mainCamera.transform.eulerAngles.x > 65 && mainCamera.transform.eulerAngles.x < 150)
            {
                mainCamera.transform.rotation = Quaternion.Euler(65, mainCamera.transform.eulerAngles.y, mainCamera.transform.eulerAngles.z);
                Debug.Log(mainCamera.transform.eulerAngles.x);
            }           
       }*/
    }

    private void FixedUpdate()
    {

/*            _rb.MovePosition(transform.position + transform.forward * inputAxis.y * forceMultiplier);*/
            
            _rb.MovePosition(transform.position + transform.forward * inputAxis.y * forceMultiplier  * Time.deltaTime + transform.right * inputAxis.x * forceMultiplier * Time.deltaTime);
/*            _rb.AddForce(transform.right * inputAxis.x * forceMultiplier, ForceMode.Impulse);*/

/*        _rb.transform.Rotate(0, inputMouse.x * mouseSensitivity * 2, 0);**/
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0, inputMouse.x * mouseSensitivity * Time.deltaTime, 0));


        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
}
