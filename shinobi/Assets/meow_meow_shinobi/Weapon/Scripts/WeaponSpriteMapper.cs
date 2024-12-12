using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon
{
    public static class WeaponSpriteMapper
    {
        private const string PATH = "UI/Weapon/";
        private static Dictionary<EWeaponType, Sprite> _mappingWeaponUiDict = new Dictionary<EWeaponType, Sprite>()
        {
                {EWeaponType.Kunai,         Resources.Load<Sprite>(PATH + EWeaponType.Kunai)},
                {EWeaponType.Fireball,      Resources.Load<Sprite>(PATH + EWeaponType.Fireball)},
                {EWeaponType.IceSpear,      Resources.Load<Sprite>(PATH + EWeaponType.IceSpear)},
                {EWeaponType.Thunderbolt,   Resources.Load<Sprite>(PATH + EWeaponType.Thunderbolt)},
                {EWeaponType.Log,           Resources.Load<Sprite>(PATH + EWeaponType.Log)},
        };

        public static Sprite GetWeaponUISprite(EWeaponType weaponType)
        {
            if(!_mappingWeaponUiDict.ContainsKey(weaponType))
                return null;

            return _mappingWeaponUiDict[weaponType];
        }
    }
}