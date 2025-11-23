using Godot;
using System;

namespace GameJam
{
    public partial class ListingLabel : Label
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
            _globalSignals.NewListing += UpdateListing;
            _globalSignals.KillListing += UpdateListing;
            _globalSignals.TestSignal += ClearLabel;
        }

        public void ClearLabel()
        {
            Text = "";
        }

        public void UpdateListing(Listing ls)
        {
            Text = "";
            foreach (var lis in _listingManager.Listings)
            {
                Text += $"\n{lis.Index}.{lis.Target.ID} [{lis.Target.Option}] -> Shares: {lis.Shares}, Offer: ${lis.PriceOffer}";
            }
        }
    }
}