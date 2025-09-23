using System.Collections.Generic;
using UnityEngine;

namespace Creotly.FizzFazz
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private GameObject point;
        [SerializeField] private GameObject topEdge;
        [SerializeField] private GameObject bottomEdge;
        [SerializeField] private GameObject leftEdge;
        [SerializeField] private GameObject rightEdge;
        [SerializeField] private GameObject highLight;

        Dictionary<Node, GameObject> ConnectedEdges;

        public int colorId { get; private set; }

        public bool IsWin
        {
            get
            {
                if (point.activeSelf)
                {
                    return _ConnectedNodes.Count == 1;
                }

                return _ConnectedNodes.Count == 2;
            }
        }

        public bool IsClickable
        {
            get
            {
                if (point.activeSelf)
                {
                    return true;
                }

                return _ConnectedNodes.Count > 0;
            }
        }

        public bool IsEndNode
        {
            get
            {
                return point.activeSelf;
            }
        }

        public List<Node> _ConnectedNodes { get; private set; }
        [HideInInspector] public Vector2Int Pos2D;

        SpriteRenderer highLightSprite;
        List<Vector2Int> directionCheck = new List<Vector2Int>() { Vector2Int.up,Vector2Int.left,Vector2Int.down,Vector2Int.right };

        private void Start()
        {
            highLightSprite = highLight.GetComponent<SpriteRenderer>();
        }

        public void Init()
        {
            point.SetActive(false);
            topEdge.SetActive(false);
            bottomEdge.SetActive(false);
            leftEdge.SetActive(false);
            rightEdge.SetActive(false);
            highLight.SetActive(false);

            ConnectedEdges = new Dictionary<Node, GameObject>();
            _ConnectedNodes = new List<Node>();
        }

        public void SetColorForPoint(int colorIdForSpawnedNode)
        {
            colorId = colorIdForSpawnedNode;
            point.SetActive(true);
            point.GetComponent<SpriteRenderer>().color = GameplayManager.Instance.NodeColors[colorId % GameplayManager.Instance.NodeColors.Count];
        }

        public void SetEdge(Vector2Int offset, Node node)
        {
            if (offset == Vector2Int.up)
            {
                ConnectedEdges[node] = topEdge;
                return;
            }

            if (offset == Vector2Int.down)
            {
                ConnectedEdges[node] = bottomEdge;
                return;
            }

            if (offset == Vector2Int.right)
            {
                ConnectedEdges[node] = rightEdge;
                return;
            }

            if (offset == Vector2Int.left)
            {
                ConnectedEdges[node] = leftEdge;
                return;
            }
        }

        public void UpdateInput(Node connectedNode)
        {
            //Invalid Input
            if (!ConnectedEdges.ContainsKey(connectedNode))
            {
                return;
            }

            // Connected Node already exists so delete the edge and connected parts
            if (_ConnectedNodes.Contains(connectedNode))
            {
                _ConnectedNodes.Remove(connectedNode);
                connectedNode._ConnectedNodes.Remove(this);

                // Remove the visual/structural edge 
                RemoveEdge(connectedNode);

                // Remove any dangling node chains starting from each end
                DeleteNode();
                connectedNode.DeleteNode();

                return;
            }

            //Start Node has 2 Edges
            if (_ConnectedNodes.Count == 2)
            {
                Node tempNode = _ConnectedNodes[0];

                if (!tempNode.IsConnectedToEndNode())
                {
                    _ConnectedNodes.Remove(tempNode);
                    tempNode._ConnectedNodes.Remove(this);
                    RemoveEdge(tempNode);
                    tempNode.DeleteNode();
                }
                else
                {
                    tempNode = _ConnectedNodes[1];
                    _ConnectedNodes.Remove(tempNode);
                    tempNode._ConnectedNodes.Remove(this);
                    RemoveEdge(tempNode);
                    tempNode.DeleteNode();
                }
            }

            //End Node has 2 Edges
            if (connectedNode._ConnectedNodes.Count == 2)
            {
                Node tempNode = connectedNode._ConnectedNodes[0];
                connectedNode._ConnectedNodes.Remove(tempNode);
                tempNode._ConnectedNodes.Remove(connectedNode);
                connectedNode.RemoveEdge(tempNode);
                tempNode.DeleteNode();

                tempNode = connectedNode._ConnectedNodes[0];
                connectedNode._ConnectedNodes.Remove(tempNode);
                tempNode._ConnectedNodes.Remove(connectedNode);
                connectedNode.RemoveEdge(tempNode);
                tempNode.DeleteNode();
            }

            //Start Node is Different Color and connected Node Has 1 Edge
            if (connectedNode._ConnectedNodes.Count == 1 && connectedNode.colorId != colorId)
            {
                Node tempNode = connectedNode._ConnectedNodes[0];
                connectedNode._ConnectedNodes.Remove(tempNode);
                tempNode._ConnectedNodes.Remove(connectedNode);
                connectedNode.RemoveEdge(tempNode);
                tempNode.DeleteNode();
            }

            //Starting is Edge Node and has 1 Edge already
            if (_ConnectedNodes.Count == 1 && IsEndNode)
            {
                Node tempNode = _ConnectedNodes[0];
                _ConnectedNodes.Remove(tempNode);
                tempNode._ConnectedNodes.Remove(this);
                RemoveEdge(tempNode);
                tempNode.DeleteNode();
            }

            //ConnectedNode is EdgeNode and has 1 Edge already
            if (connectedNode._ConnectedNodes.Count == 1 && connectedNode.IsEndNode)
            {
                Node tempNode = connectedNode._ConnectedNodes[0];
                connectedNode._ConnectedNodes.Remove(tempNode);
                tempNode._ConnectedNodes.Remove(connectedNode);
                connectedNode.RemoveEdge(tempNode);
                tempNode.DeleteNode();
            }

            AddEdge(connectedNode);

            //Dont allow Boxes
            if (colorId != connectedNode.colorId)
            {
                return;
            }

            List<Node> checkingNodes = new List<Node>() { this };
            List<Node> resultNodes = new List<Node>() { this };

            while (checkingNodes.Count > 0)
            {
                foreach (var item in checkingNodes[0]._ConnectedNodes)
                {
                    if (!resultNodes.Contains(item))
                    {
                        resultNodes.Add(item);
                        checkingNodes.Add(item);
                    }
                }

                checkingNodes.Remove(checkingNodes[0]);
            }

            foreach (var item in resultNodes)
            {
                if (!item.IsEndNode && item.IsDegreeThree(resultNodes))
                {
                    Node tempNode = item._ConnectedNodes[0];
                    item._ConnectedNodes.Remove(tempNode);
                    tempNode._ConnectedNodes.Remove(item);
                    item.RemoveEdge(tempNode);
                    tempNode.DeleteNode();

                    if (item._ConnectedNodes.Count == 0) return;

                    tempNode = item._ConnectedNodes[0];
                    item._ConnectedNodes.Remove(tempNode);
                    tempNode._ConnectedNodes.Remove(item);
                    item.RemoveEdge(tempNode);
                    tempNode.DeleteNode();

                    return;
                }
            }

        }

        private void AddEdge(Node connectedNode)
        {
            connectedNode.colorId = colorId;
            connectedNode._ConnectedNodes.Add(this);
            _ConnectedNodes.Add(connectedNode);
            GameObject connectedEdge = ConnectedEdges[connectedNode];
            connectedEdge.SetActive(true);
            connectedEdge.GetComponent<SpriteRenderer>().color = GameplayManager.Instance.NodeColors[colorId % GameplayManager.Instance.NodeColors.Count];

            // Recalculate connectivity for this color (calls OnColorsJoined for any newly connected end-node pairs)
            GameplayManager.Instance.RecalculateColorConnections(colorId);
        }

        private void RemoveEdge(Node node)
        {
            GameObject edge = ConnectedEdges[node];
            edge.SetActive(false);
            edge = node.ConnectedEdges[this];
            edge.SetActive(false);

            // Recalculate connectivity for the color of the node(s)
            // use the color of this node (they should match for connected same color graph)
            // but to be safe recalc for both colors (they might differ during removals)
            if (colorId >= 0)
                GameplayManager.Instance.RecalculateColorConnections(colorId);

            if (node.colorId >= 0 && node.colorId != colorId)
                GameplayManager.Instance.RecalculateColorConnections(node.colorId);
        }

        private void DeleteNode()
        {
            Node startNode = this;

            if (startNode.IsConnectedToEndNode())
            {
                return;
            }

            while (startNode != null)
            {
                Node tempNode = null;
                if (startNode._ConnectedNodes.Count != 0)
                {
                    tempNode = startNode._ConnectedNodes[0];
                    startNode._ConnectedNodes.Clear();
                    tempNode._ConnectedNodes.Remove(startNode);
                    startNode.RemoveEdge(tempNode);
                }
                startNode = tempNode;
            }
        }

        public bool IsConnectedToEndNode(List<Node> checkedNode = null)
        {
            if (checkedNode == null)
            {
                checkedNode = new List<Node>();
            }

            if (IsEndNode)
            {
                return true;
            }

            foreach (var item in _ConnectedNodes)
            {
                if (!checkedNode.Contains(item))
                {
                    checkedNode.Add(item);
                    return item.IsConnectedToEndNode(checkedNode);
                }
            }

            return false;
        }

        public void SolveHighlight()
        {
            if (_ConnectedNodes.Count == 0)
            {
                highLight.SetActive(false);
                return;
            }

            List<Node> checkingNodes = new List<Node>() { this };
            List<Node> resultNodes = new List<Node>() { this };

            while (checkingNodes.Count > 0)
            {
                foreach (var item in checkingNodes[0]._ConnectedNodes)
                {
                    if (!resultNodes.Contains(item))
                    {
                        resultNodes.Add(item);
                        checkingNodes.Add(item);
                    }
                }

                checkingNodes.Remove(checkingNodes[0]);
            }

            checkingNodes.Clear();

            foreach (var item in resultNodes)
            {
                if (item.IsEndNode)
                {
                    checkingNodes.Add(item);
                }
            }

            if (checkingNodes.Count == 2)
            {
                highLight.SetActive(true);
                highLightSprite.color = GameplayManager.Instance.GetHighLightColor(colorId);
            }
            else
            {
                highLight.SetActive(false);
            }

        }

        public bool IsDegreeThree(List<Node> resultNodes)
        {
            bool isdegreethree = false;

            int numOfNeighbours = 0;

            for (int i = 0; i < directionCheck.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2Int checkingPos = Pos2D + directionCheck[(i + j) % directionCheck.Count];

                    if (GameplayManager.Instance._nodeGrid.TryGetValue(checkingPos, out Node result))
                    {
                        if (resultNodes.Contains(result))
                        {
                            numOfNeighbours++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (numOfNeighbours == 3)
                {
                    break;
                }

                numOfNeighbours = 0;
            }

            if (numOfNeighbours >= 3)
            {
                isdegreethree = true;
            }

            return isdegreethree;
        }


    }
}
