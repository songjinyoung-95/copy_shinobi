using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Weapon;
using UnityEngine;

public static class WeaponInfoRecorder
{
    private static Dictionary<EWeaponType, int> _totalDamageDict = new Dictionary<EWeaponType, int>();
    private static Dictionary<EWeaponType, int> _weaponLevelDict = new Dictionary<EWeaponType, int>();

    public static void Release()
    {
        _totalDamageDict?.Clear();
        _weaponLevelDict?.Clear();
    }

    /// <summary>
    /// 웨폰 별 토탈 데미지 저장
    /// </summary>
    public static void RecordDamage(EWeaponType weaponType, int damage)
    {
        if(_totalDamageDict.ContainsKey(weaponType))
        {
            _totalDamageDict[weaponType] += damage;
        }
        else
        {
            _totalDamageDict.Add(weaponType, 0);
        }
    }

    /// <summary>
    /// 웨폰 별 토탈 데미지 리턴
    /// </summary>
    public static int GetWeaponTotalDamage(EWeaponType weaponType)
    {
        if(!_totalDamageDict.ContainsKey(weaponType))
            return 0;

        return _totalDamageDict[weaponType];
    }

    /// <summary>
    /// 웨폰 별 레벨 저장
    /// </summary>
    public static void RecordWeaponLevel(EWeaponType weaponType)
    {
        if(_weaponLevelDict.ContainsKey(weaponType))
        {
            _weaponLevelDict[weaponType]++;
        }
        else
        {
            _weaponLevelDict.Add(weaponType, 1);
        }
    }

    public static int GetWeaponLevel(EWeaponType weaponType)
    {
        if(!_weaponLevelDict.ContainsKey(weaponType))
            return 1;

        return _weaponLevelDict[weaponType];
    }
}
