using UnityEngine;
using System.Collections.Generic;

namespace Main.AI.Pathfindidng
{
    public class Grid
    {
        private readonly Transform transform;
        private LayerMask unwalkableMask;
        private Vector2 gridWorldSize;
        private float nodeRadius;
        private TerrainType[] walkableRegions;
        private int obstacleProximityPenalty = 10;
        private bool displayGridGizmos= false;
        private Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
        private LayerMask walkableMask;
        private Node[,] grid;
        private float nodeDiameter;
        private int gridSizeX, gridSizeY;
        private int penaltyMin = int.MaxValue;
        private int penaltyMax = int.MinValue;

        public Grid(Transform transform, LayerMask unwalkableMask, Vector2 gridWorldSize, float nodeRadius, TerrainType[] walkableRegions, int obstacleProximityPenalty=10, bool displayGridGizmos=false)
        {
            this.transform = transform;
            this.unwalkableMask = unwalkableMask;
            this.gridWorldSize = gridWorldSize;
            this.nodeRadius = nodeRadius;
            this.walkableRegions = walkableRegions;
            this.obstacleProximityPenalty = obstacleProximityPenalty;
            this.displayGridGizmos = displayGridGizmos;

            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            grid = new Node[gridSizeX, gridSizeY];

            foreach (TerrainType region in walkableRegions)
            {
                walkableMask.value |= region.terrainMask.value;
                walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
            }

            CreateGrid();
        }

        public int MaxSize
        {
            get
            {
                return gridSizeX * gridSizeY;
            }
        }

        public int PenaltyMax
        {
            get
            {
                return penaltyMax;
            }

            set
            {
                penaltyMax = value;
            }
        }

        public void CreateGrid()
        {
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                    int movementPenalty = 0;
                    
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, walkableMask))
                    {
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }

                    if (!walkable)
                    {
                        movementPenalty += obstacleProximityPenalty;
                    }

                    grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }

            BlurPenaltyMap(3);
        }

        private void BlurPenaltyMap(int blurSize)
        {
            int kernelSize = blurSize * 2 + 1;
            int kernelExtents = (kernelSize - 1) / 2;

            int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
            int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
                }

                for (int x = 1; x < gridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                    penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
                }
            }

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                grid[x, 0].movementPenalty = blurredPenalty;

                for (int y = 1; y < gridSizeY; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    grid[x, y].movementPenalty = blurredPenalty;

                    if (blurredPenalty > penaltyMax)
                    {
                        penaltyMax = blurredPenalty;
                    }
                    if (blurredPenalty < penaltyMin)
                    {
                        penaltyMin = blurredPenalty;
                    }
                }
            }
        }

        public Node[] GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours.ToArray();
        }


        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            return grid[
                Mathf.RoundToInt((gridSizeX - 1) * ((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x)), 
                Mathf.RoundToInt((gridSizeY - 1) * ((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y))
            ];
        }

        public void DrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
            if (grid != null && displayGridGizmos)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                    Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
                }
            }
        }
    }
}