using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PathProject
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject uiCanvas;
        [SerializeField] private TMPro.TextMeshProUGUI messageTxt;

        private Selectable[] uiBtns;
        
        public void Awake()
        {
            uiBtns = uiCanvas.GetComponentsInChildren<Selectable>();
        }

        public void UISelectable(bool value)
        {
            foreach (Selectable btn in uiBtns)
                btn.interactable = value;
        }

        public TMPro.TextMeshProUGUI GetMessageTxt() { return messageTxt; }
    }
}