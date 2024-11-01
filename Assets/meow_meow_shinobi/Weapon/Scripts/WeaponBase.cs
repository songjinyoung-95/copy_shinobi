using System;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Skill;
using Meow_Moew_Shinobi.UI.EffectText;
using Meow_Moew_Shinobi.Util;
using UnityEngine;

namespace Meow_Moew_Shinobi.Weapon
{
    public interface IHitReceiver
    {
        bool ApplyDamage(int damage);
        void ApplyStatusEffect(List<StatusEffectData> statuses);
        // void ApplyKnockback(float force);
    }

    public delegate HitText WeaponHitEffect(ETextType textType, Vector3 pos);
    public delegate void HitSplit(Vector3 spawnPos);
    public abstract class WeaponBase : BehaviourBase
    {
        ///----------------------
        /// Get, Set
        /// ---------------------

        private bool IsMaximumHitCount
        {
            get
            {
                if (_applyHitCount >= WeaponData.ApplyHitCount)
                    return true;

                return false;
            }
        }        


        ///----------------------
        /// SerializedField 
        /// ---------------------
        [SerializeField] private EWeaponType    _weaponType;
        [SerializeField] private LayerMask      _layerMask;
        [SerializeField] private SpriteRenderer _wepaonSprite;
        [SerializeField] private Collider2D     _hitCollider;

        ///----------------------
        /// protected
        /// ---------------------
        protected WeaponData    WeaponData { get; private set; }
        protected LayerMask     LayerMask   => _layerMask;
        protected string        WeaponType  => _weaponType.ToString();
        protected Collider2D    HitCollider => _hitCollider;


        ///----------------------
        /// events
        /// ---------------------
        protected Action<WeaponBase, string> DespawnEvent { get; private set; }
        protected WeaponHitEffect HitEffect => OnHitEffect;


        ///----------------------
        /// private
        /// ---------------------
        private int _applyHitCount;
        private bool _isInit;
        private Camera _cam;
        private List<StatusEffectData> _effectDatas;
        private HashSet<IHitReceiver> _hitReceivers = new HashSet<IHitReceiver>();

        public event WeaponHitEffect OnHitEffect;
        public event HitSplit OnHitSplit;

        protected abstract void OnInit();

        private void Start()
        {
            _cam = Camera.main;
        }

        public void Init(WeaponData data, List<StatusEffectData> effectDatas, Action<WeaponBase, string> despawnEvent)
        {
            WeaponData      = data;
            _effectDatas    = effectDatas;

            DespawnEvent    = despawnEvent;
            _applyHitCount  = 0;

            _hitCollider.enabled = true;
            _hitReceivers.Clear();

            OnInit();
        }

        protected abstract void OnFire(Vector3 targetPos);
        public void Fire(Vector3 targetPos)
        {
            gameObject.SetActive(true);
            OnFire(targetPos);
        }

        public void SetWeaponSize(Vector3 size) { transform.localScale = size; }

        /// 
        protected abstract void OnFireTarget(List<Transform> enemiesTF, int index);
        public void FireTarget(List<Transform> enemiesTF, int index) { OnFireTarget(enemiesTF, index); }

        protected bool IsInScreenView()
        {
            Vector3 screenPos = _cam.WorldToViewportPoint(transform.position);
            return screenPos.x >= 0 && screenPos.x <= 1.2f && screenPos.y >= 0 && screenPos.y <= 1.2f;
        }


        protected abstract void OnUpdate();
        private void Update()
        {
            if(!IsInScreenView())
                DisableWeapon();

            OnUpdate();
        }

        private void StatusEffect(IHitReceiver hitReceiver)
        {
            if (_effectDatas.Count <= 0)
                return;

            hitReceiver.ApplyStatusEffect(_effectDatas);
        }

        protected abstract void OnHit(IHitReceiver hitReceiver);
        private void Hit(IHitReceiver hitReceiver, Vector3 pos)
        {
            if(_hitReceivers.Contains(hitReceiver))
                return;

            _hitReceivers.Add(hitReceiver);

            int damage = Mathf.RoundToInt(UnityEngine.Random.Range(WeaponData.Damage * 0.8f, WeaponData.Damage * 1f));
            WeaponInfoRecorder.RecordDamage(_weaponType, damage);

            OnHitEffect?.Invoke(ETextType.HitText, pos).Show(pos, damage);
            
            hitReceiver.ApplyDamage(damage);
            OnHit(hitReceiver);
            StatusEffect(hitReceiver);

            _applyHitCount++;

            if (IsMaximumHitCount)
                DisableWeapon();
        }

        protected virtual void OnDisableWeapon() { DespawnEvent?.Invoke(this, WeaponType); }
        private void DisableWeapon()
        {
            OnDisableWeapon();

            OnHitSplit?.Invoke(transform.position);

            OnHitEffect -= OnHitEffect;
            OnHitSplit  -= OnHitSplit;
            
            _hitCollider.enabled = false;
        }


        private void OnTriggerEnter2D(Collider2D collider2D)
        {
            if(!IsInLayerMask(_layerMask, collider2D.gameObject.layer))
                return;

            IHitReceiver hitReceiver = CacheCollider<IHitReceiver>.TryGetComponenet(collider2D);
            
            if(hitReceiver == null)
            {
                Debug.LogWarning($"{collider2D.gameObject.name} {nameof(IHitReceiver)} 인터페이스를 상속받지 않음");
                return;
            }

            if(IsMaximumHitCount)
                return;

            Hit(hitReceiver, collider2D.transform.position);
        }

        private bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return (layerMask & (1 << layer)) == 0;
        }

#if UNITY_EDITOR
        protected override void OnBindSerialzedField()
        {
            _wepaonSprite   = transform.Find("sprite").GetComponent<SpriteRenderer>();
            _hitCollider    = GetComponent<Collider2D>();
        }
#endif
    }
}