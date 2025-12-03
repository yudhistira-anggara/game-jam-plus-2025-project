using Godot;
using System;

namespace GameJam
{
	public partial class TestButton : Button
	{
		private GameManager _gameManager { get; set; }
		private TradeManager _tradeManager { get; set; }
		private TraderManager _traderManager { get; set; }

		public override void _Pressed()
		{
			if (_gameManager is null)
				return;

			_tradeManager.TradeRequests.Clear();
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.TestSignal);

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

			if (TradeManager.Instance is null)
			{
				GD.PushError("TestButton; TradeManager.Instance");
				return;
			}

			_tradeManager = TradeManager.Instance;

			if (TraderManager.Instance is null)
			{
				GD.PushError("TestButton; TraderManager.Instance");
				return;
			}

			_traderManager = TraderManager.Instance;
		}
	}
}
