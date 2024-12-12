using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.UI
{
    public class UIPoolCanvas : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        public void Init()
        {
             _canvas.worldCamera = Camera.main;
             DontDestroyOnLoad(gameObject);
        }
    }
}