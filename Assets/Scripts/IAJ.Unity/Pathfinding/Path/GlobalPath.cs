using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Utils;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class GlobalPath : Path
    {
        public List<NavigationGraphNode> PathNodes { get; protected set; }
        public List<Vector3> PathPositions { get; protected set; } 
        public bool IsPartial { get; set; }
        public List<LocalPath> LocalPaths { get; protected set; } 


        public GlobalPath()
        {
            this.PathNodes = new List<NavigationGraphNode>();
            this.PathPositions = new List<Vector3>();
            this.LocalPaths = new List<LocalPath>();
        }

        public void CalculateLocalPathsFromPathPositions(Vector3 initialPosition)
        {
            this.Length = 0;
            Vector3 previousPosition = this.PathPositions[0];
            LineSegmentPath lineSegment;
            int init;
            if (this.PathPositions.Count == 1)
            {
                init = 0;
                previousPosition = initialPosition;
            }
            else
                init = 1;
            for (int i = init; i < this.PathPositions.Count; i++)
            {

				if(!previousPosition.Equals(this.PathPositions[i]))
				{
                    lineSegment = new LineSegmentPath(previousPosition, this.PathPositions[i], this.Length);
                    if(lineSegment.Length > 0)
                    {
                        this.LocalPaths.Add(lineSegment);
                        this.Length += lineSegment.Length;
                        previousPosition = this.PathPositions[i];
                    }
				}
            }
        }

        public override float GetParam(Vector3 position, float previousParam)
        {
            foreach(var localPath in this.LocalPaths)
            {
                if(previousParam < localPath.EndParam)
                {
                    return localPath.GetParam(position, previousParam);
                }
            }

            return this.LocalPaths[this.LocalPaths.Count - 1].GetParam(position, previousParam);
        }

        public override Vector3 GetPosition(float param)
        {
            foreach (var localPath in this.LocalPaths)
            {
                if (param < localPath.EndParam)
                {
                    return localPath.GetPosition(param);
                }
            }

            return this.LocalPaths[this.LocalPaths.Count - 1].GetPosition(param);
        }

        public override bool PathEnd(float param)
        {
            return (this.Length - param) <= MathConstants.EPSILON;
        }
    }
}
