using System;
using TMPro;
using UnityEngine;

namespace Meow_Moew_Shinobi.Stage
{
    public interface INexus
    {
        Vector3 HitDistance { get; }

        void HitReceiver(int damage);
        
    }

    public class Nexus : MonoBehaviour, INexus
    {
        /// -------------------
        /// serializedField
        /// -------------------
        
        [SerializeField] private NexusView          _view;
        [Space(10)]
        [SerializeField] private Animator           _animator;
        [SerializeField] private SpriteRenderer     _nexus_Sprite;

        /// -------------------
        /// private
        /// -------------------
        
        private int _health;
        private int _maxHelath;

        private Action _onDefeat;
        private Action _onChangeHP;


        public Vector3 HitDistance => transform.position;

        public void HitReceiver(int damage)
        {
            _health -= damage;
            _view.Refresh(_health, _maxHelath);

            if(_health <= 0)
            {
                _view.Refresh(0, _maxHelath);
                _onDefeat?.Invoke();
            }
        }

        public void Init(int health, Action defeat, Action recover)
        {
            _health     = health;
            _maxHelath  = health;
            _onDefeat   = defeat;

            _onChangeHP = recover;
            _onChangeHP += () => _view.Refresh(_health, _maxHelath);

            _view.Init(_health, _maxHelath);
        }

        [Serializable]
        private class NexusView
        {
            private const float DEFAULT_RTF_SIZE_X = 600;
            private const float DEAFULT_RTF_SIZE_Y = 52;

            [SerializeField] private Canvas             _canavs;
            [SerializeField] private RectTransform      _health_RTF;
            [SerializeField] private TextMeshProUGUI    _health_TMP;

            public void Init(int current, int max)
            {
                _canavs.worldCamera = Camera.main;

                Refresh(current, max);
            }

            public void Refresh(int current, int max)
            {
                float sizeX = (float)current / max;
                float sizeY = sizeX <= 0 ? 0 : DEAFULT_RTF_SIZE_Y;

                _health_RTF.sizeDelta   = new Vector2(sizeX * DEFAULT_RTF_SIZE_X, sizeY);
                _health_TMP.text        = $"{current}";
            }
        }
    }
}