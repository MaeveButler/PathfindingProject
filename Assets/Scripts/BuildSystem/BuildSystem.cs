using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathProject
{
    public class BuildSystem : MonoBehaviour
    {
        private GameObject buildObj;
        private BuildPreview buildPreview;

        private bool isBuilding = false;
        private bool snapToGrid = false;
        private bool continueBuilding = false;
        
        private List<BuildPreview> sceneObjs = new List<BuildPreview>();
        
        private void Update()
        {
            if (isBuilding)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    CancelBuild();
                    return;
                }

                if (buildPreview.Transformable())
                {
                    if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.mouseScrollDelta.y != 0)
                    {
                        Vector3 scaleFactor = new Vector3(.1f, .1f) * Input.mouseScrollDelta.y * -1f;   //inverted
                        buildObj.transform.localScale += scaleFactor;
                    }

                    if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.mouseScrollDelta.y != 0)
                    {
                        Vector3 rotationFactor = new Vector3(0f, 0f, 10f) * Input.mouseScrollDelta.y;
                        Vector3 currentRotation = buildObj.transform.localRotation.eulerAngles;
                        Quaternion targetRotation = Quaternion.Euler(currentRotation + rotationFactor);
                        buildObj.transform.localRotation = targetRotation;
                    }
                }
                
                if (buildPreview.SpawnContinously())
                {
                    if (Input.GetMouseButtonDown(0) && buildPreview.CanBuild())
                    {
                        continueBuilding = true;
                        StartCoroutine("WaitForPhysicsBuild");   //only string coroutines can be stopped with StopCoroutine("")
                    }
                    else if (Input.GetMouseButtonUp(0) && continueBuilding)// && buildPreview.SpawnContinously())
                    {
                        continueBuilding = false;
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0) && buildPreview.CanBuild())
                    {
                        if (buildObj.GetComponent<Agent>() != null && FindAllAgentsInScene().Count == 26)   //Max Agents in scene, 26 = 26 letters in the alphabet
                        {
                            Utilities.Message("Max limit of agents in scene.");
                            return;
                        }
                        BuildIt();
                    }
                }
            }
        }

        private IEnumerator WaitForPhysicsBuild()
        {
            if (buildObj == null)
                StopCoroutine("WaitForPhysicsBuild");
            
            if (buildPreview.CanBuild())
            {
                GameObject obj = buildObj;
                BuildIt();
                yield return new WaitForEndOfFrame();
                NewBuild(obj);
            }
            
            yield return Utilities.WaitTimePhysics();

            if (continueBuilding) StartCoroutine(WaitForPhysicsBuild());
            else CancelBuild();
        }

        public void NewBuild(GameObject obj)
        {
            isBuilding = true;

            buildObj = Instantiate(obj, Utilities.GetMouseWorldPosition(), obj.transform.rotation);
            buildObj.name = buildObj.name.Substring(0, buildObj.name.Length - "(Clone)".Length);
            buildPreview = buildObj.GetComponent<BuildPreview>();
            
            Manager.uiManager.UISelectable(false);
            Utilities.Message("Left Click = Cancel Build" + (buildPreview.Transformable() ? "; CTRL + Scroll = Scale Object;\nAlt + Scroll = Rotate Object" : ""));
            Manager.pathfindingManager.DestroyAllPathLines();
        }

        public void CancelBuild()
        {
            Destroy(buildObj);
            buildObj = null;
            buildPreview = null;

            isBuilding = false;

            Manager.uiManager.UISelectable(true);
            Utilities.Message();
        }

        public void BuildIt()
        {
            isBuilding = false;

            buildPreview.Place();
            sceneObjs.Add(buildPreview);
            
            Manager.uiManager.UISelectable(true);
            Utilities.Message();

            FindOverlappingTiles(buildPreview);

            buildObj = null;
            buildPreview = null;
        }

        public List<Agent> FindAllAgentsInScene()
        {
            List<Agent> sceneAgents = new List<Agent>();

            foreach (BuildPreview sceneObj in sceneObjs)
                if (sceneObj.GetComponent<Agent>() != null) sceneAgents.Add(sceneObj.GetComponent<Agent>());

            return sceneAgents;
        }

        public void DestroyAllSceneObjs()
        {
            for (int i = 0; i < sceneObjs.Count; i++)
                Destroy(sceneObjs[i].gameObject);

            Manager.pathfindingManager.DestroyAllPathLines();

            int width = Manager.gridManager.GetGrid().GetWidth(), height = Manager.gridManager.GetGrid().GetHeight();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Manager.gridManager.GetGrid().SetPathNode(x, y, new PathNode(x, y, 0));
                }
            }

            sceneObjs = new List<BuildPreview>();
        }

        public void FindAllOverlappingTiles()
        {
            StopCoroutine("WaitForPhysicsOverlapp");
            StartCoroutine("WaitForPhysicsOverlapp");
        }

        public void SnapToGrid(bool value)
        {
            snapToGrid = value;

            if (snapToGrid)
            {
                foreach (BuildPreview buildObj in sceneObjs)
                {
                    Vector2 pos = buildObj.transform.position;
                    Utilities.GetSnappedPos(Utilities.GetWorldGrid(), pos, out pos);
                    buildObj.transform.position = pos;
                }
                Manager.gridManager.GetGrid().UpdateCosts();
            }
        }

        public bool GetSnapToGrid() { return snapToGrid; }

        private IEnumerator WaitForPhysicsOverlapp()        //Wait for Physics Trigger detection
        {
            yield return Utilities.WaitTimePhysics();
            foreach (BuildPreview obj in sceneObjs)
                FindOverlappingTiles(obj);
        }

        private void FindOverlappingTiles(BuildPreview buildPreview)
        {
            List<Collider2D> collidedGridTiles = buildPreview.CollidedGridTiles();
            foreach (Collider2D gridTile in collidedGridTiles)
            {
                int x, y;
                Utilities.GetXY(gridTile.transform.position, out x, out y);
                
                if (Manager.gridManager.GetGrid().GetPathNode(x, y).nodeCost < buildPreview.NodeCost() || buildPreview.NodeCost() == 0)     //prevent layer clipping except for bridges
                {
                    PathNode node = new PathNode(x, y, buildPreview.NodeCost());
                    Manager.gridManager.GetGrid().SetPathNode(x, y, node);
                }
            }
        }
    }
}