using System;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class ZeroHeuristic : IHeuristic
    {
        public float H(Vector3 node, Vector3 goalNode)
        {
           return 0;
        }

        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            return 0;
        }
    }
}
