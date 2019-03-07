using System;
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
    public List<TextMeshProUGUI> Texts;

    public GameObject Path1;
    public GameObject Path2;
    private bool _path1Selected;
    private bool _path2Selected;


    public PostProcessVolume Volume;
    private DepthOfField _depthOfField;

    public TextMeshProUGUI LoseText;
    
    
    public Collider SpeedUpTrigger;
    public Collider SpeedDownTrigger;
    public Collider Path1Trigger;
    public Collider Path2Trigger;


    private bool _speedControl;
    private bool _control = true;
    private bool _screwed;

    public Image Overlay;

    private float _distance;

    public static FirstPlayerController Singleton;


    
    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;
        _rb = GetComponent<Rigidbody>();
        Volume.profile.TryGetSettings(out _depthOfField);
        LoseText.enabled = false;
        _speedControl = true;
        Cursor.visible = false;
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

        if (_rb.velocity.magnitude > 0f)
        {
            _depthOfField.focusDistance.value = Mathf.Lerp(_depthOfField.focusDistance.value, 0f, 0.001f);
        }
        else
        {
            _depthOfField.focusDistance.value = Mathf.Lerp(_depthOfField.focusDistance.value, 0f, 0.0003f);
        }
        

 

        SetText();

        if (Overlay.color.a >= 0.95f)
        {
            _control = false;
            LoseText.enabled = true;
        }

        if (_screwed)
        {
            EndGame("You Screwed.",0.06f);
        }
        else
        {
            if (CheckOffTrack(out _distance))
            {
                EndGame("You deviate.", 0.00f);  
                Overlay.color = new Color(0,0,0, (_distance-2.5f)*0.3f);
            }
            else
            {                
                Overlay.color = new Color(0,0,0, Mathf.Lerp(Overlay.color.a, 0, 0.06f));
            }
        }

        SelectPath();

    }

    private void EndGame(String loseText, float rate)
    {
        LoseText.text = loseText;
        Overlay.color = new Color(0,0,0, Mathf.Lerp(Overlay.color.a, 1, rate));
    }

    private void SelectPath()
    {
        if (_path1Selected)
        {
            Path2.SetActive(false);
            Lines.Remove(Path2.GetComponent<Collider>());
        }
        if(_path2Selected)
        {
            Path1.SetActive(false);
            Lines.Remove(Path1.GetComponent<Collider>());
        }
    }
    
    

    private void FixedUpdate()
    {
        if (!_control) return;
        if (_speedControl)
        {
            _rb.MovePosition(transform.position + transform.forward * inputAxis.y * forceMultiplier * 2 + transform.right * inputAxis.x * forceMultiplier);
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0, inputMouse.x * mouseSensitivity * 1.3f, 0));
        }
        else
        {
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0, inputMouse.x * mouseSensitivity, 0));
            _rb.MovePosition(transform.position + transform.forward * Mathf.Lerp(_rb.velocity.x, 30f, 0.01f) + transform.right * inputAxis.x * Mathf.Lerp(_rb.velocity.y, 7f, 0.03f));
        }
        
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
            _screwed = true;
            Overlay.color = new Color(0, 0, 0, 0);
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
        if (other == SpeedDownTrigger)
        {
            _speedControl = true;
        }
        if (other == Path1Trigger)
        {
            _path1Selected = true;
            
        }
        if (other == Path2Trigger)
        {
            _path2Selected = true;
        }
        
    }


    private void Fire()
    {
        GameObject newBullet = Instantiate(BulletPrefab, transform.position + transform.forward, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * 20, ForceMode.Impulse);
        
    }


    private bool CheckOffTrack(out float distance)
    {
        distance = 999; 
        foreach (Collider line in Lines)
        {           
            if (Vector3.Distance(line.ClosestPoint(transform.position), new Vector3(transform.position.x, 0, transform.position.z)) <= 2.5f)
            {
                distance = 0;
                return false;
            }

            else
            {
                if (Vector3.Distance(line.ClosestPoint(transform.position),
                        new Vector3(transform.position.x, 0, transform.position.z)) < distance)
                    distance = Vector3.Distance(line.ClosestPoint(transform.position),
                        new Vector3(transform.position.x, 0, transform.position.z));
            }
            
        }
        
        return true;
    }

    private void SetText()
    {
        foreach (TextMeshProUGUI text in Texts)
        {
            if (transform.position.x - text.transform.position.x <= 50f * Mathf.Clamp(_rb.velocity.x/5, 1, 100) && transform.position.x - text.transform.position.x >= -1f)
            {
                text.color = new Color(1, 1, 1, Mathf.Lerp(text.color.a, 1, 0.05f));
            }
            
            if (transform.position.x - text.transform.position.x < -1f)
            {          
                Destroy(text);
                Texts.Remove(text);     
            }
            
        }
    }
    
    
    
    
    
    
    
    
}
