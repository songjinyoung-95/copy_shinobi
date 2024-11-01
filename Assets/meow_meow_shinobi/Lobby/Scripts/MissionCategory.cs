using System;
using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Stage;
using Meow_Moew_Shinobi.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Meow_Moew_Shinobi.Lobby
{
    public class MissionCategory : LobbyCategoryBase
    {
        [Serializable]
        private class StageView
        {
            public GameObject View;
            public int StageLevel;
        }

        public int SelectStageLevel => _selectStageView.StageLevel;


        [Space(10)]
        [Header("자식 클래스")]
        [SerializeField] private StageView[] _stageViews;

        [Space(10)]
        [SerializeField] private Button _moveStage_BTN;


        private StageView _selectStageView;

        protected override void OnHide()
        {
            
        }

        protected override void OnInit()
        {
            _selectStageView = _stageViews[0];
        }

        protected override void OnRefrsh()
        {

        }

        protected override void OnShow()
        {
            
        }

        public void AddLinster_SelectStage(Action moveStage)
        {
            if(moveStage == null)
                return;

            _moveStage_BTN.onClick.AddListener(moveStage.Invoke);
        }
    }
}