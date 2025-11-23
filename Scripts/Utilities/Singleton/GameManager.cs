using Godot;
using System;

namespace GameJam
{
    public partial class GameManager : Node
    {
        public static GameManager Instance { get; private set; }
        public Timer GameTimer { get; set; }
        public int TradingCycle { get; set; } = 0;

        private bool _IsGameActive = false;
        public bool IsGameActive
        {
            get => _IsGameActive;
            set
            {
                _IsGameActive = value;
                if (value == true)
                    TradeDayStarted();
            }
        }
        public bool IsGameOver { get; set; } = false;

        public override void _Ready()
        {
            Instance = this;
            GameTimer = new Timer
            {
                WaitTime = 120,
                OneShot = true
            };
            GameTimer.Timeout += OnTimerTimeout;
            AddChild(GameTimer);
        }

        public void OnWeekOver()
        {
            IsGameOver = true;
            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GameEnd);
            GD.Print($"Game is Over!!!!!");
        }

        public void OnTimerTimeout()
        {
            IsGameActive = false;
            TradingCycle++;
            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeDayStart, false);

            if (TradingCycle >= 7)
            {
                OnWeekOver();
            }
        }

        public void TradeDayStarted()
        {
            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TradeDayStart, true);
            GameTimer.Start();
        }
    }
}