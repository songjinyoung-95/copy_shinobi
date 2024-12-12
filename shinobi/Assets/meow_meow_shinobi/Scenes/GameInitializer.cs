using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Character;
using Meow_Moew_Shinobi.Lobby;
using Meow_Moew_Shinobi.Singleton;
using Meow_Moew_Shinobi.Util;
using Meow_Moew_Shinobi.Weapon;
using UnityEngine;

namespace Meow_Moew_Shinobi
{
    public class GameInitializer : MonoBehaviour
    {
        private void Start()
        {
            // AbilityHelper.Init();
            VFXManager.Instance.Init();

            LobbyManager.Instance.MoveToLobby();
            CharacterManager.Instance.LoadEquipWeapons();
        }

        #if UNITY_EDITOR
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha2))
                Time.timeScale++;

            if(Input.GetKeyDown(KeyCode.Alpha1))
                Time.timeScale = 1;    

            if(Input.GetKeyDown(KeyCode.Q))
                SkillManager.Instance.ShowSkillView();

            if(Input.GetKeyDown(KeyCode.W))
                SkillManager.Instance.HideSkillView();
        }
        #endif
    }
}