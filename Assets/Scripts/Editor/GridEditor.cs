using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PathProject
{
    [CustomEditor(typeof(GridManager))]
    public class GridEditor : Editor
    {
        private GridManager gridManager;
        private SerializedObject getTarget;


        private void OnEnable()
        {
            gridManager = (GridManager)target;
            getTarget = new SerializedObject(gridManager);

        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();


            getTarget.ApplyModifiedProperties();
        }
    }
}