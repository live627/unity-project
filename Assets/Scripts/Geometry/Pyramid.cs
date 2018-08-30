using UnityEngine;

namespace Main.Geometry
{
    public class Pyramid : BasicShape
    {
        public Pyramid(Vector3 size) : base(size)
        {
            // z replaces y here.
            p = new Vector3[8]
            {
                new Vector3(0, 0, size.x), // top-left
                new Vector3(size.x, 0, size.x), // top-right
                new Vector3(0, 0, 0), // bottom-left
                new Vector3(size.x, 0, 0), //bottom-right
                // upper level (zx, y=1)
                new Vector3(size.x * 0.5f, size.x, size.x * 0.5f),
                new Vector3(size.x * 0.5f, size.x, size.x * 0.5f),
                new Vector3(size.x * 0.5f, size.x, size.x * 0.5f),
                new Vector3(size.x * 0.5f, size.x, size.x * 0.5f)
            };
        }
    }
}