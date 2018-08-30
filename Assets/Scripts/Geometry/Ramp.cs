using UnityEngine;

namespace Main.Geometry
{
    public class Ramp : BasicShape
    {
        public Ramp(Vector3 size) : base(size)
        {
            vertLeftTopFront = new Vector3(-size.x, -size.y, size.z);
            vertRightTopFront = new Vector3(size.x, -size.y, size.z);
            vertRightTopBack = new Vector3(size.x, size.y, -size.z);
            vertLeftTopBack = new Vector3(-size.x, size.y, -size.z);
        }
    }
}