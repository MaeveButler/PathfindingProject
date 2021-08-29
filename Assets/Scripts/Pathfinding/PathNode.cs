using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathProject
{
    public class PathNode
    {
        public int x, y;

        public int nodeCost;
        public int gCost, hCost, fCost;

        public bool walkable = true;
        
        public PathNode cameFromNode;

        public PathNode(int x, int y, int nodeCost)
        {
            this.x = x;
            this.y = y;
            this.nodeCost = nodeCost;

            walkable = nodeCost == 100 ? false : true;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost + nodeCost;
        }

        public override string ToString()
        {
            return nodeCost.ToString();
        }
        
        public List<PathNode> GetNeighbourList()
        {
            List<PathNode> neighbourList = new List<PathNode>();


            return neighbourList;
        }
    }
}