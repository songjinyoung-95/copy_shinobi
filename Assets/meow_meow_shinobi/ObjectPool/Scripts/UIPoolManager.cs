using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Meow_Moew_Shinobi.Pool;
using Meow_Moew_Shinobi.UI.EffectText;
using Meow_Moew_Shinobi.UI;

namespace Meow_Moew_Shinobi.Singleton
{
    public class UIPoolManager
    {
        private const string CANVAS_PATH    = "UI/Model/UICanvas";
        private const string HIT_TEXT_PATH  = "UI/Model/HitText";
        
        private static UIPoolManager _instance;
        public static UIPoolManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UIPoolManager();

                return _instance;
            }
        }

        private PoolBase<HitText> _textPool;
        private UIPoolCanvas _canvas;

        public UIPoolManager()
        {
            if(_canvas == null)
            {
                UIPoolCanvas canvas = Resources.Load<UIPoolCanvas>(CANVAS_PATH);

                if(canvas == null)
                {
                    Debug.LogError($"{CANVAS_PATH} 경로에 프리펩이 존재하지 않습니다 ");
                    return;
                }
                
                _canvas = Object.Instantiate(canvas);
            }

            _canvas.Init();
            HitTextGenerator();
        }

        private void HitTextGenerator()
        {
            _textPool = new PoolBase<HitText>(_canvas.transform);

            HitText text = Resources.Load<HitText>(HIT_TEXT_PATH);
            if(text == null)
            {
                Debug.LogError($"{HIT_TEXT_PATH}에 해당하는 오브젝트가 존재하지 않습니다");
                return;
            }

            _textPool.Generator(text, text.FontType.ToString(), 30);
        }

        public HitText ShowHitText(ETextType textType, Vector3 pos)
        {
            HitText hitText = _textPool.Spawn(textType.ToString(), pos);
            hitText.OnDespawnEvent += _textPool.Despawn;

            return hitText;
        }
    }
}