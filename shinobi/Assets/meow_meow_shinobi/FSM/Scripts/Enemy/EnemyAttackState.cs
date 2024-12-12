using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Enemy;
using Meow_Moew_Shinobi.Stage;
using UnityEngine;

namespace Meow_Moew_Shinobi.FSM
{
    public class EnemyAttackState : StateBase<EnemyBase>
    {
        private readonly int KEY_IDLE = Animator.StringToHash("attack");

        private float _attackRange;
        private float _attackDelay;
        private float _attackTime;

        public EnemyAttackState(EnemyBase owner) : base(owner) { }

        public override void Enter()
        {
            _attackTime  = 0;
            _attackRange = Owner.Data.AttackRange;
            _attackDelay = Owner.Data.AttackDelay;
        }

        public override void Execute()
        {
            _attackTime = 0;
            Owner.TargetNexus.HitReceiver(Owner.Data.Power);
        }

        public override void Exit()
        {
            
        }

        public override void FixedUpdate()
        {

        }

        public override void Update()
        {
            if (Owner.TargetNexus == null)
                return;

            float dir = Mathf.Abs(Owner.transform.position.y - Owner.TargetNexus.HitDistance.y);
            if (_attackRange < dir)
            {
                Owner.ChangeState(EEnemyState.Move);
                return;
            }

            _attackTime += Time.deltaTime;

            if(_attackTime >= _attackDelay)
                Execute();
        }
    }
}