using System;
using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Util;
using TMPro;
using UnityEngine;

namespace Meow_Moew_Shinobi.UI.EffectText
{
    public class HitText : TextBase
    {
        private const float EFFECT_TIME = 1.5f;

        public ETextType FontType => _fontType;

        [SerializeField] private ETextType _fontType;
        [SerializeField] private TextMeshProUGUI _hitText_TMP;


        public event Action<HitText, string> OnDespawnEvent;



        public void Show(Vector3 pos, int damage)
        {
            _hitText_TMP.text = $"{damage}";

            Vector2 randomPosition = UnityEngine.Random.insideUnitCircle * 0.25f;
            pos.x += randomPosition.x;
            pos.y += randomPosition.y;
            
            transform.position = pos;

            gameObject.SetActive(true);

            StartCoroutine(Co_Show());
        }

        public void Hide()
        {
            OnDespawnEvent?.Invoke(this, _fontType.ToString());
            OnDespawnEvent -= OnDespawnEvent;
        }

        private IEnumerator Co_Show()
        {
            // RectTransform textTF = _hitText_TMP.transform as RectTransform;
            // float time = 0;

            // while(time <= EFFECT_TIME)
            // {
            //     time += Time.deltaTime;
                
            //     Vector2 pos = textTF.anchoredPosition;
            //     pos.y += 0.001f;
                
            //     textTF.anchoredPosition = pos;
            //     yield return null;
            // }
            yield return new WaitForSeconds(1);
            Hide();
        }

#if UNITY_EDITOR
        protected override void OnBindSerialzedField()
        {
            _hitText_TMP = transform.Find("TMP_HitText").GetComponent<TextMeshProUGUI>();
        }
#endif
    }
}