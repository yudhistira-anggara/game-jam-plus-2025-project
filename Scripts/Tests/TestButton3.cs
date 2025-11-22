using Godot;
using System;

namespace GameJam
{
    public partial class TestButton3 : Button
    {
        private GameManager _gameManager { get; set; }
        private TraderManager _traderManager { get; set; }

        public override void _Pressed()
        {
            if (_traderManager is null)
                return;

            _gameManager.IsGameActive = true;
            _traderManager.InitializeTraders();
            Disabled = true;
        }

        public void OnGameTimerOver()
        {
            Disabled = false;
        }

        public override void _Ready()
        {
            if (GameManager.Instance is null)
            {
                GD.PushError("TestButton; GameManager.Instance");
                return;
            }

            _gameManager = GameManager.Instance;
            _gameManager.GameTimer.Timeout += OnGameTimerOver;

            if (TraderManager.Instance is null)
            {
                GD.PushError("TestButton; GameManager.Instance");
                return;
            }

            _traderManager = TraderManager.Instance;
        }
    }
}