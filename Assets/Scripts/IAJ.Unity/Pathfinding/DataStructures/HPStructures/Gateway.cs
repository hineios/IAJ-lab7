using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures
{
    public class Gateway : ScriptableObject
    {
        public int id;
        public Vector3 center;
        public Vector3 min { get; set; }
        public Vector3 max { get; set; }
        public List<Cluster> clusters { get; set; }

        public Gateway()
        {
            this.clusters = new List<Cluster>();
        }

        public void Initialize(int id, GameObject gatewayObject)
        {
            this.id = id;
            this.center = gatewayObject.transform.position;
            this.min = new Vector3(this.center.x - gatewayObject.transform.localScale.x * 10 / 2, 0, this.center.z - gatewayObject.transform.localScale.z * 10 / 2);
            this.max = new Vector3(this.center.x + gatewayObject.transform.localScale.x * 10 / 2, 0, this.center.z + gatewayObject.transform.localScale.z * 10 / 2);
        }

        public Vector3 Localize()
        {
            return this.center;
        }
    }
}
