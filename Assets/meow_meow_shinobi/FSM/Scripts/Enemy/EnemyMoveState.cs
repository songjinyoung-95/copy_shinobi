using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Enemy;
using UnityEngine;

namespace Meow_Moew_Shinobi.FSM
{
    public class EnemyMoveState : StateBase<EnemyBase>
    {
        private readonly int KEY_IDLE = Animator.StringToHash("move");

        private Rigidbody2D _rigidbody;
        private float       _moveSpeed;
        private float       _attackRange;

        public EnemyMoveState(EnemyBase owner) : base(owner)
        {
            _rigidbody  = owner.Rigidbody;
        }

        public override void Enter()
        {
            _moveSpeed      = Owner.Data.MoveSpeed;
            _attackRange    = Owner.Data.AttackRange;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            
        }

        public override void FixedUpdate()
        {
            _rigidbody.velocity = _moveSpeed * Time.fixedDeltaTime * Vector2.down;
        }

        public override void Update()
        {
            if(Owner.TargetNexus == null)
                return;

            float dir = Mathf.Abs(Owner.transform.position.y - Owner.TargetNexus.HitDistance.y);
            if (_attackRange >= dir)
            {
                _rigidbody.velocity = Vector2.zero;
                Owner.ChangeState(EEnemyState.Attack);
                return;
            }                
        }
    }
}