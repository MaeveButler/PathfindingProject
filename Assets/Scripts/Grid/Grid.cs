using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PathProject
{
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }
    public class OnGridObjectChangedEventArgs2 : EventArgs
    {
        public Vector3 worldPosition;
    }
    public class Grid
    {
        public event EventHandler OnGridObjectChanged;
        
        private int width, height;
        private float cellSize;
        private Vector3 originPosition;

        private PathNode[,] gridArray;
        
        private GameObject parent;
        private GameObject tileSprite;
        private Color goodColor;
        private Color badColor;
        private bool showGrid;

        private TextMesh[,] debugTextArray = new TextMesh[0, 0];
        private SpriteRenderer[,] tileSpriteArray = new SpriteRenderer[0, 0];

        public Grid(int width, int height, float cellSize, Vector3 originPosition, bool showGrid)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            this.showGrid = showGrid;

            goodColor = Utilities.GetGreenColor();
            goodColor.a = .5f;
            badColor = Utilities.GetRedColor();
            badColor.a = .5f;

            gridArray = new PathNode[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridArray[x, y] = new PathNode(x, y, 0);
                }
            }


            InitilizeGridVisuals();
        }
        
        #region Public Functions
        public void UpdateGrid(int width, int height, float cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            gridArray = new PathNode[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = new PathNode(x, y, 0);
                }
            }

            BuildGrid();
            
        }


        public void UpdateCosts()
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = new PathNode(x, y, 0);
                    debugTextArray[x, y].text = "0";
                    tileSpriteArray[x, y].color = goodColor;
                }
            }
            Manager.buildSystem.FindAllOverlappingTiles();
        }

        public void UpdateGridVisuals(bool showGrid)
        {
            this.showGrid = showGrid;

            foreach (TextMesh debugText in debugTextArray)
                debugText.GetComponent<MeshRenderer>().enabled = showGrid;
            foreach (SpriteRenderer spriteRend in tileSpriteArray)
                spriteRend.enabled = showGrid;
        }

        public void TriggerGridObjectChanged(int x, int y)
        {
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }

        public void SetPathNode(int x, int y, PathNode node)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                gridArray[x, y] = node;
                
                Vector3 tilePos = Utilities.GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
                debugTextArray[x, y].text = node.ToString();
                Color tileColor = gridArray[x, y].nodeCost < 100 ? goodColor : badColor;
                tileColor.a = .5f;
                tileSpriteArray[x, y].color = tileColor;
            }
        }
        public void SetPathNode(Vector3 worldPosition, PathNode node)
        {
            int x, y;
            Utilities.GetXY(worldPosition, out x, out y);
            SetPathNode(x, y, node);
        }

        public PathNode GetPathNode(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
                return gridArray[x, y];
            else
                return default;
        }
        public PathNode GetPathNode(Vector3 worldPosition)
        {
            int x, y;
            Utilities.GetXY(worldPosition, out x, out y);
            return GetPathNode(x, y);
        }

        public PathNode[,] CopyGridArray()
        {
            PathNode[,] copy = new PathNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    copy[x, y] = new PathNode(x, y, gridArray[x, y].nodeCost);
                }
            }

            return copy;
        }

        public int GetWidth() { return width; }
        public int GetHeight() { return height; }
        public float GetCellSize() { return cellSize; }
        public Vector3 GetOriginPosition() { return originPosition; }
        public bool GetShowGrid() { return showGrid; }
        #endregion
        
        #region Grid Visuals
        private void InitilizeGridVisuals()
        {
            //initilize Debug Objects to prevent errors
            debugTextArray = new TextMesh[width, height];
            tileSpriteArray = new SpriteRenderer[width, height];

            parent = new GameObject("Grid");
            tileSprite = Resources.Load<GameObject>("GridTile");

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    NewGridTile(x, y);
                }
            }

            BuildGrid();
        }
        
        private void BuildGrid()
        {
            //copy old values to new array
            TextMesh[,] oldDebugTextArray = debugTextArray;
            SpriteRenderer[,] oldTileSpriteArray = tileSpriteArray;
            debugTextArray = new TextMesh[width, height];
            tileSpriteArray = new SpriteRenderer[width, height];
            for (int x = 0; x < width; x++)
            {
                if (x < oldDebugTextArray.GetLength(0))
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (y < oldDebugTextArray.GetLength(1))
                        {
                            debugTextArray[x, y] = oldDebugTextArray[x, y];
                            tileSpriteArray[x, y] = oldTileSpriteArray[x, y];

                            Vector3 tilePos = (new Vector3(x, y) * cellSize + originPosition) + new Vector3(cellSize, cellSize) * .5f;
                            debugTextArray[x, y].transform.localPosition = tilePos;
                            debugTextArray[x, y].fontSize = Mathf.RoundToInt(cellSize * 15f);
                            debugTextArray[x, y].text = gridArray[x, y].ToString();

                            tileSpriteArray[x, y].transform.localPosition = tilePos;
                            tileSpriteArray[x, y].transform.localScale = new Vector3(cellSize, cellSize, 1);
                            Color tileColor = gridArray[x, y].nodeCost < 100 ? goodColor : badColor;
                            tileColor.a = .5f;
                            tileSpriteArray[x, y].color = tileColor;
                        }
                        else
                            break;
                    }
                }
                else
                    break;
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (debugTextArray[x, y] == null)
                    {
                        NewGridTile(x, y);
                    }
                }
            }

            if (width < oldDebugTextArray.GetLength(0))
            {
                for (int x = width; x < oldDebugTextArray.GetLength(0); x++)
                {
                    for (int y = 0; y < oldDebugTextArray.GetLength(1); y++)
                    {
                        GameObject.Destroy(oldDebugTextArray[x, y].gameObject);
                        GameObject.Destroy(oldTileSpriteArray[x, y].gameObject);
                    }
                }
            }
            if (height < oldDebugTextArray.GetLength(1))
            {
                for (int y = height; y < oldDebugTextArray.GetLength(1); y++)
                {
                    for (int x = 0; x < oldDebugTextArray.GetLength(0); x++)
                    {
                        GameObject.Destroy(oldDebugTextArray[x, y].gameObject);
                        GameObject.Destroy(oldTileSpriteArray[x, y].gameObject);
                    }
                }
            }
        }

        private void NewGridTile(int x, int y)
        {
            Vector3 tilePos = (new Vector3(x, y) * GetCellSize() + GetOriginPosition()) + new Vector3(cellSize, cellSize) * .5f;
            debugTextArray[x, y] = Utilities.CreateWorldText(gridArray[x, y]?.ToString(), parent.transform, tilePos, Mathf.RoundToInt(cellSize * 15f), Utilities.GetBlackColor(), 30);
            debugTextArray[x, y].GetComponent<MeshRenderer>().enabled = showGrid;

            Color tileColor = gridArray[x, y].nodeCost < 100 ? goodColor : badColor;
            tileColor.a = .5f;
            GameObject tileObj = GameObject.Instantiate(tileSprite, parent.transform);
            tileObj.transform.localPosition = tilePos;
            tileObj.transform.localScale = new Vector3(cellSize, cellSize, 1);
            tileSpriteArray[x, y] = tileObj.GetComponent<SpriteRenderer>();
            tileSpriteArray[x, y].color = tileColor;
            tileSpriteArray[x, y].enabled = showGrid;
        }
        #endregion
    }
}
