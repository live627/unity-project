using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using System;

namespace Main.AI.Pathfindidng
{
    public class PathRequestManager : MonoBehaviour
    {
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;
        public TerrainType[] walkableRegions;
        public int obstacleProximityPenalty = 10;
        public bool displayGridGizmos = false;
        private static Queue q = Queue.Synchronized(new Queue());
        private Queue<PathResult> results = new Queue<PathResult>();
        public static PathRequestManager instance;
        private Pathfinding pathfinding;
        private Grid grid;
        private GameObject selectionPrefab;

        public static event Action<Unit> FinishedTraversingPath;

        void Awake()
        {
            instance = this;
            grid = new Grid(transform, unwalkableMask, gridWorldSize, nodeRadius, walkableRegions, obstacleProximityPenalty, displayGridGizmos);
            pathfinding = new Pathfinding(grid);

            Thread t = new Thread(() =>
            {
                while (true)
                {
                    if (q.Count > 0)
                    {
                        instance.pathfinding.FindPath((PathRequest)q.Dequeue(), instance.FinishedProcessingPath);
                    }
                    Thread.Sleep(1);
                }
            })
            {
                IsBackground = true
            };

            t.Start();
        }

        void Update()
        {
            while (results.Count > 0)
            {
                PathResult result = results.Dequeue();
                result.callback(result.path, result.success);
            }
        }

        public static void RecreateGrid()
        {
            // instance.grid.CreateGrid();
        }

        public static Node GetNodeFromWorldPoint(Vector3 worldPosition)
        {
            return instance.grid.NodeFromWorldPoint(worldPosition);
        }

        public static void RequestPath(PathRequest request)
        {
            q.Enqueue(request);
        }

        public void FinishedProcessingPath(PathResult result)
        {
            results.Enqueue(result);
        }

        void OnDrawGizmos()
        {
            if (grid != null)
            {
                grid.DrawGizmos();
            }
        }

        public void SelectUnit(GameObject selectedObject)
        {
            if (selectionPrefab == null)
            {
                selectionPrefab = Resources.Load<GameObject>("Prefabs/UI Selection Indicator");
                selectionPrefab.SetActive(false);
            }

            if (selectedObject != null)
            {
                Material[] materials =selectedObject.GetComponent<Renderer>().materials;
                selectedObject.GetComponent<Renderer>().materials = new List<Material>(materials)
                {
                    Resources.Load<Material>("Materials/Outline")
                }.ToArray();
                //GameObject selection = Instantiate(selectionPrefab, canvas.transform);

                //if (selection != null)
                //{
                //    selection.SetActive(true);

                //    Rect visualRect = RendererBoundsInScreenSpace(selectedObject.GetComponent<Renderer>());
                //    RectTransform rt = selection.GetComponent<RectTransform>();
                //    rt.position = new Vector2(visualRect.xMin, visualRect.yMin);
                //    rt.sizeDelta = new Vector2(visualRect.width, visualRect.height);
                //}
                //else
                //{
                //    selection.SetActive(false);
                //}
            }
        }

        private Rect RendererBoundsInScreenSpace(Renderer r)
        {
            // This is the space occupied by the object's visuals
            // in WORLD space.
            Bounds bigBounds = r.bounds;

            Vector3[] screenSpaceCorners = new Vector3[8];

            Camera theCamera = Camera.main;

            // For each of the 8 corners of our renderer's world space bounding box,
            // convert those corners into screen space.
            screenSpaceCorners[0] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[1] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[2] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[3] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[4] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[5] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
            screenSpaceCorners[6] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
            screenSpaceCorners[7] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));

            // Now find the min/max X & Y of these screen space corners.
            float minX = screenSpaceCorners[0].x;
            float minY = screenSpaceCorners[0].y;
            float maxX = screenSpaceCorners[0].x;
            float maxY = screenSpaceCorners[0].y;

            for (int i = 1; i < 8; i++)
            {
                minX = Math.Min(screenSpaceCorners[i].x, minX);
                minY = Math.Min(screenSpaceCorners[i].y, minX);
                maxX = Math.Max(screenSpaceCorners[i].x, maxX);
                maxY = Math.Max(screenSpaceCorners[i].y, maxY);
            }

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }
    }
}