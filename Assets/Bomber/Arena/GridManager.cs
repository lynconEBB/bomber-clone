using Lynck0.Bomberman.Interfaces;
using RealityCollective.ServiceFramework.Services;
using Lynck0.BomberClone;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lynck0.Bomberman
{
    [System.Runtime.InteropServices.Guid("49e469ca-26cd-408e-af91-b86d26461a4a")]
    public class GridManager : BaseServiceWithConstructor, IGridManager
    {
        private Grid _grid;
        private IGridManager _gridManagerImplementation;

        public GridManager(string name, uint priority, GridManagerProfile profile)
            : base(name, priority)
        {
        }
		
        #region MonoBehaviour callbacks
        public override void Initialize()
        {
            // Initialize is called when the Service Framework first instantiates the service.  ( during MonoBehaviour 'Awake')
            // This is called AFTER all services have been registered but before the 'Start' call.
        }

        /// <inheritdoc />
        public override void Start()
        {
            _grid = Object.FindFirstObjectByType<Grid>();
            
            // Start is called when the Service Framework receives the "Start" call on loading of the Scene it is attached to.
            // If "Do Not Destroy" is enabled on the Root Service Profile, this is received only once on startup, Else it will occur for each scene load with a Service Framework Instance.
        }

        public Vector2Int WorldToCell(Vector2 worldPos)
        {
            return (Vector2Int)_grid.WorldToCell(worldPos); 
        }

        public Vector2 CellCenter(Vector2Int cell)
        {
            return _grid.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
        }

        public Vector2Int GetCellHitted(Vector2 origin, Vector2 impact)
        {
            Vector2 dir = impact - origin;
            dir.Normalize();
            return WorldToCell(impact + dir);
        }

        public bool IsEmptyCell(Vector2Int cell)
        {
            Vector2 cellPos = CellCenter(cell);
            Collider2D collider = Physics2D.OverlapCircle(cellPos, 0.2f, layerMask:LayerMask.GetMask("Wall"));
            return collider == null;
        }

        public float GetEmptyNeighbourDistance(Vector2 origin, Vector2 impact, Orientation orientation)
        {
            Vector2Int cellHitted = GetCellHitted(origin, impact);
            Vector2 center = CellCenter(cellHitted);
            
            Vector2Int neighbourOffset = orientation == Orientation.Horizontal
                ? new Vector2Int(0, (int)Mathf.Sign(origin.y - center.y))
                : new Vector2Int((int)Mathf.Sign(origin.x - center.x), 0);
            if (IsEmptyCell(cellHitted + neighbourOffset))
                return orientation == Orientation.Horizontal
                    ? neighbourOffset.y : neighbourOffset.x;

            return 0;
        }

        /// <inheritdoc />
        public override void Reset()
        {
            // Whenever the Service Framework is forcibly "Reset" whilst running, each service will also receive the "Reset" call to request they reinitialize.
        }

        /// <inheritdoc />
        public override void Update()
        {
            // The Unity "Update" MonoBehaviour, this is called when the Service Manager Instance receives the Update Event.
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            // The Unity "LateUpdate" MonoBehaviour, this is called when the Service Manager Instance receives the LateUpdate Event.
        }

        /// <inheritdoc />
        public override void FixedUpdate()
        {
            // The Unity "FixedUpdate" MonoBehaviour, this is called when the Service Manager Instance receives the FixedUpdate Event.
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            // The Unity "Destroy" MonoBehaviour, this is called when the Service Manager Instance receives the Destroy Event.
        }

        /// <inheritdoc />
        public override void OnApplicationFocus(bool isFocused)
        {
            // The Unity "OnApplicationFocus" MonoBehaviour, this is called when Unity generates the OnFocus event on App start or resume.
        }

        /// <inheritdoc />
        public override void OnApplicationPause(bool isPaused)
        {
            // The Unity "OnApplicationPause" MonoBehaviour, this is called when Unity generates the OnPause event on App pauses or is about to suspend.
        }        
        #endregion MonoBehaviour callbacks
    }
}
