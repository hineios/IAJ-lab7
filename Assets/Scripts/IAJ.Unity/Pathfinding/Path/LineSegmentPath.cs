using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class LineSegmentPath : LocalPath
    {
        protected Vector3 LineVector { get; set; }
        protected Vector3 NormalizedLineVector { get; set; }

        public LineSegmentPath(Vector3 start, Vector3 end)
        {
            this.StartPosition = start;
            this.EndPosition = end;
            this.LineVector = end - start;
            this.NormalizedLineVector = this.LineVector.normalized;
            this.Length = this.LineVector.magnitude;
            this.StartParam = 0.0f;
            this.EndParam = 1.0f;
        }

        public LineSegmentPath(Vector3 start, Vector3 end, float startParam) : this(start, end)
        {
            this.StartParam = startParam;
            this.EndParam = startParam + this.Length;
        }

        public override Vector3 GetPosition(float param)
        {
            if (param < this.StartParam) return this.StartPosition;
            if (param > this.EndParam) return this.EndPosition;

            return this.StartPosition + this.NormalizedLineVector*(param - this.StartParam);
        }

        public override bool PathEnd(float param)
        {
            return (this.EndParam - param) <= MathConstants.EPSILON;
        }

        public override float GetParam(Vector3 position, float lastParam)
        {
            return this.StartParam + MathHelper.closestParamInLineSegmentToPoint(this.StartPosition, this.EndPosition, position)*this.Length;
        }
    }
}
