﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class FirstPlayerController : MonoBehaviour
{
    public Rigidbody Rb;
    public Vector2 inputAxis;
    public Vector2 inputMouse;

    public GameObject mainCamera;

    private AudioSource _audio;
    
    
    
    public float forceMultiplier;
    public float mouseSensitivity;
    public float JumpForce;

    private float rotateMouseX;
    private float rotateMouseY;

    private bool OnGround;
    private float JumpTime;

    public GameObject Face1;
    public Animator Anim1;
    
   
    
    public GameObject Face2;
    public Animator Anim2_1;
    public Animator Anim2_2;
    public Animator Anim2_3;
    public GameObject Face3;
    public Animator Anim3_1;
    public Animator Anim3_2;
    public Animator Anim3_3;
    public Animator Anim3_4;
    public Animator Anim3_5;

    public GameObject EMesh;
    public GameObject GMesh;
    public GameObject OMesh;

    public Collider Face1_Trigger;
    public Collider Face2_Trigger;
    public Collider Face3_Trigger;
    
    

    public List<Collider> Lines;
    public List<TextMeshProUGUI> Texts;

//    public GameObject Path1;
//    public GameObject Path2;
    
    public Collider Option1_1;
    public Collider Option1_2;
    public Collider Option2_1;
    public Collider Option2_2;
    public Collider Option3_1;
    public Collider Option3_2;
    public Collider Option4_1;
    public Collider Option4_2;
    public Collider ETrigger;
    public Collider GTrigger;
    public Collider OTrigger;
    public GameObject Mask1;
    public GameObject Mask2;
    public GameObject Mask3;
    public GameObject E;
    public GameObject G;
    public GameObject O;
    public GameObject Mirror;
    
    private bool _path1Selected;
    private bool _path2Selected;


    public PostProcessVolume Volume;
    private DepthOfField _depthOfField;

    public TextMeshProUGUI LoseText;
    
    
    public Collider SpeedUpTrigger;
    public Collider EndTrigger;
//    public Collider Path1Trigger;
//    public Collider Path2Trigger;
    public Collider Option1Trigger;
    public Collider Option2Trigger;
    public Collider Option3Trigger;
    public Collider Option4Trigger;


    public bool _speedControl;
    private bool _control = true;
    private bool _screwed;
    private bool _sank;

    public Image Overlay;

    private float _distance;

    public static FirstPlayerController Singleton;

    private GameObject _mirror;

    private bool _endGame;
    private bool _E_QTE;
    private bool _G_QTE;
    private bool _O_QTE;
    


    
    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;
        Rb = GetComponent<Rigidbody>();
        Volume.profile.TryGetSettings(out _depthOfField);
        LoseText.enabled = false;
//        _speedControl = true;
        Cursor.visible = false;
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
        Debug.Log(lines.Length);
        foreach (GameObject line in lines)
        {
          var col = line.GetComponent<Collider>();
          Lines.Add(col);
        }

        _audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_endGame)
        {
            EndGame("A Game By Eleanor Yang", 0.01f);
            Rb.velocity = Vector3.zero;
            return;
        }
        
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

//        if (Input.GetMouseButton(0))
//        {
//            Fire();
//        }


/*        maxSpeed += 0.0001f;*/

        if (Rb.velocity.magnitude > 0f)
        {
            _depthOfField.focusDistance.value = Mathf.Lerp(_depthOfField.focusDistance.value, 0f, 0.001f);
        }
        else
        {
            _depthOfField.focusDistance.value = Mathf.Lerp(_depthOfField.focusDistance.value, 0f, 0.0003f);
        }
        

 

        SetText();
/*

        if (Overlay.color.a >= 0.95f)
        {
            
            LoseText.enabled = true;
        }*/

        if (_screwed)
        {
            EndGame("You screwed up.",0.06f);
        }
        if (_sank)
        {
            EndGame("You are overwhelmed.",0.1f);
        }
        
        else
        {
            if (CheckOffTrack(out _distance))
            {
                EndGame("You deviate.", 0.00f);  
                Overlay.color = new Color(0,0,0, (_distance-2.5f)*0.3f);
                _audio.pitch = 1 - Mathf.Clamp((_distance - 3.2f) *0.02f, 0, 0.2f);
            }
            else
            {                
                Overlay.color = new Color(0,0,0, Mathf.Lerp(Overlay.color.a, 0, 0.06f));
                _audio.pitch = 1;
            }
        }

//        SelectPath();

        if (_E_QTE)
        {

            if (Input.GetKeyDown(KeyCode.E))
            {
                Anim1.SetTrigger("Fracture");
                Destroy(Face1_Trigger.gameObject);
                Destroy(EMesh);
                _E_QTE = false;
            }
        }

        if (_G_QTE)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
              Anim2_1.SetTrigger("Fracture");
              Anim2_2.SetTrigger("Fracture");
              Anim2_3.SetTrigger("Fracture");
              Destroy(Face2_Trigger.gameObject);
              Destroy(GMesh);
              _G_QTE = false;
            }
        }

        if (_O_QTE){
            if (Input.GetKeyDown(KeyCode.O))
            {
                Anim3_1.SetTrigger("Fracture");
                Anim3_2.SetTrigger("Fracture");
                Anim3_3.SetTrigger("Fracture");
                Anim3_4.SetTrigger("Fracture");
                Anim3_5.SetTrigger("Fracture");
                Destroy(Face3_Trigger.gameObject);
                Destroy(OMesh);
                _O_QTE = false;
            }
        }

    }

    private void EndGame(String loseText, float rate)
    {
        LoseText.text = loseText;
        Overlay.color = new Color(0,0,0, Mathf.Lerp(Overlay.color.a, 1, rate));
        if (Overlay.color.a > 0.99f)
        {
            _control = false;
            LoseText.enabled = true;
        }
    }

//    private void SelectPath()
//    {
//        if (_path1Selected)
//        {
//            Path2.SetActive(false);
//            Lines.Remove(Path2.GetComponent<Collider>());
//        }
//        if(_path2Selected)
//        {
//            Path1.SetActive(false);
//            Lines.Remove(Path1.GetComponent<Collider>());
//        }
//    }
    
    

    private void FixedUpdate()
    {
        if (!_control) return;
        if (_speedControl)
        {
            Rb.MovePosition(transform.position + transform.forward * inputAxis.y * forceMultiplier * 2 + transform.right * inputAxis.x * forceMultiplier);
            Rb.MoveRotation(Rb.rotation * Quaternion.Euler(0, inputMouse.x * mouseSensitivity * 1.3f, 0));
        }
        else
        {
            Rb.MoveRotation(Rb.rotation * Quaternion.Euler(0, inputMouse.x * mouseSensitivity, 0));
            Rb.MovePosition(transform.position + transform.forward * Mathf.Lerp(Rb.velocity.x, 30f, 0.01f) + transform.right * inputAxis.x * Mathf.Lerp(Rb.velocity.y, 7f, 0.03f));
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
            Rb.velocity = new Vector3(Rb.velocity.x, JumpForce, Rb.velocity.z);
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
            Rb.angularVelocity = Vector3.zero;
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
/*            Overlay.color = new Color(0, 0, 0, 0);*/
        }
        
        if (other == SpeedUpTrigger)
        {
            _speedControl = false;
            Debug.Log("SpeedUp");
        }        
        if (other == EndTrigger)
        {
            _speedControl = true;
            if (_mirror == null)
            {
                _mirror = Instantiate(Mirror, new Vector3(transform.position.x + 5, 0.9f, 0), Quaternion.identity);
            }
        }

        if (other == Option1Trigger)
        {
            Option1_1.gameObject.SetActive(false);
            Lines.Remove(Option1_1);
            Lines.Add(Option1_2);
            Option1_2.gameObject.SetActive(true);
            Option2_1.gameObject.SetActive(true);
        }
        
        if (other == Option2Trigger)
        {
            Option2_1.gameObject.SetActive(false);
            Lines.Remove(Option2_1);
            Lines.Add(Option2_2);
            Option2_2.gameObject.SetActive(true);
            Option3_1.gameObject.SetActive(true);
        }
        
        if (other == Option3Trigger)
        {
            Option3_1.gameObject.SetActive(false);
            Lines.Remove(Option3_1);
            Lines.Add(Option3_2);
            Option3_2.gameObject.SetActive(true);
            Option4_1.gameObject.SetActive(true);
        }
        
        if (other == Option4Trigger)
        {
            Option4_1.gameObject.SetActive(false);
            Lines.Remove(Option4_1);
            Lines.Add(Option4_2);
            Option4_2.gameObject.SetActive(true);
            Mask1.SetActive(false);
            Mask2.SetActive(true);
        }

        if (other.CompareTag("Mirror"))
        {
            _endGame = true;
            Mask3.SetActive(false);
            Rb.velocity = Vector3.zero;
            Rb.IsSleeping();
        }   
        
        if (other == ETrigger)
        {
            EMesh.SetActive(true);
            _E_QTE = true;
        }
        
        if (other == GTrigger)
        {
            GMesh.SetActive(true);
            _G_QTE = true;
        }
        
        if (other == OTrigger)
        {
            OMesh.SetActive(true);
            _O_QTE = true;
        }

        if (other == Face1_Trigger || other == Face2_Trigger || other == Face3_Trigger)
        {
            _sank = true;
            Rb.velocity = Vector3.zero;
            Debug.Log("fail");
/*            LoseText.enabled= true;*/
            
        }
        
    }

    
    
    
//
//    private void Fire()
//    {
//        GameObject newBullet = Instantiate(BulletPrefab, transform.position + transform.forward, Quaternion.identity);
//        newBullet.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * 20, ForceMode.Impulse);
//        
//    }


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
            if (transform.position.x - text.transform.position.x <= 50f * Mathf.Clamp(Rb.velocity.x/5, 1, 100) && transform.position.x - text.transform.position.x >= -1f)
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
