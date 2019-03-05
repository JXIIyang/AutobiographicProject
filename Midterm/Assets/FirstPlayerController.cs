using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class FirstPlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    public Vector2 inputAxis;
    public Vector2 inputMouse;

    public GameObject mainCamera;
    
    
    public float forceMultiplier;
    public float mouseSensitivity;
    public float maxSpeed;
    public float JumpForce;

    private float rotateMouseX;
    private float rotateMouseY;

    private bool OnGround;
    private float JumpTime;

    public GameObject BulletPrefab;

    public List<Collider> Lines;



    public PostProcessVolume Volume;
    private DepthOfField _depthOfField;

    public TextMeshProUGUI LoseText;
    
    
    public Collider SpeedUpTrigger;


    private bool _speedControl;

    public Image Overlay;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Volume.profile.TryGetSettings(out _depthOfField);
        LoseText.enabled = false;
        _speedControl = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        inputAxis.y = Input.GetAxis("Vertical");
        inputAxis.x = Input.GetAxis("Horizontal");
        
        
        
        inputMouse.x = Input.GetAxis("Mouse X");
        inputMouse.y += Input.GetAxis("Mouse Y");

        inputMouse.y = Mathf.Clamp(inputMouse.y, -90, 60);
        
        mainCamera.transform.localEulerAngles = new Vector3(- inputMouse.y * mouseSensitivity, 0, 0);

        if (Input.GetMouseButton(0))
        {
            Fire();
        }


/*        maxSpeed += 0.0001f;*/

        _depthOfField.focusDistance.value = Mathf.Lerp(_depthOfField.focusDistance.value, 0f, 0.001f);

        CheckOffTrack();
        if (CheckOffTrack())
        {
            Overlay.color = new Color(0,0,0, Mathf.Lerp(Overlay.color.a, 1, Time.deltaTime));
        }
        else
        {
            Overlay.color = new Color(0,0,0, Mathf.Lerp(Overlay.color.a, 0, Time.deltaTime));
        }
        


    }

    private void FixedUpdate()
    {
        if (_speedControl)
        {
            _rb.MovePosition(transform.position + transform.forward * inputAxis.y * forceMultiplier + transform.right * inputAxis.x * forceMultiplier);
        }
        else
        {
            _rb.MovePosition(transform.position + transform.forward * Mathf.Lerp(_rb.velocity.x, 20f, 0.03f) + transform.right * inputAxis.x * Mathf.Lerp(_rb.velocity.y, 20f, 0.03f));
        }
        
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0, inputMouse.x * mouseSensitivity * 1.3f, 0));

        
        
        

        if (OnGround)
            JumpTime = 0;
        else
        {
            JumpTime += Time.deltaTime;
            if (!Input.GetKey(KeyCode.UpArrow))
                JumpTime = 999;
        }
        
        

        if (Input.GetKey(KeyCode.Space) && (OnGround || JumpTime < 0.3f))
        {
            _rb.velocity = new Vector3(_rb.velocity.x, JumpForce, _rb.velocity.z);
            OnGround = false;
        }

    }

    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Floor"))
        {
            if (!Input.GetKey(KeyCode.Space)) OnGround = true;
        }
        else
        {
            _rb.angularVelocity = Vector3.zero;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Lense"))
        {
            _depthOfField.focusDistance.value = 10f;
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Obstacle"))
        {
/*            _rb.isKinematic = true;*/
            LoseText.enabled = true;

        }
        if (other.gameObject.CompareTag("Finish"))
        {
/*            _rb.isKinematic = true;*/
              LoseText.text = "You Win!";
              LoseText.enabled = true;


        }
        
        if (other == SpeedUpTrigger)
        {
            _speedControl = false;
        }
        
    }


    private void Fire()
    {
        GameObject newBullet = Instantiate(BulletPrefab, transform.position + transform.forward, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * 20, ForceMode.Impulse);
        
    }


    private bool CheckOffTrack()
    {
        foreach (Collider line in Lines)
        {           
            if (Vector3.Distance(line.ClosestPoint(transform.position), transform.position) <= 10f)
            {
                return false;
            }
        }

        return true;
    }
    
    
    
    
    
    
}
