using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Creotly.FizzFazz
{
    public class GameplayManager : MonoBehaviour
    {
        [Header("---------- Inspector Variables ----------")]
        [SerializeField] private SpriteRenderer clickHighlight;
        [SerializeField] private SpriteRenderer boardPrefab;
        [SerializeField] private SpriteRenderer bgCellPrefab;
        [SerializeField] private Node _nodePrefab;
        public List<Color> NodeColors;

        [Header("---------- Events ----------")]
        public UnityEvent onColoursJoined;
        public UnityEvent onColoursSeparated;

        public static GameplayManager Instance { get; private set; }
        public Dictionary<Vector2Int, Node> _nodeGrid { get; private set; }
        public bool GameFinished { get; private set; }

        Dictionary<int, HashSet<string>> _connectedEndPairs = new Dictionary<int, HashSet<string>>();

        LevelData CurrentLevelData;
        List<Node> _nodes;
        Node _startNode;

        private void Awake()
        {
            // ----- Singleton -----
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;

        }

        void Start()
        {
            // ----- Setup -----
            CurrentLevelData = GamesManager.Instance.GetLevel();

            SpawnBoard();
            SpawnNodes();
        }
        
        private void Update()
        {
            if (GameFinished) 
                return;

            if (Input.GetMouseButtonDown(0))
            {
                _startNode = null;
                return;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (_startNode == null)
                {
                    if (hit && hit.collider.gameObject.TryGetComponent(out Node tNode) && tNode.IsClickable)
                    {
                        _startNode = tNode;
                        clickHighlight.gameObject.SetActive(true);
                        clickHighlight.gameObject.transform.position = (Vector3)mousePos2D;
                        clickHighlight.color = GetHighLightColor(tNode.colorId);
                    }

                    return;
                }

                clickHighlight.gameObject.transform.position = (Vector3)mousePos2D;

                if (hit && hit.collider.gameObject.TryGetComponent(out Node tempNode) && _startNode != tempNode)
                {
                    if (_startNode.colorId != tempNode.colorId && tempNode.IsEndNode)
                    {
                        return;
                    }

                    _startNode.UpdateInput(tempNode);
                    CheckWin();
                    _startNode = null;
                }

                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _startNode = null;
                clickHighlight.gameObject.SetActive(false);
            }

        }

        private void SpawnBoard()
        {
            int currentLevelSize = GamesManager.Instance.CurrentLevel + 4;
            var board = Instantiate(boardPrefab, new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, 0f), Quaternion.identity, this.transform);
            board.size = new Vector2(currentLevelSize + 0.08f, currentLevelSize + 0.08f);

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    Instantiate(bgCellPrefab, new Vector3(i + 0.5f, j + 0.5f, 0f), Quaternion.identity, this.transform);
                }
            }

            Camera.main.orthographicSize = currentLevelSize + 2f;
            Camera.main.transform.position = new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, -10f);

            clickHighlight.size = new Vector2(currentLevelSize / 4f, currentLevelSize / 4f); clickHighlight.transform.position = Vector3.zero; 
            clickHighlight.gameObject.SetActive(false);
        }

        private void SpawnNodes()
        {
            _nodes = new List<Node>();
            _nodeGrid = new Dictionary<Vector2Int, Node>();

            int currentLevelSize = GamesManager.Instance.CurrentLevel + 4;
            Node spawnedNode;
            Vector3 spawnPos;

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    spawnPos = new Vector3(i + 0.5f, j + 0.5f, 0f);
                    spawnedNode = Instantiate(_nodePrefab, spawnPos, Quaternion.identity, this.transform);
                    spawnedNode.Init();

                    int colorIdForSpawnedNode = GetColorId(i, j);

                    if (colorIdForSpawnedNode != -1)
                    {
                        spawnedNode.SetColorForPoint(colorIdForSpawnedNode);
                    }

                    _nodes.Add(spawnedNode);
                    _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                    spawnedNode.gameObject.name = i.ToString() + j.ToString();
                    spawnedNode.Pos2D = new Vector2Int(i, j);

                }
            }

            SetupNodeEdges();
        }

        private void SetupNodeEdges()
        {
            int currentLevelSize = GamesManager.Instance.CurrentLevel + 4;

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    Node node = _nodeGrid[new Vector2Int(i, j)];

                    // for each 4-neighbour direction, register the neighbor
                    Vector2Int up = new Vector2Int(0, 1);
                    Vector2Int down = new Vector2Int(0, -1);
                    Vector2Int left = new Vector2Int(-1, 0);
                    Vector2Int right = new Vector2Int(1, 0);

                    if (_nodeGrid.TryGetValue(new Vector2Int(i, j) + up, out Node nUp))
                        node.SetEdge(up, nUp);
                    if (_nodeGrid.TryGetValue(new Vector2Int(i, j) + down, out Node nDown))
                        node.SetEdge(down, nDown);
                    if (_nodeGrid.TryGetValue(new Vector2Int(i, j) + left, out Node nLeft))
                        node.SetEdge(left, nLeft);
                    if (_nodeGrid.TryGetValue(new Vector2Int(i, j) + right, out Node nRight))
                        node.SetEdge(right, nRight);
                }
            }
        }

        public Color GetHighLightColor(int colorID)
        {
            Color result = NodeColors[colorID % NodeColors.Count];
            result.a = 0.4f;
            return result;
        }

        public int GetColorId(int i, int j)
        {
            List<Edge> edges = CurrentLevelData.Edges;
            Vector2Int point = new Vector2Int(i, j);

            for (int colorId = 0; colorId < edges.Count; colorId++)
            {
                if (edges[colorId].StartPoint == point ||
                    edges[colorId].EndPoint == point)
                {
                    return colorId;
                }
            }

            return -1;
        }

        private void CheckWin()
        {
            bool IsWinning = true;

            foreach (var item in _nodes)
            {
                item.SolveHighlight();
            }

            foreach (var item in _nodes)
            {
                IsWinning &= item.IsWin;
                if (!IsWinning)
                {
                    return;
                }
            }

            clickHighlight.gameObject.SetActive(false);

            GameFinished = true;
        }

        public void OnColorsJoined(Node a, Node b)
        {
            onColoursJoined?.Invoke();
            //Debug.Log($"[Colors Joined] {a.name} {a.Pos2D} <--> {b.name} {b.Pos2D} (Color {a.colorId})");
        }

        public void OnColorsSeparated(Node a, Node b)
        {
            onColoursSeparated.Invoke();
            //Debug.Log($"[Colors Separated] {a.name} {a.Pos2D} X {b.name} {b.Pos2D} (Color {a.colorId})");
        }

        private string PairKey(Node a, Node b)
        {
            int idA = a.GetInstanceID();
            int idB = b.GetInstanceID();
            if (idA < idB) return $"{idA}_{idB}";
            return $"{idB}_{idA}";
        }

        public bool AreNodesConnected(Node start, Node target)
        {
            if (start == null || target == null) return false;
            if (start == target) return true;
            if (start.colorId != target.colorId) return false;

            var visited = new HashSet<Node>();
            var q = new Queue<Node>();
            visited.Add(start);
            q.Enqueue(start);

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                if (cur == target) return true;

                foreach (var nb in cur._ConnectedNodes)
                {
                    if (nb == null) continue;
                    if (nb.colorId != start.colorId) continue; // Only traverse same color nodes
                    if (visited.Contains(nb)) continue;
                    visited.Add(nb);
                    q.Enqueue(nb);
                }
            }

            return false;
        }

        public void RecalculateColorConnections(int colorId)
        {
            // Get all end nodes of this color
            var endNodes = new List<Node>();
            foreach (var n in _nodes)
            {
                if (n != null && n.IsEndNode && n.colorId == colorId)
                    endNodes.Add(n);
            }

            var currentPairs = new HashSet<string>();

            for (int i = 0; i < endNodes.Count; i++)
            {
                for (int j = i + 1; j < endNodes.Count; j++)
                {
                    var a = endNodes[i];
                    var b = endNodes[j];
                    if (AreNodesConnected(a, b))
                    {
                        currentPairs.Add(PairKey(a, b));
                    }
                }
            }

            if (!_connectedEndPairs.TryGetValue(colorId, out var previousPairs))
            {
                previousPairs = new HashSet<string>();
            }

            // Newly connected pairs
            foreach (var p in currentPairs)
            {
                if (!previousPairs.Contains(p))
                {
                    // Find the nodes from key and notify OnColorsJoined
                    var parts = p.Split('_');
                    int idA = int.Parse(parts[0]);
                    int idB = int.Parse(parts[1]);
                    Node a = FindNodeByInstanceId(idA);
                    Node b = FindNodeByInstanceId(idB);
                    if (a != null && b != null)
                        OnColorsJoined(a, b);
                }
            }

            // Pairs that were connected before but are now separated
            foreach (var p in previousPairs)
            {
                if (!currentPairs.Contains(p))
                {
                    var parts = p.Split('_');
                    int idA = int.Parse(parts[0]);
                    int idB = int.Parse(parts[1]);
                    Node a = FindNodeByInstanceId(idA);
                    Node b = FindNodeByInstanceId(idB);
                    if (a != null && b != null)
                        OnColorsSeparated(a, b);
                }
            }

            // Store current snapshot
            _connectedEndPairs[colorId] = currentPairs;
        }

        private Node FindNodeByInstanceId(int instanceId)
        {
            foreach (var n in _nodes)
            {
                if (n == null) continue;
                if (n.GetInstanceID() == instanceId) return n;
            }
            return null;
        }

    }
}
