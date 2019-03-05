using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    private Rigidbody _rb;
    public Vector2 inputAxis;
    public float forceMultiplier;
    public float torqueMultiplier;
    public float maxSpeed;
    
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
        
    }

    private void FixedUpdate()
    {

        if (_rb.velocity.magnitude <= maxSpeed)
        {
        _rb.AddForce(transform.forward * inputAxis.y * forceMultiplier, ForceMode.Impulse);
/*        _rb.AddForce(transform.right * inputAxis.x * forceMultiplier, ForceMode.Impulse);*/
                    
        }
        
        
        _rb.AddTorque(0, inputAxis.x * torqueMultiplier, 0, ForceMode.Impulse);
    }
    
}
