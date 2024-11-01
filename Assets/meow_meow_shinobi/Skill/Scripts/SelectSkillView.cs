using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Meow_Moew_Shinobi.Util;
using System;

namespace Meow_Moew_Shinobi.Skill
{
    public delegate void SelectedSkill(SkillData data);

    public interface ISkillView
    {
        void Init(Action rerollAction);
        void ShowSkillView(SkillData[] datas);
        void HideSkillView();

        public event SelectedSkill OnSelectedSkill;
    }


    public class SelectSkillView : BehaviourBase, ISkillView
    {
        [SerializeField] private Button _reroll_BTN;
        [SerializeField] private Slot[] _slots;

        private float _currnetTimeScale;

        public event SelectedSkill OnSelectedSkill;

        public void HideSkillView()
        {
            gameObject.SetActive(false);
        }

        public void Init(Action rerollAction)
        {
            foreach (var slot in _slots)
            {
                slot.SelectButtonEvent += StopPause;
                slot.SelectButtonEvent += HideSkillView;

                slot.SelectButtonEvent += () => OnSelectedSkill?.Invoke(slot.LinkData);

                slot.ButtonInit();
            }

            _reroll_BTN.onClick.AddListener(rerollAction.Invoke);

            HideSkillView();
            DontDestroyOnLoad(gameObject);
        }

        private void Puase()
        {
            if(Time.timeScale <= 0)
                return;

            _currnetTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        private void StopPause()
        {
            Time.timeScale = _currnetTimeScale;
        }

        public void ShowSkillView(SkillData[] datas)
        {
            Puase();
            
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].SetData(datas[i]);

                _slots[i].Thumnail.sprite   = datas[i].Thumnail;
                _slots[i].TMP_Name.text     = datas[i].Name;
                _slots[i].TMP_Info.text     = datas[i].Info;

                _slots[i].SetLevelText(datas[i].Level);
            }

            gameObject.SetActive(true);
        }


#if UNITY_EDITOR
        protected override void OnBindSerialzedField()
        {
            _slots = new Slot[3];
            Transform slotParent = transform.Find("SelectView");
            for (int i = 0; i < _slots.Length; i++)
            {
                Transform slot = slotParent.GetChild(i);

                Image           thumnail    = slot.Find("Thumnail").Find("IMG_Thumnail").GetComponent<Image>();
                Button          select      = slot.GetComponent<Button>();
                TextMeshProUGUI name_TMP    = slot.Find("TMP_Skill_Name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI level_TMP   = slot.Find("TMP_Skill_Level").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI info_TMP    = slot.Find("TMP_Skill_Info").GetComponent<TextMeshProUGUI>();


                name_TMP.font   = Resources.Load<TMP_FontAsset>("Fonts & Materials/Maplestory OTF Bold SDF");
                level_TMP.font  = Resources.Load<TMP_FontAsset>("Fonts & Materials/Maplestory OTF Bold SDF");
                info_TMP.font   = Resources.Load<TMP_FontAsset>("Fonts & Materials/Maplestory OTF Bold SDF");

                _slots[i] = new Slot(thumnail, select, name_TMP, level_TMP, info_TMP);
            }
        }
#endif


        [Serializable]
        private class Slot
        {
            public SkillData LinkData { get; private set; }
            public Image Thumnail;
            public Button BTN_Select;
            public TextMeshProUGUI TMP_Name;
            public TextMeshProUGUI TMP_Level;
            public TextMeshProUGUI TMP_Info;

            public event Action SelectButtonEvent;

            public Slot(Image thumnail, Button select, TextMeshProUGUI name, TextMeshProUGUI level, TextMeshProUGUI info)
            {
                Thumnail = thumnail;
                BTN_Select = select;
                TMP_Name = name;
                TMP_Level = level;
                TMP_Info = info;
            }

            public void SetData(SkillData data)
            {
                LinkData = data;
            }

            public void SetLevelText(int level)
            {
                if(level == 0)
                {
                    TMP_Level.color = Color.yellow;
                    TMP_Level.text  = "New";
                }
                else
                {
                    TMP_Level.color = Color.white;
                    TMP_Level.text = $"{level} >> {level + 1}";
                }
            }

            public void ButtonInit()
            {
                BTN_Select.onClick.AddListener(() => SelectButtonEvent?.Invoke());
            }
        }

    }
}