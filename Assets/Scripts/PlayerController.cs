using System;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    public float moveSpeed = 5f;
    public InputActionReference horizontalMovementAction;
    public InputActionReference verticalMovementAction;

    private Vector2 _contactPoint;
    private Vector2 _cellCenter;
    private Vector2 _movement;
    private TileBase _tile;
    public Tilemap tilemap; 
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
       _movement.x = horizontalMovementAction.action.ReadValue<float>();
       _movement.y = verticalMovementAction.action.ReadValue<float>();

       List<ContactPoint2D> contacts = new List<ContactPoint2D>();
       _rb.GetContacts(contacts);
       foreach (ContactPoint2D contact in contacts)
       {
           _contactPoint = contact.point;
           _cellCenter = GetTileCenterFromContact(contact);
       }
    }

    private Vector2 GetTileCenterFromContact(ContactPoint2D contact)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(contact.point + (-contact.normal * 0.5f));
        return tilemap.GetCellCenterWorld(cellPosition);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        Vector3 tileCorner = GetTileFromWorldPosition(pos);
        drawTileBounds(tileCorner);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_contactPoint, 0.2f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(_cellCenter, 0.2f);
    }

    private void drawTileBounds(Vector3 tileCorner)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(tileCorner,new Vector3(tilemap.cellSize.x,0,0));
        Gizmos.DrawRay(tileCorner,new Vector3(0,tilemap.cellSize.y,0));
        Gizmos.DrawRay(tileCorner + tilemap.cellSize,new Vector3(-tilemap.cellSize.x,0,0));
        Gizmos.DrawRay(tileCorner + tilemap.cellSize,new Vector3(0,-tilemap.cellSize.y,0));
    }
    private Vector3 GetTileFromWorldPosition(Vector2 worldPosition)
    {
        return tilemap.CellToWorld(tilemap.WorldToCell(worldPosition));  
    }

    private void FixedUpdate()
    {
        
        ContactPoint2D[] contacts = new ContactPoint2D[4];
        int result = Physics2D.GetContacts(_rb, contacts);
        _rb.MovePosition(_rb.position + _movement * (moveSpeed * Time.fixedDeltaTime));
    }
}
