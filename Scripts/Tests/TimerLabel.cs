using Godot;
using System;

namespace GameJam
{
    public partial class TimerLabel : Label
    {
        private Timer _gameManagerTimer { get; set; }

        public override void _Process(double delta)
        {
            if (_gameManagerTimer is null)
                return;

            var ts = TimeSpan.FromSeconds(_gameManagerTimer.TimeLeft);
            Text = ts.ToString(@"mm\:ss");
        }

        public override void _Ready()
        {
            if (GameManager.Instance is null)
            {
                GD.PushError("TimerLabel; GameManager.Instance");
                return;
            }

            _gameManagerTimer = GameManager.Instance.GameTimer;

            if (_gameManagerTimer is null)
            {
                GD.PushError("TimerLabel._gameManagerTimer; GameManager.Instance.GameTimer");
                return;
            }
        }
    }
}