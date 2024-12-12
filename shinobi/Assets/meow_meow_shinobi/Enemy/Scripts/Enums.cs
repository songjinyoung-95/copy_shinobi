using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.Enemy
{
    public enum EEnemyCondition
    {
        Live,
        Die
    }

    public enum EEnemyState
    {
        Idle,
        Move,
        Attack,
        Die,
    }

    public enum EEnemyType
    {
        Hedgehog,
        Slime,
        Snake
    }

    public enum EBehavioralType
    {
        Default,
        Die_Splitting,
        Die_Explosive,
    }
}
