using Godot;
using System;
using System.Reflection;

namespace GameJam
{
	public partial class TestProgress : ProgressBar
	{
		private Timer _gameManagerTimer { get; set; }
		public override void _Process(double delta)
		{
			if (_gameManagerTimer is null)
				return;

			if (_gameManagerTimer.IsStopped())
				return;

			var wt = _gameManagerTimer.WaitTime;
			var tl = _gameManagerTimer.TimeLeft;

			var p = 1 - (wt - tl) / wt;
			Value = p;
		}

		public override void _Ready()
		{
			if (GameManager.Instance is null)
			{
				GD.PushError("TestProgress; GameManager.Instance");
				return;
			}

			_gameManagerTimer = GameManager.Instance.GameTimer;
			
			if (_gameManagerTimer is null)
			{
				GD.PushError("TestProgress._gameManagerTimer; GameManager.Instance.GameTimer");
				return;
			}
		}
	}
}
