using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    class DictionaryNodeList : IClosedSet
    {
        private Dictionary<int, NodeRecord> NodeRecords { get; set; }

        public DictionaryNodeList()
        {
            NodeRecords = new Dictionary<int, NodeRecord>();
        }
        public void AddToClosed(NodeRecord nodeRecord)
        {
            NodeRecords.Add(nodeRecord.GetHashCode(), nodeRecord);
        }

        public ICollection<NodeRecord> All()
        {
            return this.NodeRecords.Values;
        }

        public void Initialize()
        {
            NodeRecords.Clear();
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            NodeRecords.Remove(nodeRecord.GetHashCode());
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            NodeRecords.TryGetValue(nodeRecord.GetHashCode(), out nodeRecord);
            return nodeRecord;
        }

        public int CountOpen()
        {
            return NodeRecords.Count;
        }
    }
}
