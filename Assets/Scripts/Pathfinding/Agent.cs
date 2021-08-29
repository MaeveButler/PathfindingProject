using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathProject
{
    public class Agent : MonoBehaviour
    {
        private Pathfinding pathfinding;
        
        private List<Agent> targets;
        private List<LineRenderer> pathLines = new List<LineRenderer>();
        
        public void SetTargetNode(List<Agent> targets)
        {
            this.targets = targets;
        }

        public List<List<PathNode>> FindPaths()
        {
            List<List<PathNode>> allPaths = new List<List<PathNode>>();

            DestroyPathLines();
            for (int i = 0; i < targets.Count; i++)
            {
                pathfinding = new Pathfinding();

                List<PathNode> path = pathfinding.FindPath(transform.position, targets[i].transform.position);
                if (path != null)
                {
                    allPaths.Add(path);

                    LineRenderer pathLine = pathfinding.DrawPath(path);
                    pathLine.SetPosition(0, transform.position);
                    pathLine.SetPosition(pathLine.positionCount - 1, targets[i].transform.position);
                    pathLines.Add(pathLine);

                    string text = GetComponentInChildren<TextMesh>().text + targets[i].GetComponentInChildren<TextMesh>().text + "\n" + path[path.Count - 1].fCost;
                    Vector3 position = pathLine.GetPosition(Mathf.RoundToInt(pathLine.positionCount / 2f));
                    Utilities.CreateWorldText(text, pathLine.transform, position, 15, Utilities.GetBlueColor(), 20);
                }
            }

            return allPaths;
        }

        public void DestroyPathLines()
        {
            for (int i = 0; i < pathLines.Count; i++)
            {
                Destroy(pathLines[i].gameObject);

            }
            pathLines = new List<LineRenderer>();
        }

        public void SmoothPathLines()
        {
            if (pathLines.Count == 0) return;

            foreach (LineRenderer pathLine in pathLines)
            {
                List<Vector2> path = new List<Vector2>();

                for (int i = 0; i < pathLine.positionCount; i++)
                    path.Add(pathLine.GetPosition(i));
                
                pathLine.GetComponent<PathSmoothing>().SmoothPath(path);
            }
        }
    }
}