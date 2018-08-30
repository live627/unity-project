using UnityEngine;

namespace Main.Geometry
{
    public class Cube : BasicShape
    {
        public Cube(Vector3 size) : base(size)
        {
            // z replaces y here.
            p = new Vector3[8]
            {
                new Vector3(0, 0, size.x), // top-left
                new Vector3(size.x, 0, size.x), // top-right
                new Vector3(0, 0, 0), // bottom-left
                new Vector3(size.x, 0, 0), //bottom-right
                new Vector3(size.x, size.x, 0),
                new Vector3(size.x, size.x, size.x),
                new Vector3(0, size.x, 0),
                new Vector3(0, size.x, size.x)
            };
        }
    }
}