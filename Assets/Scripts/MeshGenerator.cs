using Main.AI.Pathfindidng;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshGenerator
{
    public MeshFilter walls;

    public SquareGrid squareGrid;
    private List<Vector3> vertices;
    private List<int> triangles;
    private Dictionary<int, int> EdgeIndicse = new Dictionary<int, int>();

    public GameObject MiniMap { get; private set; }
    public GameObject Ground;
    public GameObject gameObject;

    public MeshGenerator(GameObject gameObject)
    {
        this.gameObject = gameObject;
        Ground = gameObject.transform.GetChild(0).gameObject;
    }

    void OnDrawGizmos()
    {
        if (squareGrid != null && false)
        {
            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
                    Gizmos.color = (squareGrid.squares[x, y].controlNodes[0].active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].controlNodes[0].position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].controlNodes[1].active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].controlNodes[1].position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].controlNodes[2].active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].controlNodes[2].position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, y].controlNodes[3].active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].controlNodes[3].position, Vector3.one * .4f);


                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(squareGrid.squares[x, y].Nodes[0].position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].Nodes[1].position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].Nodes[2].position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].Nodes[3].position, Vector3.one * .15f);

                }
            }
        }
    }

    public void GenerateMesh(int[,] map, float squareSize, LogHandler logHandler)
    {
        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();
        EdgeIndicse.Clear();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        logHandler.Write("TriangulateSquare");
        Mesh groundMesh = GenerateGroundMesh();
        Ground.GetComponent<MeshFilter>().mesh = groundMesh;
        AddMeshCollider(Ground, groundMesh);
        logHandler.Write("GenerateGroundMesh");
        DrawGrid();

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = GetUv(vertices)
        };
        mesh.RecalculateNormals();

        DrawCubes(squareSize);
        logHandler.Write("DrawCubes");
        // CreateWallMesh(logHandler);
        logHandler.Write("CreateWallMesh");

        // GetComponent<MeshFilter>().mesh = mesh;
        //gameObject.AddComponent<MeshSplit>().Split();
        //logHandler.Write("Split");
        //RenderMiniMap();
        //logHandler.Write("RenderMiniMap");
    }

    private void DrawCubes(float squareSize)
    {
        GameObject cubeParent = new GameObject("Cubes");
        cubeParent.transform.parent = gameObject.transform;
        GameObject myLine;
        Renderer lr;
        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                if (squareGrid.squares[x, y].configuration != 0x0)
                //if(squareGrid.squares[x, y].configuration==0x1111)
                {
                    myLine = new GameObject(String.Format("Cube ({0},{1})", x, y))
                    {
                        layer = 8
                    };
                    myLine.transform.position = squareGrid.squares[x, y].controlNodes[3].position;
                    myLine.transform.parent = cubeParent.transform;

                    Mesh mesh = new Main.Geometry.Cube(new Vector3(squareSize, squareSize, squareSize)).GenerateMesh();
                    myLine.AddComponent<MeshFilter>().mesh = mesh;
                    AddMeshCollider(myLine, mesh);
                    lr = myLine.AddComponent<MeshRenderer>();
                    lr.material = Resources.Load("Materials/NTT_GrandCanyonCliff", typeof(Material)) as Material;
                }
            }
        }
        PathRequestManager.RecreateGrid();
    }

    private void DrawGrid()
    {
        GameObject grid = new GameObject("Grid");
        MeshFilter meshFilter = grid.AddComponent<MeshFilter>();
        var mesh = new Mesh();
        var verticies = new List<Vector3>();
        var indicies = new List<int>();
        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            verticies.Add(new Vector3(squareGrid.squares[x, 0].controlNodes[0].position.x, 0, squareGrid.squares[x, 0].controlNodes[0].position.z));
            verticies.Add(new Vector3(squareGrid.squares[x, 0].controlNodes[0].position.x, 0, squareGrid.squares[x, squareGrid.squares.GetLength(1) - 1].controlNodes[0].position.z));

            indicies.Add(4 * x + 0);
            indicies.Add(4 * x + 1);
        }

        for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
        {
            verticies.Add(new Vector3(squareGrid.squares[0, y].controlNodes[0].position.x, 0, squareGrid.squares[0, y].controlNodes[0].position.z));
            verticies.Add(new Vector3(squareGrid.squares[squareGrid.squares.GetLength(0) - 1, y].controlNodes[0].position.x, 0, squareGrid.squares[0, y].controlNodes[0].position.z));

            indicies.Add(4 * y + 2);
            indicies.Add(4 * y + 3);
        }

        mesh.vertices = verticies.ToArray();
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Lines, 0);
        meshFilter.mesh = mesh;

        MeshRenderer lr = grid.AddComponent<MeshRenderer>();
        lr.material = Resources.Load("Materials/Grid", typeof(Material)) as Material;

        grid.transform.parent = gameObject.transform;
        grid.transform.position = new Vector3(0, 0.01f, 0);
    }

    private Mesh GenerateGroundMesh()
    {
        Mesh mesh = new Mesh
        {
            // make two triangles for the floor.
            vertices = new Vector3[] { squareGrid.squares[0, 0].controlNodes[0].position,
                                        squareGrid.squares[0, squareGrid.squares.GetLength(1)-1].controlNodes[2].position,
                                        squareGrid.squares[squareGrid.squares.GetLength(0)-1, 0].controlNodes[1].position,
                                        squareGrid.squares[squareGrid.squares.GetLength(0)-1, squareGrid.squares.GetLength(1)-1].controlNodes[3].position },
            triangles = new int[] { 0, 1, 2, 2, 1, 3 }
        };
        mesh.uv = GetUv(mesh.vertices.ToList());
        return mesh;
    }

    private Vector2[] GetUv(List<Vector3> verts)
    {
        Vector2[] uvs = new Vector2[verts.Count];
        for (int i = 0; i < verts.Count; i++)
        {
            uvs[i] = new Vector2(verts[i].x * 0.2f, verts[i].z * 0.2f);
        }
        return uvs;
    }

    private Vector2[] GetWallUv(List<Vector3> verts)
    {
        Vector2[] uvs = new Vector2[verts.Count];
        for (int i = 0; i < verts.Count; i++)
        {
            uvs[i] = new Vector2(verts[i].x * 0.2f, verts[i].y * 0.2f);
        }
        return uvs;
    }

    private void RenderMiniMap()
    {
        if (MiniMap == null)
        {
            MiniMap = UnityEngine.Object.Instantiate(gameObject);
            MiniMap.name = "MiniMap";
            MiniMap.SetActive(false);
        }
        return;
        if (MiniMap == null)
        {
            MiniMap = new GameObject("MiniMap");
            MiniMap.AddComponent<MeshFilter>();
            MiniMap.AddComponent<MeshRenderer>();
        }
        MeshFilter meshFilter = MiniMap.GetComponent<MeshFilter>();
        //   meshFilter.mesh = mesh;
        MiniMap.transform.localScale = new Vector3(.2f, .2f, .2f);
        // Camera camera = GetComponent<CameraControls>().m_MainCamera;
        float aspectRatio = Camera.main.aspect; //(width divided by height)
        float camSize = Camera.main.orthographicSize; //The size value mentioned earlier
        float correctPositionX = aspectRatio * camSize - 10;
        MiniMap.transform.position = new Vector3(correctPositionX, camSize, 1);
    }

    void CreateWallMesh(LogHandler logHandler)
    {
        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();

        Mesh wallMesh = new Mesh();

        int current = -1;
        int next = -1;
        while (EdgeIndicse.Count > 0)
        {
            current = next == -1 ? EdgeIndicse.First().Key : next;
            var val = EdgeIndicse[current];
            int startIndex = wallVertices.Count;
            wallVertices.Add(vertices[current]); // left
            wallVertices.Add(vertices[val]); // right
            wallVertices.Add(vertices[current] - Vector3.up * 3); // bottom left
            wallVertices.Add(vertices[val] - Vector3.up * 3); // bottom right

            wallTriangles.Add(startIndex + 0);
            wallTriangles.Add(startIndex + 2);
            wallTriangles.Add(startIndex + 3);

            wallTriangles.Add(startIndex + 3);
            wallTriangles.Add(startIndex + 1);
            wallTriangles.Add(startIndex + 0);

            EdgeIndicse.Remove(current);
            next = EdgeIndicse.ContainsKey(val) ? val : -1;
        }
        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        wallMesh.uv = GetWallUv(wallVertices);
        walls.mesh = wallMesh;
        //  AddMeshCollider(walls);
    }

    private void AddMeshCollider(GameObject gameObject, Mesh mesh)
    {
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = mesh;
    }

    void TriangulateSquare(Square square)
    {
        if (square.configuration == 0x0)
        {
            return;
        }

        List<Node> nodelist = new List<Node>(6);

        for (int i = 0; i < 4; ++i)
        {
            var i_prev = (3 + i) % 4; // i->i_prev => 4->3, 3->2, 2->1, 1->4
            if (!square.controlNodes[i].active)
            {
                continue;
            }

            if (nodelist.Count > 0 && nodelist[nodelist.Count - 1] == square.Nodes[i_prev])
            {
                nodelist.RemoveAt(nodelist.Count - 1);
            }
            else
            {
                nodelist.Add(square.Nodes[i_prev]);
            }

            nodelist.Add(square.controlNodes[i]);
            nodelist.Add(square.Nodes[i]);
        }
        if (nodelist[nodelist.Count - 1] == nodelist[0])
        {
            nodelist.RemoveAt(nodelist.Count - 1);
            nodelist.RemoveAt(0);
        }

        MeshFromPoints(nodelist.ToArray());

        var pair = new int[2] { -1, -1 };
        for (int i = 0; i < 4; ++i)
        {
            if (square.controlNodes[i].active)
            {
                continue;
            }

            if (pair[0] == -1)
            {
                var i_prev = (3 + i) % 4;
                var x = nodelist.IndexOf(square.Nodes[i_prev]);
                pair[0] = x;
            }

            if (pair[1] == -1)
            {
                var x = nodelist.IndexOf(square.Nodes[i]);
                pair[1] = x;
            }

            if (pair[0] != -1 && pair[1] != -1)
            {
                EdgeIndicse.Add(nodelist[pair[0]].vertexIndex, nodelist[pair[1]].vertexIndex);
                pair[0] = -1;
                pair[1] = -1;
            }
        }
        if (pair[0] != pair[1])
        {
            Debug.DebugBreak();
        }

    }

    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);
        if (points.Length < 3)
        {
            throw new ArgumentException("cant generate triangle from less than 3 points");
        }

        if (points.Length > 6)
        {
            Debug.LogError("(points.Length > 6) this is not supposed to happen, but this method could handle it (points.Length = " + points.Length + ")");
        }

        for (int i = 0; i < points.Length - 2; ++i)
        {
            CreateTriangle(points[0], points[i + 1], points[i + 2]);
        }
    }

    void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);
    }

    public class SquareGrid
    {
        public Square[,] squares;
        private int nodeCountX;
        private int nodeCountY;
        private float mapWidth;
        private float mapHeight;

        public SquareGrid(int[,] map, float squareSize)
        {
            nodeCountX = map.GetLength(0);
            nodeCountY = map.GetLength(1);
            mapWidth = nodeCountX * squareSize;
            mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0,
                        -mapHeight / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1],
                        controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }

        public Square CellFromWorldPoint(Vector3 worldPosition)
        {
            return squares[
                Mathf.RoundToInt((nodeCountX - 1) * ((worldPosition.x + mapWidth / 2) / mapWidth)),
                Mathf.RoundToInt((nodeCountY - 1) * ((worldPosition.z + mapHeight / 2) / mapHeight))
            ];
        }
    }

    public class Square
    {
        public ControlNode[] controlNodes = new ControlNode[4]; // order:  topLeft, topRight, bottomRight, bottomLeft;
        public Node[] Nodes = new Node[4]; // order: centreTop, centreRight, centreBottom, centreLeft;

        public int configuration;

        public Square(params ControlNode[] controlNodes)
        {
            if (controlNodes.Length != 4)
            {
                throw new ArgumentException("expected the 4 control nodes: topLeft, topRight, bottomRight, bottomLeft");
            }

            this.controlNodes = controlNodes;
            Nodes[0] = controlNodes[0].right;
            Nodes[1] = controlNodes[2].above;
            Nodes[2] = controlNodes[3].right;
            Nodes[3] = controlNodes[3].above;

            foreach (var node in controlNodes)
            {
                configuration = configuration << 1;
                if (node.active)
                {
                    configuration++;
                }
            }
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos)
        {
            position = _pos;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
        {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize / 2);
            right = new Node(position + Vector3.right * squareSize / 2);
        }
    }
}