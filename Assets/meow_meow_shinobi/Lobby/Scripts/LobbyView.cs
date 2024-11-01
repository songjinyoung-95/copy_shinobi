using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Meow_Moew_Shinobi.Lobby
{
    public interface ILobbyView
    {
        void Init();
        void Show(ELobbyMenu menu);
        void Hide();
    }

    public delegate void MoveToStage(int stageLevel);

    public class LobbyView : BehaviourBase, ILobbyView
    {
        [SerializeField] private LobbyCategoryBase[]    _lobbies;
        [SerializeField] private GameObject             _parent;
        [SerializeField] private Button[]               _category_BTNS;

        private Dictionary<ELobbyMenu, LobbyCategoryBase> _categoryDict;
        private ELobbyMenu _lastSelectMenu;
        


        public event MoveToStage OnMoveToStage;

        public void Init()
        {
            _categoryDict = new Dictionary<ELobbyMenu, LobbyCategoryBase>();
            
            foreach (var lobby in _lobbies)
            {
                ELobbyMenu lobbyType = lobby.LobbyType;

                if (_categoryDict.ContainsKey(lobbyType))
                {
                    Debug.LogError($"{lobbyType} : 중복된 카테고리가 존재합니다");
                    return;
                }
                
                _categoryDict.Add(lobbyType, lobby);
                lobby.Init(Show);
            }
            
            _lastSelectMenu = ELobbyMenu.Unknown;

            MoveToSelectStage();
            CategoryButtonAddListner();
            
            DontDestroyOnLoad(gameObject);
        }

        public void Show(ELobbyMenu menu)
        {
            if(menu == _lastSelectMenu)
            {
                _categoryDict[_lastSelectMenu].Refresh();
                return;
            }

            if(_lastSelectMenu != ELobbyMenu.Unknown)
                _categoryDict[_lastSelectMenu]?.Hide();

            _categoryDict[menu].Show();

            _lastSelectMenu = menu;
            
            _parent.SetActive(true);
        }

        public void Hide()
        {
            _parent.SetActive(false);

            _categoryDict[_lastSelectMenu].Hide();
            _lastSelectMenu = ELobbyMenu.Unknown;
        }


        private void CategoryButtonAddListner()
        {
            int index = 0;
            foreach (var item in _categoryDict)
            {
                _category_BTNS[index].onClick.AddListener(() => Show(item.Value.LobbyType));
                index++;
            }
        }



        private void MoveToSelectStage()
        {
            MissionCategory missionCategory = _categoryDict[ELobbyMenu.Mission] as MissionCategory;

            if(missionCategory == null)
            {
                Debug.LogError("null");
                return;
            }

            missionCategory.AddLinster_SelectStage(() => OnMoveToStage?.Invoke(missionCategory.SelectStageLevel));
        }





#if UNITY_EDITOR
        protected override void OnBindSerialzedField()
        {
            _parent = gameObject;

            Transform category  = transform.Find("RTF_Bottom").Find("Category");
            _category_BTNS      = new Button[category.childCount];

            for (int i = 0; i < _category_BTNS.Length; i++)
                _category_BTNS[i] = category.GetChild(i).GetComponent<Button>();


            Transform lobby = transform.Find("RTF_Mid");
            _lobbies = new LobbyCategoryBase[lobby.childCount];

            for (int i = 0; i < _lobbies.Length; i++)
                _lobbies[i] = lobby.GetChild(i).GetComponent<LobbyCategoryBase>();            
        }
#endif
    }
}

