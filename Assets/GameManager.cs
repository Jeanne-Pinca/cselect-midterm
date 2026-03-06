using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    Camera cam;
    DraggableObj draggingObj;
    Vector2 dragOffset;

    public Vector2 MousePos
    {
        get { return cam.ScreenToWorldPoint(Input.mousePosition); }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && draggingObj == null)
        {
            RaycastHit2D _hit = Physics2D.Raycast(MousePos, Vector2.zero);
            if (_hit.collider != null)
            {
                if (_hit.collider.GetComponent<DraggableObj>() != null)
                {
                    draggingObj = _hit.collider.GetComponent<DraggableObj>();
                    dragOffset = MousePos - (Vector2)draggingObj.transform.position;
                }
            }
        }

        if (draggingObj != null)
        {
            draggingObj.transform.position = MousePos - dragOffset;

            if (draggingObj.canRotate)
            {
                const float rotSpeed = 180;
                if (Input.GetKey(KeyCode.LeftArrow))
                    draggingObj.transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);
                else if (Input.GetKey(KeyCode.RightArrow))
                    draggingObj.transform.Rotate(Vector3.forward * -rotSpeed * Time.deltaTime);
            }


            if (!Input.GetMouseButton(0))
                draggingObj = null;
        }
    }
}
