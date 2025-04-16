
using Lynck0.BomberClone;
using RealityCollective.ServiceFramework.Interfaces;
using UnityEngine;

namespace Lynck0.Bomberman.Interfaces
{
    public interface IGridManager : IService
    {
        public Vector2Int WorldToCell(Vector2 worldPos);

        public Vector2 CellCenter(Vector2Int cell);

        public Vector2Int GetCellHitted(Vector2 origin, Vector2 impact);

        public bool IsEmptyCell(Vector2Int cell);

        public float GetEmptyNeighbourDistance(Vector2 origin, Vector2 impact, Orientation orientation);
    }
}