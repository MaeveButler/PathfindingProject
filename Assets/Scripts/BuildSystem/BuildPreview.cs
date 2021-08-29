using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PathProject
{
    public class BuildPreview : MonoBehaviour
    {
        [SerializeField] private LayerMask rayLayer;
        [SerializeField] private bool spawnContinously;
        [SerializeField] private bool transformable;
        [SerializeField] private List<string> tagsIIgnore = new List<string>();
        [SerializeField] private int nodeCost;

        private SpriteRenderer spriteRend;

        private Color goodColor;
        private Color badColor;

        private bool isInPreview = true;
        private bool canBuild = false;
        private bool snapToGrid;
        
        private float borderRight = -3.11f, borderLeft = 8.89f, borderDown = -3f, borderUp = 5f;    //world borders
        private Vector2[,] worldGrid = new Vector2[0, 0];
        private Vector3 cellSizeVec;

        private List<Collider2D> collidedObjs = new List<Collider2D>();
        private List<Collider2D> collidedGridTiles = new List<Collider2D>();

        private void Awake()
        {
            isInPreview = true;
            
            goodColor = Utilities.GetGreenColor();
            goodColor.a = .5f;
            badColor = Utilities.GetRedColor();
            badColor.a = .5f;

            Vector3 spriteBorders = new Vector3(GetComponent<SpriteRenderer>().bounds.size.x * transform.localScale.x, GetComponent<SpriteRenderer>().bounds.size.y * transform.localScale.y, 1f);

            borderRight += spriteBorders.x / 2f;
            borderLeft -= spriteBorders.x / 2f;
            borderDown += spriteBorders.y / 2f;
            borderUp -= spriteBorders.y / 2f;
            snapToGrid = Manager.buildSystem.GetSnapToGrid();
            
            spriteRend = gameObject.GetComponent<SpriteRenderer>();
            
            if (snapToGrid)
            {
                worldGrid = Utilities.GetWorldGrid();
            }
            
            
            MoveWithinBorders();
            CanNotBuildObj();
        }

        private void FixedUpdate()
        {
            if (isInPreview)
            {
                MoveWithinBorders();
                
                if (collidedObjs.Count == 0)
                    CanBuildObj();
                else
                    CanNotBuildObj();
            }
        }

        public void Place()
        {
            spriteRend.color = Color.white;
            isInPreview = false;
            GetComponent<Collider2D>().isTrigger = true;

            if (GetComponent<Agent>() != null)
            {
                string index = Utilities.GetLetter(Manager.buildSystem.FindAllAgentsInScene().Count);

                Vector3 position = new Vector3(GetComponent<Collider2D>().bounds.size.x / 2f * -1f, 0f);
                TextMesh mesh = Utilities.CreateWorldText(index, transform, position, 15, Utilities.GetBlackColor(), 20);
                mesh.transform.localPosition = new Vector3(mesh.transform.localPosition.x - mesh.GetComponent<MeshRenderer>().bounds.size.x / 2f, mesh.transform.localPosition.y + mesh.GetComponent<MeshRenderer>().bounds.size.y / 2f);
            }
        }


        private void CanBuildObj()
        {
            spriteRend.color = goodColor;
            canBuild = true;
        }
        private void CanNotBuildObj()
        {
            spriteRend.color = badColor;
            canBuild = false;
        }

        private void MoveWithinBorders()
        {
            Vector2 pos = Utilities.GetMouseWorldPosition();

            //Outside of Borders on right side
            if (pos.x < borderRight && pos.y > borderDown && pos.y < borderUp)
            {
                pos = new Vector2(borderRight, pos.y);
            }
            //Outside of Borders on left side
            else if (pos.x > borderLeft && pos.y > borderDown && pos.y < borderUp)
            {
                pos = new Vector2(borderLeft, pos.y);
            }
            //Outside of Borders on down side
            else if (pos.x > borderRight && pos.x < borderLeft && pos.y < borderDown)
            {
                pos = new Vector2(pos.x, borderDown);
            }
            //Outside of Borders on up side
            else if (pos.x > borderRight && pos.x < borderLeft && pos.y > borderUp)
            {
                pos = new Vector2(pos.x, borderUp);
            }
            //Outside of Borders on right/down side
            else if (pos.x < borderRight && pos.y < borderDown)
            {
                pos = new Vector2(borderRight, borderDown);
            }
            //Outside of Borders on right/up side
            else if (pos.x < borderRight && pos.y > borderUp)
            {
                pos = new Vector2(borderRight, borderUp);
            }
            //Outside of Borders on left/down side
            else if (pos.x > borderLeft && pos.y < borderDown)
            {
                pos = new Vector2(borderLeft, borderDown);
            }
            //Outside of Borders on left/up side
            else if (pos.x > borderLeft && pos.y > borderUp)
            {
                pos = new Vector2(borderLeft, borderUp);
            }

            if (snapToGrid) Utilities.GetSnappedPos(worldGrid, pos, out pos);
            transform.position = pos;
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.gameObject.layer == 7) //7 = grid layer
            {
                if (!collidedGridTiles.Contains(collision)) collidedGridTiles.Add(collision);
            }

            if (!isInPreview) return;

            if (rayLayer == (rayLayer | (1 << collision.transform.gameObject.layer)) && !tagsIIgnore.Contains(collision.tag))
            {
                if (!collidedObjs.Contains(collision)) collidedObjs.Add(collision);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            OnTriggerEnter2D(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.gameObject.layer == 7) //7 = grid layer
            {
                collidedGridTiles.Remove(collision);
            }

            if (!isInPreview) return;
            
            if ((collision.transform.gameObject.layer << rayLayer.value) != 0)
            {
                collidedObjs.Remove(collision);
            }
        }

        public bool CanBuild() { return canBuild; }
        public bool SpawnContinously() { return spawnContinously; }
        public bool Transformable() { return transformable; }
        public int NodeCost() { return nodeCost; }
        public List<Collider2D> CollidedGridTiles() { return collidedGridTiles; }
    }
}