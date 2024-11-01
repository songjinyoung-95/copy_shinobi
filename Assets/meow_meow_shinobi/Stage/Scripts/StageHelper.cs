using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Singleton;
using Meow_Moew_Shinobi.Weapon;
using UnityEngine;

namespace Meow_Moew_Shinobi.Stage
{
    public static class StageHelper
    {
        /// <summary>
        /// 스테이지 맵 크기 가져오기
        /// </summary>
        public static (Vector2 minArea, Vector2 maxArea) StageArea => StageManager.Instance.StageArea;

        /// <summary>
        /// 현재 스테이지에서 장착중인 웨폰 타입 가져오기
        /// </summary>
        public static EWeaponType[] EquipWeapons => WeaponManager.Instance.EquipWeapons;

        
    }
}