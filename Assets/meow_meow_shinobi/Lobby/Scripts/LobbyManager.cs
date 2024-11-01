using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meow_Moew_Shinobi.Lobby;

namespace Meow_Moew_Shinobi.Singleton
{
    public class LobbyManager
    {
        private const string VIEW_PATH = "UI/Lobby/LobbyView";

        private static LobbyManager _instance;
        public static LobbyManager  Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new LobbyManager();

                return _instance;
            }
        }

        private LobbyView _view;

        public LobbyManager()
        {
            LobbyView view = Resources.Load<LobbyView>(VIEW_PATH);

            if(view == null)
            {
                Debug.LogError($"{VIEW_PATH} 에 프리펩이 없습니다");
                return;                
            }

            _view = Object.Instantiate(view);

            _view.OnMoveToStage += MoveToStage;

            _view.Init();
        }

        public void MoveToLobby()
        {
            _view.Show(ELobbyMenu.Mission);
        }

        private void MoveToStage(int level)
        {
            _view.Hide();
            StageManager.Instance.SetStage(level);
        }
    }
}