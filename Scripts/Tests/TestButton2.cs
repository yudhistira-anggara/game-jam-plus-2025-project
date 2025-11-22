using Godot;
using System;

namespace GameJam
{
    public partial class TestButton2 : Button
    {
        private GameManager _gameManager { get; set; }

        public override void _Pressed()
        {
            if (_gameManager is null)
                return;

            _gameManager.IsGameActive = true;
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
        }
    }
}