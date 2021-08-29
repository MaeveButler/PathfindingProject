using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PathProject
{
    public class GridManager : MonoBehaviour
    {
        
        [SerializeField] private Vector3 originPosition;
        [SerializeField] private int inputTileCost;

        

        private Grid grid;

        private int width = MIN_WIDTH * 2, height = MIN_HEIGHT * 2;
        private float cellSize = MAX_CELLSIZE / 2;

        private PathNode overrideTile;
        
        public Slider cellSizeSlider;

        private static int MIN_WIDTH = 3;       //ratio:
        private static int MIN_HEIGHT = 2;      //3 : 2
        private static float MAX_CELLSIZE = 4;
        private static int MAX_SCALINGFACTOR = 10;

        private void Start()
        {
            grid = new Grid(width, height, cellSize, originPosition, false);

            InitilizeUI();
        }
        
        private void SetPathNodeInGrid(int cost)
        {
            Vector3 mousePosition = Utilities.GetMouseWorldPosition();
            int x, y;
            Utilities.GetXY(mousePosition, out x, out y);
            PathNode node = new PathNode(x, y, cost);
            grid.SetPathNode(mousePosition, node);
        }

        private void MyEventHandler(object sender, EventArgs e)
        {
            if (e is OnGridObjectChangedEventArgs)
            {
                int x = ((OnGridObjectChangedEventArgs)e).x;
            }
            if (e is OnGridObjectChangedEventArgs2)
            {
                Vector3 worldPosition = ((OnGridObjectChangedEventArgs2)e).worldPosition;
            }
        }

        
        private void InitilizeUI()
        {
            cellSizeSlider.maxValue = MAX_SCALINGFACTOR;
            cellSizeSlider.minValue = 1;
            cellSizeSlider.wholeNumbers = true;
            cellSizeSlider.value = MAX_SCALINGFACTOR / 2;
        }

        public void ChangeGrid()
        {
            int sizeFactor = MAX_SCALINGFACTOR - Mathf.RoundToInt(cellSizeSlider.value) + 1;
            
            cellSize = MAX_CELLSIZE / sizeFactor;
            width = sizeFactor * MIN_WIDTH;
            height = sizeFactor * MIN_HEIGHT;
            
            grid.UpdateGrid(width, height, cellSize);
            Manager.buildSystem.FindAllOverlappingTiles();
        }
        
        

        public void ToggleGrid(bool value)
        {
            grid.UpdateGridVisuals(value);
        }

        public Grid GetGrid() { return grid; }
    }
}