using RAIN.Navigation.Graph;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Utils;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures
{
    public class ClusterGraph : ScriptableObject
    {
        public List<Cluster> clusters;
        public List<Gateway> gateways;
        public GatewayDistanceTableRow[] gatewayDistanceTable;
        public Cluster[] nodesCluster;

        public ClusterGraph()
        {
            this.clusters = new List<Cluster>();
            this.gateways = new List<Gateway>();
        }

        public Cluster Quantize(NavigationGraphNode node)
        {
            return nodesCluster[node.NodeIndex];
        }

        public Cluster Quantize(Vector3 node)
        {
            foreach(var c in clusters)
            {
                if (MathHelper.PointInsideBoundingBox(node, c.min, c.max))
                    return c;
            }
            return null;
        }

        public void SaveToAssetDatabase()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (System.IO.Path.GetExtension(path) != "")
            {
                path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(ClusterGraph).Name.ToString() + ".asset");

            AssetDatabase.CreateAsset(this, assetPathAndName);
            EditorUtility.SetDirty(this);
            
            //save the clusters
            foreach(var cluster in this.clusters)
            {
                AssetDatabase.AddObjectToAsset(cluster, assetPathAndName);
            }

            //save the gateways
            foreach (var gateway in this.gateways)
            {
                AssetDatabase.AddObjectToAsset(gateway, assetPathAndName);
            }

            //save the gatewayTableRows and tableEntries
            foreach(var tableRow in this.gatewayDistanceTable)
            {
                AssetDatabase.AddObjectToAsset(tableRow, assetPathAndName);
                foreach(var tableEntry in tableRow.entries)
                {
                    AssetDatabase.AddObjectToAsset(tableEntry, assetPathAndName);
                }
            }

            //save the clusters for each node
            foreach(var c in this.nodesCluster)
            {
                AssetDatabase.AddObjectToAsset(c, assetPathAndName);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = this;
        }
    }
}
