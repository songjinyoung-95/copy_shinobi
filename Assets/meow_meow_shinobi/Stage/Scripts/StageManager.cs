using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Enemy;
using Meow_Moew_Shinobi.Lobby;
using Meow_Moew_Shinobi.Stage;
using Meow_Moew_Shinobi.Util;
using Meow_Moew_Shinobi.Weapon;
using UnityEngine;

namespace Meow_Moew_Shinobi.Singleton
{
    public enum EStageState
    {
        Playing,
        End,
    }

    public class StageManager
    {
        private const string STAGE_PATH = "Stage/Level/";
        private const string VIEW_PATH  = "Stage/StageView";

        private static StageManager _instance;
        public static StageManager  Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new StageManager();

                return _instance;
            }
        }

        public EStageState StageState { get; private set; }

        private StageController _stage;
        private StageView       _view;
        public (Vector2 minArea, Vector2 maxArea) StageArea => (_stage.StageArea_Min, _stage.StageArea_Max);


        public StageManager()
        {
            StageView view = Resources.Load<StageView>(VIEW_PATH);

            if(view == null)
            {
                Debug.LogError($"{VIEW_PATH} 에 프리펩이 없습니다");
                return;                
            }

            _view = Object.Instantiate(view);

            _view.OnMoveToLobby  += LobbyManager.Instance.MoveToLobby;
            _view.OnMoveToLobby  += EnemyManager.Instance.AllReturnEnemies;
            _view.OnMoveToLobby  += SkillManager.Instance.Release;
            _view.OnMoveToLobby  += CacheCollider<INexus>.Release;
            _view.OnMoveToLobby  += ReleaseStage;
            _view.OnMoveToLobby  += WeaponInfoRecorder.Release;

            Object.DontDestroyOnLoad(_view.gameObject);
        }

        public void SetStage(int loadLevel)
        {
            string path = STAGE_PATH + loadLevel;
            StageController stage =  Resources.Load<StageController>(path);

            if(stage == null)
            {
                Debug.LogError($"{path} 에 해당하는 스테이지가 없습니다 / \n 레벨 : {loadLevel}");
                return;
            }

            _stage = Object.Instantiate(stage);
            
            _stage.OnSpawnEnemy     += EnemyManager.Instance.SpawnEnemy;

            _stage.OnLevelUp        += SkillManager.Instance.ShowSkillView;

            _stage.OnEndStage       += EndStage;
            _stage.OnEndStage       += EnemyManager.Instance.AllStopEnemies;

            _stage.Init(_view);
            StartStage();
        }

        public Transform GetCharacterTransform() => _stage.TF_Character;


        private void StartStage()
        {
            SkillManager.Instance.CharacterEquipWeapon(CharacterManager.Instance.EquippedWeapon);
            _stage.StartWave();

            StageState = EStageState.Playing;
        }

        private void EndStage()
        {
            _view.ShowResultView(GetResultEquipWeaponData(), GetResultRewardItemData());

            WeaponManager.Instance.Release();
            StageState = EStageState.End;
        }

        private void ReleaseStage()
        {
            _stage.DestroyStage();
            // _stage = null;
        }


        public void NewSplitSpawnEnemy(EnemyBase enemy) => _stage.SpawnSplitEnemy(enemy);


        private EquipWeaponResult GetResultEquipWeaponData()
        {
            EWeaponType[] weaponTypes = StageHelper.EquipWeapons;

            int count = weaponTypes.Length;

            Sprite[]    weaponSprites   = new Sprite[count];
            int[]       weaponLevels    = new int[count];
            int []      totalDamage     = new int[count];

            for (int i = 0; i < weaponSprites.Length; i++)
            {
                EWeaponType weaponType = weaponTypes[i];

                weaponSprites[i]    = WeaponSpriteMapper.GetWeaponUISprite(weaponType);
                weaponLevels[i]     = WeaponInfoRecorder.GetWeaponLevel(weaponType);
                totalDamage[i]      = WeaponInfoRecorder.GetWeaponTotalDamage(weaponType);
            }


            return new EquipWeaponResult(weaponSprites, weaponLevels, totalDamage);
        }

        private RewardItemResult GetResultRewardItemData()
        {
            return new RewardItemResult(new Sprite[3], new int[] { 5, 12, 3 });
        }        
    }
}