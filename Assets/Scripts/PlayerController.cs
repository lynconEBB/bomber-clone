using System;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;

    public float threshold = 0.1f;
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
        Gizmos.DrawRay(tileCorner, new Vector3(tilemap.cellSize.x, 0, 0));
        Gizmos.DrawRay(tileCorner, new Vector3(0, tilemap.cellSize.y, 0));
        Gizmos.DrawRay(tileCorner + tilemap.cellSize, new Vector3(-tilemap.cellSize.x, 0, 0));
        Gizmos.DrawRay(tileCorner + tilemap.cellSize, new Vector3(0, -tilemap.cellSize.y, 0));
    }

    private Vector3 GetTileFromWorldPosition(Vector2 worldPosition)
    {
        return tilemap.CellToWorld(tilemap.WorldToCell(worldPosition));
    }

    private void AssistMovement()
    {
        if (_movement.magnitude == 0) 
            return;

        if (_movement.magnitude > 1)
        {
            Vector3Int playerCell = tilemap.WorldToCell(_rb.position);
            bool horizontalCollidable = HasCollidableTile(new Vector3Int(playerCell.x, playerCell.y + Mathf.CeilToInt(_movement.y), playerCell.z));
            bool verticalCollidable = HasCollidableTile(new Vector3Int(playerCell.x + Mathf.CeilToInt(_movement.x), playerCell.y, playerCell.z));

            if (horizontalCollidable && !verticalCollidable)
            {
                _movement.x = 0;
            } else if (verticalCollidable && !horizontalCollidable)
            {
                _movement.y = 0;
            }
            
            return;
        }

        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        _rb.GetContacts(contacts);
        foreach (ContactPoint2D contact in contacts)
        {
            Vector3Int tileCell = tilemap.WorldToCell(contact.point + (-contact.normal * 0.5f));
            int xCell = tileCell.x;
            int yCell = tileCell.y;
            _cellCenter = tilemap.GetCellCenterWorld(tileCell);
            bool isVerticalMovement = _movement.y != 0;
            
            float movementComponent = isVerticalMovement ? _movement.y : _movement.x;
            ref float oppositeMovementComponent = ref isVerticalMovement ? ref _movement.x : ref _movement.y;
            ref int oppositeTileCellComponent = ref isVerticalMovement ? ref xCell : ref yCell; 
            
            float contactNormalComponent = isVerticalMovement ? contact.normal.y : contact.normal.x;
            float playerOppositeComponent = isVerticalMovement ? _rb.position.x : _rb.position.y;
            float cellCenterOppositeComponent = isVerticalMovement ? _cellCenter.x : _cellCenter.y;

            bool isBlockingPath = Math.Abs(movementComponent - contactNormalComponent) >= 1;
            if (!isBlockingPath)
                continue;
            
            float oppositeComponentDiff = cellCenterOppositeComponent - playerOppositeComponent;

            if (oppositeComponentDiff > threshold)
            {
                oppositeTileCellComponent--;
                if (!HasCollidableTile(new Vector3Int(xCell, yCell, 0)))
                {
                    oppositeMovementComponent = -1;
                    break; 
                }
            }
            else if (oppositeComponentDiff < -threshold)
            {
                oppositeTileCellComponent++;
                if (!HasCollidableTile(new Vector3Int(xCell, yCell, 0)))
                {
                    oppositeMovementComponent = 1;
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        AssistMovement(); 
        
        _rb.MovePosition(_rb.position + _movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private bool HasCollidableTile(Vector3Int tileCell)
    {
        Collider2D collider = Physics2D.OverlapBox(tilemap.GetCellCenterWorld(tileCell), new Vector2(0.5f, 0.5f), 0);
        return collider != null;
    }
}