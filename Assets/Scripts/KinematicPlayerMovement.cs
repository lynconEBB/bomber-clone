using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class KinematicPlayerMovement : MonoBehaviour
    {
        public InputActionReference horizontalMovementAction;
        public InputActionReference verticalMovementAction;
        public Vector2 velocity;

        [SerializeField]
        private LayerMask filterLayer;
        
        private ContactFilter2D filter;
        private BoxCollider2D _collider;

        private void Awake()
        {
            filter = new ContactFilter2D();
            filter.layerMask = filterLayer;
            filter.useLayerMask = true;
            
            _collider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            velocity = new Vector2(horizontalMovementAction.action.ReadValue<float>(), verticalMovementAction.action.ReadValue<float>());
        }

        private void FixedUpdate()
        {
            List<RaycastHit2D> hits = new();
            
            int count = Physics2D.BoxCast(transform.position, _collider.size, 0f, velocity,filter, hits);
            if (count > 0)
            {
                 
            }
            else
            {
                transform.position += new Vector3(velocity.x, velocity.y, 0);
            }
        }

        private void OnDrawGizmos()
        {
            
        }
    }
}