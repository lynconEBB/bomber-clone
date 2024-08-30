using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    public InputActionReference movementAction;
    public float moveSpeed = 5f;
    private Vector2 _movement;
    private bool _hitWallLastFrame = false;
    private CircleCollider2D _collider;
    public Tilemap tilemap; 
    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
    }

    private void Update()
    {
       _movement = movementAction.action.ReadValue<Vector2>();
       
       ContactFilter2D contactFilter = new ContactFilter2D();
       contactFilter.NoFilter();
       List<RaycastHit2D> hits = new();
       _collider.Cast(_movement, contactFilter, hits, 1);
       
       Debug.Log(hits.Count);
       foreach (RaycastHit2D hit in hits)
       { 
           Debug.DrawLine(_rb.position, hit.point, Color.magenta,Time.deltaTime, false);
           Vector3Int worldToCell = tilemap.WorldToCell(hit.point);
           tilemap.GetCellCenterWorld(worldToCell);
       }
    }

    private void OnDrawGizmos()
    {
        if (_rb != null)
        {
            //Debug.DrawRay(_rb.position, _movement , Color.red, 0.1f, false);
        }
    }

    private void FixedUpdate()
    {
        if (_movement.x > 0)
            _movement.x = 1;
        else if (_movement.x < 0)
            _movement.x = -1;
        if (_movement.y > 0)
            _movement.y = 1;
        else if (_movement.y < 0)
            _movement.y = -1;
        
        ContactPoint2D[] contacts = new ContactPoint2D[4];
        int result = Physics2D.GetContacts(_rb, contacts);
        _rb.MovePosition(_rb.position + _movement * (moveSpeed * Time.fixedDeltaTime));
    }
}
