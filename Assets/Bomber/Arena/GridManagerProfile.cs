
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Interfaces;
using UnityEngine;

namespace Lynck0.Bomberman
{
    [CreateAssetMenu(menuName = "GridManagerProfile", fileName = "GridManagerProfile", order = (int)CreateProfileMenuItemIndices.ServiceConfig)]
    public class GridManagerProfile : BaseServiceProfile<IServiceModule>
    { }
}
