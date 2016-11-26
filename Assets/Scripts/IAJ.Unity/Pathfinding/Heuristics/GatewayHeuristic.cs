using RAIN.Navigation.Graph;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures;
using UnityEngine;
using System;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class GatewayHeuristic : IHeuristic
    {
        private ClusterGraph ClusterGraph { get; set; }

        public GatewayHeuristic(ClusterGraph clusterGraph)
        {
            this.ClusterGraph = clusterGraph;
        }

        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            //for now just returns the euclidean distance
            Cluster StartCluster = ClusterGraph.Quantize(node);
            Cluster GoalCluster = ClusterGraph.Quantize(goalNode);
            if (object.ReferenceEquals(null, StartCluster) || object.ReferenceEquals(null, GoalCluster) || StartCluster.Equals(GoalCluster))
            {
                return EuclideanDistance(node.LocalPosition, goalNode.LocalPosition);
            }else
            {
                var startPos = node.LocalPosition;
                var goalPos = goalNode.LocalPosition;
                float min = float.MaxValue;
                foreach (Gateway g1 in StartCluster.gateways)
                {
                    var first = EuclideanDistance(startPos, g1.center);
                    foreach (Gateway g2 in GoalCluster.gateways)
                    {
                        float h =
                            first +
                            ClusterGraph.gatewayDistanceTable[g1.id].entries[g2.id].shortestDistance +
                            EuclideanDistance(g2.center, goalPos);
                        if (h < min)
                            min = h;
                    }
                }
                return min;
            }
        }

        public float EuclideanDistance(Vector3 startPosition, Vector3 endPosition)
        {
            return (endPosition - startPosition).magnitude;
        }

        public float H(Vector3 startPos, Vector3 goalPos)
        {
            Cluster StartCluster = ClusterGraph.Quantize(startPos);
            Cluster GoalCluster = ClusterGraph.Quantize(goalPos);
            if (object.ReferenceEquals(null, StartCluster) || object.ReferenceEquals(null, GoalCluster) || StartCluster.Equals(GoalCluster))
            {
                return EuclideanDistance(startPos, goalPos);
            }
            else
            {
                float min = float.MaxValue;
                foreach (Gateway g1 in StartCluster.gateways)
                {
                    var first = EuclideanDistance(startPos, g1.center);
                    foreach (Gateway g2 in GoalCluster.gateways)
                    {
                        float h =
                            first +
                            ClusterGraph.gatewayDistanceTable[g1.id].entries[g2.id].shortestDistance +
                            EuclideanDistance(g2.center, goalPos);
                        if (h < min)
                            min = h;
                    }
                }
                return min;
            }
        }
    }
}
