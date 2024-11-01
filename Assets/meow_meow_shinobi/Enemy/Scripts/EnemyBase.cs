using System;
using System.Collections.Generic;
using UnityEngine;

using Meow_Moew_Shinobi.FSM;
using Meow_Moew_Shinobi.Util;
using Meow_Moew_Shinobi.Weapon;
using Meow_Moew_Shinobi.Stage;
using Meow_Moew_Shinobi.Skill;
using Meow_Moew_Shinobi.Singleton;

namespace Meow_Moew_Shinobi.Enemy
{
    public delegate void FreezeEvent();
    public delegate void StunEvent();
    public delegate void KnockbackEvent();

    public abstract class EnemyBase : BehaviourBase, IHitReceiver
    {
        private readonly int ANI_HIT = Animator.StringToHash("hit");

        ///----------------------
        /// Get, Set
        /// ---------------------
        public Animator     Animator    { get; private set; }
        public Rigidbody2D  Rigidbody   { get; private set; }
        public INexus       TargetNexus { get; private set; }

        public EnemyScriptableData  Data                => _enemyData;
        public EEnemyType           EnemyType           => _enemyType;
        public EBehavioralType      EBehavioralType     => _behavioral;
        public EStatusEffect        EStatusEffect       => _currentStatus;

        ///----------------------
        /// SerializedField 
        /// ---------------------
        [SerializeField] private StatusUpdate           _updateStatus;
        [SerializeField] private EEnemyType             _enemyType;
        [SerializeField] private EBehavioralType        _behavioral;
        [SerializeField] private LayerMask              _layerMask;
        [SerializeField] private Animator               _animator;
        [SerializeField] private Rigidbody2D            _rigidbody;
        [SerializeField] private Collider2D             _collider;


        ///----------------------
        /// Private
        /// ---------------------
        private EnemyScriptableData             _enemyData;
        private int                             _health;
        private EStatusEffect                   _currentStatus;
        private FiniteStateMachine<EnemyBase>   _fsm;

        private bool _isInit;
        private Dictionary<EEnemyState, IState<EnemyBase>> _stateDict;


        ///----------------------
        /// Events
        /// ---------------------        
        public event Action<EnemyBase> DieEvent;
        public Action<EnemyBase> DespawnEvent { get; private set; }
        public event FreezeEvent    OnFreezeEvent;
        public event StunEvent      OnStunEvent;
        public event KnockbackEvent OnKnockbackEvent;


        /// <summary>
        /// Enemy 초기화
        /// </summary>
        protected abstract void OnInit();
        public void Init()
        {
            if(_isInit)
                return;

            /// Bind Componenet
            Animator    = _animator;
            Rigidbody   = _rigidbody;
            
            _updateStatus.OnFreeze      += ApplyFreeze;
            _updateStatus.OnFreeze      += () => VFXManager.Instance.ShowVFX(EVfxType.Freeze, transform.position, transform);

            _updateStatus.OnStun        += ApplyStun;

            _updateStatus.OnKnockback   += ApplyKnockback;
            
            _updateStatus.Init(RevertToPreviousState);

            /// Init FSM
            _fsm        = new FiniteStateMachine<EnemyBase>(this);
            _stateDict  = new Dictionary<EEnemyState, IState<EnemyBase>>()
            {
                {EEnemyState.Idle,      new EnemyIdleState(this)},
                {EEnemyState.Move,      new EnemyMoveState(this)},
                {EEnemyState.Die,       new EnemyDieState(this)},
                {EEnemyState.Attack,    new EnemyAttackState(this)},
            };

            OnInit();

            _isInit = true;
        }

        protected void HalfHelath()
        {
            _health = Mathf.RoundToInt(_health / 2);
        }

        public void ChangeState(EEnemyState eEnemy)
        {


            _fsm.ChangeState(_stateDict[eEnemy]);
        }

        protected abstract void OnSpawn();
        public void Spawn(EnemyScriptableData data, Action<EnemyBase> despawnAction)
        {
            TargetNexus     = null;
            DieEvent        = null;
            DespawnEvent    = despawnAction;

            _enemyData      = data;
            _health         = data.MaxHelath;

            _currentStatus  = EStatusEffect.None;

            Init();
            OnSpawn();

            _fsm.ChangeState(_stateDict[EEnemyState.Move]);
            gameObject.SetActive(true);
        }

        public void Release()
        {
            DieEvent        -= DieEvent;
            DespawnEvent    -= DespawnEvent;
            
            _updateStatus.ClearStatus();
        }

        private void Update()
        {
            if(_fsm == null)
                return;

            _fsm.CurrentState?.Update();
        }

        private void FixedUpdate()
        {
            if(_fsm == null)
                return;

            _fsm.CurrentState?.FixedUpdate();
        }

        private void RevertToPreviousState()
        {
            if(!_fsm.TryRevertToPreviousState())
                return;

            _currentStatus = EStatusEffect.None;
        }

        protected void AddState(EEnemyState eState, IState<EnemyBase> state)
        {
            if(_stateDict == null)
            {
                Debug.LogError($"{typeof(EnemyBase)} 초기화되지 않은 Dict 사용");
                return;
            }

            if(_stateDict.ContainsKey(eState))
            {
                Debug.LogWarning($"{eState} 동일한 키를 가진 스테이트가 이미 존재");
                return;
            }

            _stateDict.Add(eState, state);
        }

        protected void RemoveState(EEnemyState eState)
        {
            if (_stateDict == null)
            {
                Debug.LogError($"{typeof(EnemyBase)} 초기화되지 않은 Dict 사용");
                return;
            }

            if(!_stateDict.ContainsKey(eState))
            {
                Debug.LogWarning($"{eState} 키를 가진 스테이트가 없음");
                return;
            }

            _stateDict.Remove(eState);
        }

        

        private void OnTriggerEnter2D(Collider2D collider2D)
        {
            if (!IsInLayerMask(_layerMask, collider2D.gameObject.layer))
                return;

            Nexus nexus = CacheCollider<Nexus>.TryGetComponenet(collider2D);

            if(nexus == null)
                return;

            TargetNexus = nexus;
        }


        private bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return (layerMask & (1 << layer)) == 0;
        }
        


        private bool IsDieEnemy(int damage)
        {
            _health -= damage;

            if(_health <= 0)
                return true;

            return false;
        }


        /// --------------
        /// Interface
        /// --------------
        protected abstract void OnDie();
        public bool ApplyDamage(int damage)
        {
            _animator.SetTrigger(ANI_HIT);

            if(IsDieEnemy(damage))
            {
                OnDie();
                _fsm.ChangeState(_stateDict[EEnemyState.Die]);
                DieEvent?.Invoke(this);
                DespawnEvent?.Invoke(this);
                Release();
                return true;
            }

            return false;
        }


        public void ApplyKnockback(float force)
        {
            if(_fsm.CurrentState == _stateDict[EEnemyState.Die])
                return;

            _currentStatus = EStatusEffect.Knockback;
            _fsm.ChangeState(_stateDict[EEnemyState.Idle]);
            Rigidbody.AddForce(Vector2.up * force, ForceMode2D.Force);
        }

        public void ApplyStun()
        {
            if(_fsm.CurrentState == _stateDict[EEnemyState.Die])
                return;

            _currentStatus = EStatusEffect.Stun;
            _fsm.ChangeState(_stateDict[EEnemyState.Idle]);
        }

        public void ApplyFreeze()
        {
             if(_fsm.CurrentState == _stateDict[EEnemyState.Die])
                return;

            _currentStatus = EStatusEffect.Freeze;
            _fsm.ChangeState(_stateDict[EEnemyState.Idle]);
        }


        public void ApplyStatusEffect(List<StatusEffectData> statuses)
        {
            foreach (var status in statuses)
            {
                _updateStatus.AddStatus(status);
            }
        }


#if UNITY_EDITOR
        protected override void OnBindSerialzedField()
        {
            _rigidbody  = GetComponent<Rigidbody2D>();
            _animator   = GetComponent<Animator>();
            _collider   = GetComponent<Collider2D>();
        }

        private void OnDrawGizmos()
        {
            if(_enemyData == null)
                return;
            
            Gizmos.DrawWireSphere(transform.position, _enemyData.AttackRange);
            Gizmos.color = Color.red;
        }
#endif
    }
}
