using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Weapon;
using UnityEngine;

namespace Meow_Moew_Shinobi.Singleton
{
    public class CharacterManager
    {
        private static CharacterManager _instance;
        public static CharacterManager  Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new CharacterManager();

                return _instance;
            }
        }

        public List<EWeaponType> EquipWeapons { get; private set; }
        public EWeaponType EquippedWeapon { get; private set; }



        public CharacterManager()
        {
            EquipWeapons    = new List<EWeaponType>();
            EquippedWeapon  = EWeaponType.Kunai;
        }

        public void LoadEquipWeapons()
        {
            EquipWeapons.Add(EWeaponType.Kunai);
            EquipWeapons.Add(EWeaponType.Fireball);
            EquipWeapons.Add(EWeaponType.IceSpear);
            EquipWeapons.Add(EWeaponType.Thunderbolt);
            EquipWeapons.Add(EWeaponType.Log);
        }
    }
}