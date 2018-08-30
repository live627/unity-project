using System;
using UnityEngine;

namespace Main.AI
{
    public class Road : AbstractBuildMode
    {
        private GameObject roadParent = new GameObject("Roads");
        private Vector3 roadEnd;
        private GameObject road;

        public Road(GameObject gameObject, GameObject canvas) : base(gameObject, canvas)
        {
            roadParent.transform.parent = gameObject.transform;
            keyCode = KeyCode.R;

            // Add callback for our custom MonoBehavior.Update() event.
            //Main.Updating += OnUpdateAgain;
        }

        //private void OnUpdateAgain()
        //{
        //    // Check if the mouse is not over a UI element.
        //    if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && isBuilding)
        //    {
        //        if (Input.GetMouseButton(0))
        //        {
        //            RaycastHit hitInfo;
        //            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        //            {
        //                roadEnd = hitInfo.point;
        //            }
        //        }
        //    }
        //}

        protected override void Reset()
        {
            UnityEngine.Object.Destroy(road);
            road = null;
        }

        protected override void Build(Vector3 roadPosition)
        {
            // reset
            road = null;
        }

        private Vector3[] GetArcCoords(float startAngle)
        {
            float endAngle = startAngle + 90;
            int segments = 10;
            float radius = 0.5f;
            float angle = startAngle;
            float arcLength = endAngle - startAngle;
            Vector3[] arcPoints = new Vector3[segments];
            for (int i = 0; i < segments; i++)
            {
                float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                arcPoints[i] = new Vector3(x, 0, y);

                angle += arcLength / segments;
            }

            return arcPoints;
        }

        protected override void PreviewBuild(Vector3 roadPosition)
        {
            Create(roadPosition);
            road.transform.position = Main.instance.CellFromWorldPoint(roadPosition).controlNodes[0].position + new Vector3(0, 0.01f, 0);
            //road.transform.rotation = Quaternion.FromToRotation(Vector3.right, roadEnd - roadPosition);
        }

        /// <summary>
        /// Actually creates the GameObject
        /// </summary>
        /// <param name="roadPosition">world podition</param>
        protected void Create(Vector3 roadPosition)
        {
            if (road == null)
            {
                road = new GameObject(String.Format("Road ({0},{1})", roadPosition.x, roadPosition.z))
                {
                    layer = 10
                };
                Renderer lr = road.AddComponent<MeshRenderer>();
                lr.material = Resources.Load("Materials/Road", typeof(Material)) as Material;

                roadEnd = roadPosition;
                float width = 8;
                float length = Math.Max(8, Vector3.Distance(roadPosition, roadEnd));
                Vector3[] vertices = {
                    new Vector3(0, 0, width), // top-left
                    new Vector3(length, 0, width), // top-right
                    Vector3.zero, // bottom-left
                    new Vector3(length, 0, 0) // bottom-right
                };

                Vector2[] uvs = new Vector2[4]
                {
                    new Vector2(0, 1), // top-left
                    new Vector2(1, 1), // top-right
                    new Vector2(0, 0), // bottom-left
                    new Vector2(1, 0) // bottom-right
                };

                Mesh mesh = new Mesh
                {
                    vertices = vertices,
                    triangles = new int[6] { 0, 1, 2, 1, 3, 2 },
                    uv = uvs
                };
                mesh.RecalculateNormals();

                MeshFilter mesh_filter = road.AddComponent<MeshFilter>();
                mesh_filter.mesh = mesh;
                road.transform.parent = roadParent.transform;
            }
        }
    }
}