using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathProject
{
    public class PathfindingManager : MonoBehaviour
    {
        private List<Agent> agents = new List<Agent>();


        public void FindPaths()
        {
            agents = Manager.buildSystem.FindAllAgentsInScene();

            if (agents.Count == 0)
            {
                Utilities.Message("No agent found, place one in the scene.");
                return;
            }
            else if (agents.Count == 1)
            {
                Utilities.Message("No valid target node found, set one.");
                return;
            }

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            int pathCount = 0;
            for (int a = 0; a < agents.Count; a++)
            {
                List<Agent> targets = new List<Agent>();
                for (int b = a; b < agents.Count; b++)
                {
                    if (agents[a] != agents[b])
                    {
                        targets.Add(agents[b]);
                    }
                }
                agents[a].SetTargetNode(targets);
                pathCount += agents[a].FindPaths().Count;
            }
            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;

            Utilities.Message(pathCount == 0 ? "No valid path found." : "Found " + pathCount + " paths in " + elapsedMs + " ms.");
        }

        public void SmoothPaths()
        {
            foreach (Agent agent in agents)
            {
                agent.SmoothPathLines();
            }
        }

        public void DestroyAllPathLines()
        {
            foreach (Agent agent in agents)
            {
                agent.DestroyPathLines();
            }
        }
    }
}