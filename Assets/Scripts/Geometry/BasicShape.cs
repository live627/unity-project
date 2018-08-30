using UnityEngine;

namespace Main.Geometry
{
    public abstract class BasicShape
    {
        protected Vector3 vertLeftTopFront;
        protected Vector3 vertRightTopFront;
        protected Vector3 vertRightTopBack;
        protected Vector3 vertLeftTopBack;
        protected Vector3 size;
        protected Vector3[] p;

        protected BasicShape(Vector3 size)
        {
            this.size = size;
        }

        public Mesh GenerateMesh()
        {
            Vector3[] vertices = new Vector3[24]
            {
			    // face 1 (xy p[lane], z=0)
			    p[2],
                p[3],
                p[4],
p[6], 
			    // face 2 (zy p[lane], x=1)
			    p[3],
                p[1],
                p[5],
                p[4], 
			    // face 3 (xy p[lane], z=1)
			    p[1],
                p[0],
                p[7],
                p[5], 
			    // face 4 (zy p[lane], x=0)
			    p[0],
                p[2],
p[6],
                p[7],
			    // face 5 (zx p[lane], y=1)
 p[6],
                p[4],
                p[5],
                p[7], 
			    // face 6 (zx p[lane], y=0)
			    p[2],
                p[0],
                p[1],
                p[3],
            };

            // Each face has four vertices and two triangles.
            int faces = 6;

            int[] triangles = new int[faces * 6];
            Vector2[] uvs = new Vector2[faces * 4];

            for (int i = 0; i < faces; i++)
            {
                int triangleOffset = i * 4;
                int triangleIndexOffset = i * 6;
                int uvOffset = i * 4;
                //TODO change order here to go 0 1 2  3 2 1 ?
                triangles[triangleIndexOffset + 0] = 0 + triangleOffset;
                triangles[triangleIndexOffset + 1] = 2 + triangleOffset;
                triangles[triangleIndexOffset + 2] = 1 + triangleOffset;

                triangles[triangleIndexOffset + 3] = 0 + triangleOffset;
                triangles[triangleIndexOffset + 4] = 3 + triangleOffset;
                triangles[triangleIndexOffset + 5] = 2 + triangleOffset;

                // same uvs for all faces
                //BUG the UVs might be going counter-clockwise (wrong) while the triangles go clockwise (correct).
                uvs[uvOffset + 0] = new Vector2(0, 0);
                uvs[uvOffset + 1] = new Vector2(1, 0);
                uvs[uvOffset + 2] = new Vector2(1, 1);
                uvs[uvOffset + 3] = new Vector2(0, 1);
            }

            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}