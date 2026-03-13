using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private readonly Collider2D[] dragHitBuffer = new Collider2D[16];
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
        EnsureGoalSetup();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && draggingObj == null)
        {
            TryBeginDrag();
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

    private void TryBeginDrag()
    {
        int hitCount = Physics2D.OverlapPointNonAlloc(MousePos, dragHitBuffer);
        if (hitCount <= 0)
            return;

        DraggableObj candidate = null;
        int bestSortingOrder = int.MinValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = dragHitBuffer[i];
            if (hitCollider == null)
                continue;

            DraggableObj draggable = hitCollider.GetComponentInParent<DraggableObj>();
            if (draggable == null || !draggable.enabled)
                continue;

            int sortingOrder = GetSortingOrder(hitCollider);
            if (candidate == null || sortingOrder >= bestSortingOrder)
            {
                candidate = draggable;
                bestSortingOrder = sortingOrder;
            }
        }

        if (candidate == null)
            return;

        draggingObj = candidate;
        dragOffset = MousePos - (Vector2)draggingObj.transform.position;
    }

    private static void EnsureGoalSetup()
    {
        GameObject goal = GameObject.Find("Goal");
        if (goal == null)
            return;

        if (goal.GetComponent<SpriteRenderer>() == null)
            return;

        if (goal.GetComponent<GoalGlow>() == null)
            goal.AddComponent<GoalGlow>();
    }

    private static int GetSortingOrder(Collider2D hitCollider)
    {
        SpriteRenderer spriteRenderer = hitCollider.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = hitCollider.GetComponentInParent<SpriteRenderer>();

        return spriteRenderer != null ? spriteRenderer.sortingOrder : 0;
    }
}
