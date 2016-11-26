using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures
{
    public class GatewayDistanceTableEntry : ScriptableObject
    {
        public Vector3 startGatewayPosition;
        public Vector3 endGatewayPosition;
        public float shortestDistance;
    }
}
