using System;
using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Lobby;
using Meow_Moew_Shinobi.Util;
using Meow_Moew_Shinobi.Weapon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meow_Moew_Shinobi.Stage
{
    public interface IStaeView
    {
        void Init(string StageName);
        void UpdateLevelPrograss(int gainExp, int needExp);
        void UpdateWaveLevelText(int currentWave, int maxWave);

        void ShowWaveTimer();
        void HideWaveTimer();
        void UpdateWaveTimer(int nextWaveTime);
    }

    public class StageView : BehaviourBase, IStaeView
    {
        [Serializable]
        private class ResultView
        {
            public GameObject   OBJ_View;
            public Button       BTN_MoveToLobby;

            public GameObject[]         OBJ_Star_Level;
            public RewardItemView[]     RewardItems;
            public EquipWeaponView[]    EquipWeapons;

            public void ShowRewardView(Action onLobby, EquipWeaponResult weaponResult, RewardItemResult itemResult)
            {
                OBJ_View.SetActive(true);
                BTN_MoveToLobby.onClick.AddListener(onLobby.Invoke);

                ShowWeaponDamageView(weaponResult);
                ShowItemRewardView(itemResult);
            }

            public void HideView()
            {
                HideWeaponDamageView();
                HideRewardItemView();
                OBJ_View.SetActive(false);
            }

            private void ShowWeaponDamageView(EquipWeaponResult weaponResult)
            {
                int count = weaponResult.WeaponSprites.Length;
                for (int i = 0; i < count; i++)
                    EquipWeapons[i].ShowView(weaponResult.WeaponSprites[i], weaponResult.WeaponLevels[i], weaponResult.WeaponTotalDamages[i]);
            }

            private void ShowItemRewardView(RewardItemResult itemResult)
            {
                int count = itemResult.ItemSprites.Length;
                for (int i = 0; i < count; i++)
                    RewardItems[i].ShowView(itemResult.ItemSprites[i], itemResult.RewardCounts[i]);
            }

            private void HideWeaponDamageView()
            {
                foreach (var item in RewardItems)
                {
                    item.HideView();
                }
            }

            private void HideRewardItemView()
            {
                foreach (var item in EquipWeapons)
                {
                    item.HideView();
                }
            }


            [Serializable]
            public class EquipWeaponView
            {
                public GameObject Parent;
                public Image IMG_EquipWeapon;
                public TextMeshProUGUI TMP_EquipLevel;
                public TextMeshProUGUI TMP_TotalDamage;

                public EquipWeaponView(GameObject parent, Image image, TextMeshProUGUI level, TextMeshProUGUI damage)
                {
                    Parent = parent;
                    IMG_EquipWeapon = image;
                    TMP_EquipLevel = level;
                    TMP_TotalDamage = damage;
                }

                public void ShowView(Sprite sprite, int level, int totalDamage)
                {
                    IMG_EquipWeapon.sprite  = sprite;
                    TMP_EquipLevel.text     = $"Lv.{level}";
                    TMP_TotalDamage.text    = totalDamage.ToString();

                    Parent.SetActive(true);
                }

                public void HideView()
                {
                    Parent.SetActive(false);
                }
            }

            [Serializable]
            public class RewardItemView
            {
                public GameObject Parent;
                public Image IMG_Reward_Item;
                public TextMeshProUGUI TMP_Reward_Count;

                public RewardItemView(GameObject parent, Image img, TextMeshProUGUI text)
                {
                    Parent = parent;
                    IMG_Reward_Item = img;
                    TMP_Reward_Count = text;
                }

                public void HideView()
                {
                    Parent.SetActive(false);
                }                
                
                public void ShowView(Sprite sprite, int count)
                {
                    IMG_Reward_Item.sprite  = sprite;
                    TMP_Reward_Count.text   = $"x{count}";

                    Parent.SetActive(true);
                }
            }

            #if UNITY_EDITOR
            public void BindSerializedField(Transform rewardView)
            {
                OBJ_View        = rewardView.gameObject;
                BTN_MoveToLobby = rewardView.Find("BTN_Next").GetComponent<Button>();

                OBJ_Star_Level  = new GameObject[rewardView.Find("Stars").childCount];
                RewardItems     = new RewardItemView[rewardView.Find("RewardViews").childCount];
                EquipWeapons    = new EquipWeaponView[rewardView.Find("EquipWeapons").childCount];

                for (int i = 0; i < OBJ_Star_Level.Length; i++)
                    OBJ_Star_Level[i] = rewardView.Find("Stars").GetChild(i).gameObject;

                for (int i = 0; i < RewardItems.Length; i++)
                {
                    Transform       parent  = rewardView.Find("RewardViews").GetChild(i);
                    Image           img     = parent.Find("IMG_Weapon").GetComponent<Image>();
                    TextMeshProUGUI tmp     = parent.Find("TMP_Count").GetComponent<TextMeshProUGUI>();

                    RewardItems[i] = new RewardItemView(parent.gameObject, img, tmp);
                }

                for (int i = 0; i < EquipWeapons.Length; i++)
                {
                    Transform       parent  = rewardView.Find("EquipWeapons").GetChild(i);
                    Image           img     = parent.Find("IMG_Weapon").GetComponent<Image>();
                    TextMeshProUGUI level   = parent.Find("TMP_Level").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI damage  = parent.Find("TMP_TotalDamage").GetComponent<TextMeshProUGUI>();

                    EquipWeapons[i] = new EquipWeaponView(parent.gameObject, img, level, damage);
                }
            }
            #endif
        }


        private const float DEFAULT_FILL_SIZE_X = 350;
        private const float DEAFULT_FILL_SIZE_Y = 52;

        [Header("스테이지 결과 보상창")]
        [SerializeField] private ResultView _resultView;
        [Space(10)]

        [SerializeField] private GameObject _waveTimer_OBJ;

        [SerializeField] private RectTransform  _expFill_RTF;
        [SerializeField] private Button _timeScale_BTN;

        [SerializeField] private TextMeshProUGUI _waveLevel_TMP;
        [SerializeField] private TextMeshProUGUI _stageName_TMP;
        [SerializeField] private TextMeshProUGUI _waveTimer_TMP;
        [SerializeField] private TextMeshProUGUI _timeScale_TMP;
        

        private bool _isChangedTimeScale = false;

        public event Action OnMoveToLobby;
        

        public void Init(string StageName)
        {
            _stageName_TMP.text        = StageName;
            _expFill_RTF.sizeDelta     = new Vector2(0, DEAFULT_FILL_SIZE_Y);

            _timeScale_BTN.onClick.RemoveAllListeners();
            _timeScale_BTN.onClick.AddListener(GameScaleChangeButton);

            _resultView.HideView();
            HideWaveTimer();
        }

        public void UpdateLevelPrograss(int current, int max)
        {   
            float sizeX = (float)current / max;
            float sizeY = sizeX <= 0 ? 0 : DEAFULT_FILL_SIZE_Y;

            _expFill_RTF.sizeDelta = new Vector2(sizeX * DEFAULT_FILL_SIZE_X, sizeY);
        }

        public void UpdateWaveLevelText(int currentWaveLevel, int maxWaveLevel)
        {
            _waveLevel_TMP.text = $"Wave {currentWaveLevel}/{maxWaveLevel}";
        }

        public void GameScaleChangeButton()
        {
            _isChangedTimeScale = !_isChangedTimeScale;

            if (_isChangedTimeScale)
            {
                _timeScale_TMP.text = "x2";
                Time.timeScale = 2;
            }
            else
            {
                _timeScale_TMP.text = "x1";
                Time.timeScale = 1;
            }                        
        }

        public void ShowResultView(EquipWeaponResult weaponResult, RewardItemResult itemResult)
        {
            _resultView.ShowRewardView(OnMoveToLobby, weaponResult, itemResult);
        }

        public void ShowWaveTimer() => _waveTimer_OBJ.SetActive(true);
        public void HideWaveTimer() => _waveTimer_OBJ.SetActive(false);
        public void UpdateWaveTimer(int time) => _waveTimer_TMP.text = $"{time}";



#if UNITY_EDITOR
        protected override void OnBindSerialzedField()
        {            
            _expFill_RTF   = transform.Find("IMG_Wave").Find("IMG_WaveFill").GetComponent<RectTransform>();
            _waveLevel_TMP  = transform.Find("TMP_Wave_Level").GetComponent<TextMeshProUGUI>();
            _stageName_TMP  = transform.Find("TMP_Stage_Name").GetComponent<TextMeshProUGUI>();
            _timeScale_BTN  = transform.Find("BTN_TimeScale").GetComponent<Button>();
            _timeScale_TMP  = transform.Find("BTN_TimeScale").Find("TMP_TimeScale").GetComponent<TextMeshProUGUI>();

            _waveTimer_OBJ  = transform.Find("OBJ_WaveTimer").gameObject;
            _waveTimer_TMP  = transform.Find("OBJ_WaveTimer").Find("TMP_WaveTimer").GetComponent<TextMeshProUGUI>();
            
            _waveLevel_TMP.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Maplestory OTF Bold SDF");
            _stageName_TMP.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Maplestory OTF Bold SDF");
            _waveTimer_TMP.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Maplestory OTF Bold SDF");


            _resultView.BindSerializedField(transform.Find("RewardView"));
        }
#endif
    }

    public struct EquipWeaponResult
    {
        public Sprite[] WeaponSprites        { get; private set; }
        public int[]    WeaponLevels         { get; private set; }
        public int[]    WeaponTotalDamages   { get; private set; }

        public EquipWeaponResult(Sprite[] sprites, int[] levels, int[] totalDamages)
        {
            WeaponSprites        = sprites;
            WeaponLevels         = levels;
            WeaponTotalDamages   = totalDamages;
        }
    }

    public struct RewardItemResult
    {
        public Sprite[] ItemSprites  { get; private set; }
        public int[]    RewardCounts { get; private set; }

        public RewardItemResult(Sprite[] sprites, int[] counts)
        {
            ItemSprites     = sprites;
            RewardCounts    = counts;
        }
    }
}