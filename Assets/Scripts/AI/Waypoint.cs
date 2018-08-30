using System;
using UnityEngine;

namespace Main.AI
{
    public class Waypoint : AbstractBuildMode
    {
        private GameObject waypointParent = new GameObject("Waypoints");
        private GameObject waypoint;

        public Waypoint(GameObject gameObject, GameObject canvas) : base(gameObject, canvas)
        {
            waypointParent.transform.parent = gameObject.transform;
            keyCode = KeyCode.W;
        }

        protected override void Reset()
        {
            UnityEngine.Object.Destroy(waypoint);
            waypoint = null;
        }

        protected override void Build(Vector3 waypointPosition)
        {
            // reset
            waypoint = null;
        }

        protected override void PreviewBuild(Vector3 waypointPosition)
        {
            if (waypoint == null)
            {
                waypoint = new GameObject(String.Format("Waypoint ({0},{1})", waypointPosition.x, waypointPosition.z))
                {
                    layer = 10
                };
                Renderer lr = waypoint.AddComponent<MeshRenderer>();
                lr.material = Resources.Load("Materials/blue", typeof(Material)) as Material;
                Vector3 size = new Vector3(8, 8, 8);

                MeshFilter mesh_filter = waypoint.AddComponent<MeshFilter>();
                mesh_filter.mesh = new Geometry.Pyramid(size).GenerateMesh();
                waypoint.transform.parent = waypointParent.transform;
            }
            waypoint.transform.position = Main.instance.CellFromWorldPoint(waypointPosition).controlNodes[0].position;
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
    }
}