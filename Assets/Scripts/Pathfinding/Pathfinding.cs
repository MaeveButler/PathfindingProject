using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathProject
{
    public class Pathfinding
    {
        private Grid grid;
        private PathNode[,] gridCopy;

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private List<PathNode> openList;
        private List<PathNode> closedList;

        public Pathfinding()
        {
            grid = Manager.gridManager.GetGrid();
            gridCopy = grid.CopyGridArray();
        }

        public List<PathNode> FindPath(Vector3 startPosition, Vector3 targetPosition)
        {
            int startX, startY, endX, endY;
            Utilities.GetXY(startPosition, out startX, out startY);
            Utilities.GetXY(targetPosition, out endX, out endY);

            PathNode startNode = GetPathNode(startX, startY);
            PathNode endNode = GetPathNode(endX, endY);

            openList = new List<PathNode> { startNode };
            closedList = new List<PathNode>();

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    PathNode pathNode = GetPathNode(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.cameFromNode = null; //delete former connections
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();


            //A Star Cycle
            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                {
                    //endPath
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                List<PathNode> neighbourNodes = GetNeighbourList(currentNode);
                foreach (PathNode neighbourNode in neighbourNodes)
                {
                    if (closedList.Contains(neighbourNode)) continue;

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode) && neighbourNode.walkable)
                        {
                            openList.Add(neighbourNode);
                        }
                    }

                }
            }
            
            //Out of nodes on the openList
            return null;
        }



        public virtual List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            //Left side
            if (currentNode.x - 1 >= 0)
            {
                //Left
                neighbourList.Add(GetPathNode(currentNode.x - 1, currentNode.y));
                //Left down
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetPathNode(currentNode.x - 1, currentNode.y - 1));
                //Left up
                if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetPathNode(currentNode.x - 1, currentNode.y + 1));
            }
            //Right side
            if (currentNode.x + 1 < grid.GetWidth())
            {
                //Right
                neighbourList.Add(GetPathNode(currentNode.x + 1, currentNode.y));
                //Right down
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetPathNode(currentNode.x + 1, currentNode.y - 1));
                //Right up
                if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetPathNode(currentNode.x + 1, currentNode.y + 1));
            }
            //Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetPathNode(currentNode.x, currentNode.y - 1));
            //Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetPathNode(currentNode.x, currentNode.y + 1));


            return neighbourList;
        }
        private PathNode GetPathNode(int x, int y)
        {
            return gridCopy[x, y];
        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;

            while (currentNode.cameFromNode != null)
            {
                path.Add(currentNode.cameFromNode);
                currentNode = currentNode.cameFromNode;
            }
            path.Reverse();
            return path;
        }

        private int CalculateDistanceCost(PathNode p1, PathNode p2) //the distance while ignoring all blocked areas
        {
            int xDistance = Mathf.Abs(p1.x - p2.x);
            int yDistance = Mathf.Abs(p1.y - p2.y);
            int remaining = Mathf.Abs(xDistance - yDistance);

            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                    lowestFCostNode = pathNodeList[i];
            }
            return lowestFCostNode;
        }
        
        public LineRenderer DrawPath(List<PathNode> path)
        {
            GameObject pathLineObj = GameObject.Instantiate(Resources.Load("PathLine") as GameObject);
            pathLineObj.transform.position = Vector3.zero;
            LineRenderer pathLine = pathLineObj.GetComponent<LineRenderer>();
            Vector3[] positions = new Vector3[path.Count];
            Vector3 cellSizeCorrecture = new Vector2(Manager.gridManager.GetGrid().GetCellSize(), Manager.gridManager.GetGrid().GetCellSize()) * .5f;
            List<Vector2> colPoints = new List<Vector2>();

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = Utilities.GetWorldPosition(path[i].x, path[i].y) + cellSizeCorrecture;
                colPoints.Add(new Vector2(positions[i].x, positions[i].y));
            }
            pathLine.positionCount = positions.Length;
            pathLine.SetPositions(positions);
            
            return pathLine;
        }
    }
}