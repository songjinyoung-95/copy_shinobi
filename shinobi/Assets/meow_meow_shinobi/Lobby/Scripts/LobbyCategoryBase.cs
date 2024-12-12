using System;
using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Meow_Moew_Shinobi.Lobby
{
    public abstract class LobbyCategoryBase : BehaviourBase
    {
        public ELobbyMenu LobbyType => _lobbyType;


        [SerializeField] private GameObject _panel;
        [SerializeField] private ELobbyMenu _lobbyType;

        protected abstract void OnInit();
        public void Init(Action<ELobbyMenu> selectLobby)
        {
            OnInit();
            Hide();
        }

        protected abstract void OnShow();
        public void Show()
        {
            _panel.SetActive(true);

            Refresh();
        }

        protected abstract void OnHide();
        public void Hide()
        {
            _panel.SetActive(false);
        }

        protected abstract void OnRefrsh();
        public void Refresh()
        {
            OnRefrsh();
        }


#if UNITY_EDITOR
        protected override void OnBindSerialzedField()
        {
            _panel      = gameObject;
        }
#endif
    }
}