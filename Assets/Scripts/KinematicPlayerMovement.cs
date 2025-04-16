using System;
using System.Collections.Generic;
using Lynck0.Bomberman.Interfaces;
using RealityCollective.ServiceFramework.Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lynck0.BomberClone
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None,
    }

    public enum Orientation
    {
        Vertical,
        Horizontal,
    }
    
    public class KinematicPlayerMovement : MonoBehaviour
    {
        private IGridManager _gridManager;
        private IGridManager GridManager => _gridManager ??= ServiceManager.Instance.GetService<IGridManager>();
        
        [SerializeField]
        private InputActionReference horizontalMovementAction;
        [SerializeField]
        private InputActionReference verticalMovementAction;

        [SerializeField]
        private LayerMask filterLayer;
        [SerializeField]
        private float movementSpeed = 0.4f;
        [SerializeField]
        private float skinWidth = 0.1f;
        
        [Header("Debug")]
        [SerializeField]
        private bool debugDraw = true;
        
        private CircleCollider2D _collider;
        private Vector2 _velocity;
        private Vector2 _input;
        
        private Direction _verticalDir = Direction.None;
        private Direction _horizontalDir = Direction.None;
        
        Dictionary<Direction, RaycastHit2D> _hits = new()
        {
            { Direction.Up , new RaycastHit2D()},
            { Direction.Down , new RaycastHit2D()},
            { Direction.Left , new RaycastHit2D()},
            { Direction.Right , new RaycastHit2D()},
        };
        Dictionary<Direction, float> _availableSpaces = new()
        {
            { Direction.Up, 0 },
            { Direction.Down, 0 },
            { Direction.Left, 0 },
            { Direction.Right, 0 }
        };
        private void Awake()
        {
            Application.targetFrameRate = 60;
            _collider = GetComponent<CircleCollider2D>();
        }

        void Update()
        {
            _input = new Vector2(horizontalMovementAction.action.ReadValue<float>(), verticalMovementAction.action.ReadValue<float>()).normalized;
        }

        private void CalculateAvailableSpaces(float max)
        {
            RaycastHit2D hitUp = Physics2D.CircleCast(transform.position, _collider.radius - skinWidth, Vector2.up, max + skinWidth, filterLayer);
            RaycastHit2D hitDown = Physics2D.CircleCast(transform.position, _collider.radius - skinWidth, Vector2.down, max + skinWidth, filterLayer);
            RaycastHit2D hitLeft = Physics2D.CircleCast(transform.position, _collider.radius - skinWidth, Vector2.left, max + skinWidth, filterLayer);
            RaycastHit2D hitRight = Physics2D.CircleCast(transform.position, _collider.radius - skinWidth, Vector2.right, max + skinWidth, filterLayer);
            
            _hits[Direction.Up] = hitUp;
            _hits[Direction.Down] = hitDown;
            _hits[Direction.Left] = hitLeft;
            _hits[Direction.Right] = hitRight;

            _availableSpaces[Direction.Up] = hitUp.collider ?
                    Mathf.Max(max * (hitUp.distance - 2 * skinWidth), 0)
                    : max;
            _availableSpaces[Direction.Down] = hitDown.collider ?
                    Mathf.Max(max * (hitDown.distance - 2 * skinWidth), 0)
                    : max;
            _availableSpaces[Direction.Left] = hitLeft.collider ?
                    Mathf.Max(max * (hitLeft.distance - 2 * skinWidth), 0)
                    : max;
            _availableSpaces[Direction.Right] = hitRight.collider ?
                    Mathf.Max(max * (hitRight.distance - 2 * skinWidth), 0)
                    : max;
        }

        private void CalculateDirections()
        {
            _verticalDir = _velocity.y > 0 ? Direction.Up :
                _velocity.y == 0 ? Direction.None :Direction.Down;
            _horizontalDir = _velocity.x > 0 ? Direction.Right :
                _velocity.x == 0 ? Direction.None :Direction.Left;
        }
        
        private void FixedUpdate()
        {
            _velocity = _input * (movementSpeed * Time.fixedDeltaTime);
            
            CalculateDirections();
            CalculateAvailableSpaces(1);
            
            if (_velocity.magnitude == 0)
                return;
            
            _velocity.x = MoveX(_velocity.x);
            _velocity.y = MoveY(_velocity.y);

            transform.position += (Vector3)_velocity;
        }
        
        private float MoveX(float value)
        {
            if (value == 0)
                return value;

            if (_availableSpaces[_horizontalDir] >= Mathf.Abs(value))
            {
                _availableSpaces[_horizontalDir] -= Mathf.Abs(value);
                return value;
            }

            float newX = Mathf.Sign(value) * _availableSpaces[_horizontalDir] ;
            float newY = Mathf.Sqrt(_velocity.magnitude * _velocity.magnitude - (newX * newX));
            float signY = 0;
            
            if (_verticalDir != Direction.None)
            {
                signY = Mathf.Sign(_velocity.y);
            }
            else if (_hits[_horizontalDir].collider) 
            {
                signY = GridManager.GetEmptyNeighbourDistance(transform.position,_hits[_horizontalDir].point, Orientation.Horizontal);
            }

            if (signY != 0)
            {
                _velocity.y = signY * newY;
                _verticalDir = signY > 0 ? Direction.Up : Direction.Down;
            }
            _availableSpaces[_horizontalDir] = 0;
            return newX;
        }
        
        private float MoveY(float value)
        {
            if (value == 0)
                return value;

            if (_availableSpaces[_verticalDir] >= Mathf.Abs(value))
            {
                _availableSpaces[_verticalDir] -= Mathf.Abs(value);
                return value;
            }

            float newY = Mathf.Sign(value) * _availableSpaces[_verticalDir];
            float newX = Mathf.Sqrt(_velocity.magnitude * _velocity.magnitude - (newY * newY));
            float signX = 0;
            
            if (_horizontalDir != Direction.None)
            {
                signX = Mathf.Sign(_velocity.x);
            }
            else if (_hits[_verticalDir].collider)
            {
                signX = GridManager.GetEmptyNeighbourDistance(transform.position,_hits[_verticalDir].point, Orientation.Vertical);
                _horizontalDir = signX > 0 ? Direction.Right : Direction.Left;
            }
            
            if (signX != 0)
            {
                if (_availableSpaces[_horizontalDir] < Mathf.Abs(newX))
                {
                    _velocity.x = signX * _availableSpaces[_horizontalDir];
                    _availableSpaces[_horizontalDir] = 0;
                }
                else
                {
                    _velocity.x = signX * newX;
                    _availableSpaces[_horizontalDir] -= newX;
                }
            }
            _availableSpaces[_verticalDir] = 0;
            return newY;
        }

        private void OnDrawGizmos()
        {
            if (!debugDraw)
                return;
        }
    }
}