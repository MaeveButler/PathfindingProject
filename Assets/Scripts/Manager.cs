using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathProject
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] private GameObject system;

        [Header("UI Variables")]
        [SerializeField] private GameObject uiCanvas;
        [SerializeField] private TMPro.TextMeshProUGUI messageTxt;

        public static GridManager gridManager;
        public static PathfindingManager pathfindingManager;
        public static BuildSystem buildSystem;
        public static UIManager uiManager;

        private void Awake()
        {
            gridManager = system.GetComponentInChildren<GridManager>();
            pathfindingManager = system.GetComponentInChildren<PathfindingManager>();
            buildSystem = system.GetComponentInChildren<BuildSystem>();
            uiManager = system.GetComponentInChildren<UIManager>();

            Utilities.Message();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
        }
    }
}