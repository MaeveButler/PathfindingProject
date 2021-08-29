using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathProject
{
    public class PathSmoothing : MonoBehaviour
    {
        private List<Collider2D> obstacles = new List<Collider2D>();

        private List<Vector2> path = new List<Vector2>();
        private List<Vector2> smoothedPath = new List<Vector2>();
        private int startPointIndex;
        private int currentPointIndex;

        EdgeCollider2D col;
        Rigidbody2D rb2d;

        public void SmoothPath(List<Vector2> path)
        {
            if (path.Count <= 1) return;
            
            this.path = path;

            col = gameObject.AddComponent<EdgeCollider2D>();
            rb2d = gameObject.AddComponent<Rigidbody2D>();
            rb2d.isKinematic = true;

            smoothedPath = new List<Vector2> { path[0] };
            startPointIndex = 0;
            currentPointIndex = 2;
            
            StartCoroutine(SmoothPath());
        }

        private IEnumerator SmoothPath()
        {
            List<Vector2> pathSection = new List<Vector2> { path[startPointIndex], path[currentPointIndex] };
            col.SetPoints(pathSection);
            obstacles = new List<Collider2D>();
            yield return Utilities.WaitTimePhysics();
            
            if (obstacles.Count != 0)
            {
                smoothedPath.Add(path[currentPointIndex - 1]);
                startPointIndex = currentPointIndex - 1;
            }

            currentPointIndex++;

            if (currentPointIndex < path.Count)
                StartCoroutine(SmoothPath());
            else
            {
                smoothedPath.Add(path[path.Count - 1]);

                Vector3[] positions = new Vector3[smoothedPath.Count];
                for (int i = 0; i < positions.Length; i++)
                    positions[i] = smoothedPath[i];

                LineRenderer pathLine = GetComponent<LineRenderer>();
                pathLine.positionCount = positions.Length;
                pathLine.SetPositions(positions);

                GameObject text = GetComponentInChildren<TextMesh>().gameObject;
                text.transform.localPosition = pathLine.positionCount == 2 ? (pathLine.GetPosition(0) + pathLine.GetPosition(pathLine.positionCount - 1)) / 2f : pathLine.positionCount == 3 ? pathLine.GetPosition(2) : pathLine.GetPosition(Mathf.RoundToInt(pathLine.positionCount / 2f));

                Destroy(col);
                Destroy(rb2d);
            }
        }


        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.transform.gameObject.layer == 6 && !obstacles.Contains(collision)) //6 = buildobjs layer
            {
                obstacles.Add(collision);
            }
        }
    }
}