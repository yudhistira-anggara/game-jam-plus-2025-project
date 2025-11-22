using Godot;
using System;

namespace GameJam
{
    public partial class ListingTimerLabel : Label
    {
        private GlobalSignals _globalSignals { get; set; }
        private ListingManager _listingManager { get; set; }

        public override void _Ready()
        {
            if (GlobalSignals.Instance is null)
            {
                GD.PushError($"[{GetType().Name}]");
                return;
            }

            if (ListingManager.Instance is null)
            {
                GD.PushError($"[{GetType().Name}]");
                return;
            }

            _globalSignals = GlobalSignals.Instance;
            _listingManager = ListingManager.Instance;
            _globalSignals.DurationLeft += UpdateTimer;
        }

        public void UpdateTimer(double d)
        {
            Text = "";
            foreach (var ls in _listingManager.Listings)
            {
                var ts = TimeSpan.FromSeconds(ls.Duration);
                var st = ts.ToString(@"mm\:ss");
                Text += $"\n{st}";
            }
        }
    }
}