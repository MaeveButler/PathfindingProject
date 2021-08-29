using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PathProject
{
    public static class Utilities
    {
        public static string GetLetter(int index)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string letter = alphabet.Substring(index, 1);
            return letter;
        }

        public static void Message()
        {
            Message("Press \"Escape\" to quit application.");
        }
        public static void Message(string message)
        {
            Manager.uiManager.GetMessageTxt().text = message;
        }

        public static Vector3 GetWorldPosition(int x, int y)
        {
            Grid grid = Manager.gridManager.GetGrid();
            return new Vector3(x, y) * grid.GetCellSize() + grid.GetOriginPosition();
        }
        public static void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            Grid grid = Manager.gridManager.GetGrid();
            x = Mathf.FloorToInt((worldPosition - grid.GetOriginPosition()).x / grid.GetCellSize());
            y = Mathf.FloorToInt((worldPosition - grid.GetOriginPosition()).y / grid.GetCellSize());
        }
        public static Vector2[,] GetWorldGrid()
        {
            Grid grid = Manager.gridManager.GetGrid();
            Vector2[,] worldGrid = new Vector2[grid.GetWidth(), grid.GetHeight()];
            Vector3 cellSizeVec = new Vector2(grid.GetCellSize(), grid.GetCellSize());

            for (int x = 0; x < worldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < worldGrid.GetLength(1); y++)
                {
                    worldGrid[x, y] = GetWorldPosition(x, y) + cellSizeVec * .5f;
                }
            }
            return worldGrid;
        }

        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vector = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vector.z = 0f;
            return vector;
        }
        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
        
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default, int fontSize = 40, Color color = default, int sortingOrder = 0, TextAnchor textAnchor = TextAnchor.MiddleCenter, TextAlignment textAlignment = TextAlignment.Center)
        {
            if (color == default) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
        }
        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.characterSize = .2f;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }

        public static SpriteRenderer CreateTileSprite(Sprite sprite, Transform parent = null, Vector3 localPosition = default, Vector3 localScale = default, Color color = default, int sortingOrder = 0)
        {
            if (localScale == default) localScale = Vector3.one;
            if (color == default) color = Color.white;
            return CreateTileSprites(parent, sprite, localPosition, localScale, color, sortingOrder);
        }
        public static SpriteRenderer CreateTileSprites(Transform parent, Sprite sprite, Vector3 localPosition, Vector3 localScale, Color color, int sortingOrder)
        {
            GameObject gameObject = new GameObject("Tile", typeof(SpriteRenderer));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            SpriteRenderer sR = gameObject.GetComponent<SpriteRenderer>();
            sR.sprite = sprite;
            sR.color = color;
            sR.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
            return sR;
        }

        public static Color GetGreenColor() //green color from color palette
        {
            return new Color(.63529411764705882352941176470588f, .91764705882352941176470588235294f, .6666666666666667f);
        }
        public static Color GetRedColor()   //red color from color palette
        {
            return new Color(.83137254901960784313725490196078f, .42745098039215686274509803921569f, .56862745098039215686274509803922f);
        }
        public static Color GetBlueColor()  //blue color from color palette
        {
            return new Color(0f, .55294117647058823529411764705882f, .67450980392156862745098039215686f);
        }
        public static Color GetBlackColor() //black color from color palette
        {
            return new Color(0f, .16078431372549019607843137254902f, .1960784313725490196078431372549f);
        }
        
        public static WaitForSeconds WaitTimePhysics()
        {
            WaitForSeconds waitTime = new WaitForSeconds(Time.deltaTime * 2f);  //enough time to look for overlapping Trigger, because OnTrigger gets invoked every fixed update, change fixed delta time to change spawning interval
            return waitTime;
        }

        public static void GetSnappedPos(Vector2[,] worldGrid, Vector2 pos, out Vector2 snappedPos)
        {
            if (worldGrid[0, 0].x > pos.x)
            {
                pos.x = worldGrid[0, 0].x;
            }
            else
            {
                for (int x = 1; x < worldGrid.GetLength(0); x++)
                {
                    if (worldGrid[x - 1, 0].x < pos.x && worldGrid[x, 0].x > pos.x)
                    {
                        if (pos.x - worldGrid[x - 1, 0].x < worldGrid[x, 0].x - pos.x)
                            pos.x = worldGrid[x - 1, 0].x;
                        else
                            pos.x = worldGrid[x, 0].x;

                        break;
                    }
                }

                if (worldGrid[worldGrid.GetLength(0) - 1, 0].x < pos.x)
                {
                    pos.x = worldGrid[worldGrid.GetLength(0) - 1, 0].x;
                }
            }

            if (worldGrid[0, 0].y > pos.y)
            {
                pos.y = worldGrid[0, 0].y;
            }
            else
            {
                for (int y = 1; y < worldGrid.GetLength(1); y++)
                {
                    if (worldGrid[0, y - 1].y < pos.y && worldGrid[0, y].y > pos.y)
                    {
                        if (pos.y - worldGrid[0, y - 1].y < worldGrid[0, y].y - pos.y)
                            pos.y = worldGrid[0, y - 1].y;
                        else
                            pos.y = worldGrid[0, y].y;

                        break;
                    }
                }

                if (worldGrid[0, worldGrid.GetLength(1) - 1].y < pos.y)
                {
                    pos.y = worldGrid[0, worldGrid.GetLength(1) - 1].y;
                }
            }

            snappedPos = pos;
        }
    }
}