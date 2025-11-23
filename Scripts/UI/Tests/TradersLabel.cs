using Godot;
using System;

namespace GameJam
{
    public partial class TradersLabel : Label
    {
        private GlobalSignals _globalSignals { get; set; }
        private TraderManager _traderManager { get; set; }

		public override void _Ready()
		{
			if (GlobalSignals.Instance is null)
			{
				GD.PushError($"[{GetType().Name}]");
				return;
			}

            if (TraderManager.Instance is null)
            {
                GD.PushError($"[{GetType().Name}]");
                return;
            }

            _globalSignals = GlobalSignals.Instance;
            _traderManager = TraderManager.Instance;
            _globalSignals.NewTrader += UpdateTrader;
            _globalSignals.TraderUpdate += UpdateTrader;
            _globalSignals.BuyListing += UpdateTrader;
        }

        public void UpdateTrader(Trader trader)
        {
            Text = "\n";
            foreach (var tr in _traderManager.Traders)
            {
                Text += $"{tr.Name}\n";
                Text += $"${tr.Wealth}\n\n";
            }
        }

        public void UpdateTrader(Trader trader, Listing listing)
        {
            Text = "\n";
            foreach (var tr in _traderManager.Traders)
            {
                Text += $"{tr.Name}\n";
                Text += $"${tr.Wealth}\n\n";
            }
        }
    }
}
