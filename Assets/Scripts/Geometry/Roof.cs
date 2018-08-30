using UnityEngine;

namespace Main.Geometry
{
    public class Roof : BasicShape
    {
        public Roof(Vector3 size) : base(size)
        {
            vertLeftTopFront = new Vector3(-size.x, size.y, 0);
            vertRightTopFront = new Vector3(size.x, size.y, 0);
            vertRightTopBack = new Vector3(size.x, size.y, 0);
            vertLeftTopBack = new Vector3(-size.x, size.y, 0);
        }
    }
}