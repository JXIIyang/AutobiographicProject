using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    private float _offsetX;
    public GameObject Player;
    
    // Start is called before the first frame update
    void Start()
    {
        _offsetX = (transform.position - Player.transform.position).x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Player.transform.position.x + _offsetX, 0.83f , transform.position.z);
    }
}
