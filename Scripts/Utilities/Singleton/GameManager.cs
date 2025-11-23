using Godot;
using System;

namespace GameJam
{
	public partial class GameManager : Node
	{
		public static GameManager Instance { get; private set; }
		public Timer GameTimer { get; set; }

		private bool _IsGameActive = false;
		public bool IsGameActive
		{
			get => _IsGameActive;
			set
			{
				_IsGameActive = value;
				if (value == true)
					GameStarted();
			}
		}
		private bool _IsTradingActive = false;
		public bool IsTradingActive { get; set; } = false;

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

		public void OnTimerTimeout()
		{
			IsGameActive = false;
		}

		public void GameStarted()
		{
			GameTimer.Start();
		}
	}
}
