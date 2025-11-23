using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameJam
{
	public partial class Trader : Node
	{
		public int Index { get; set; }
		public string ID { get; set; }
		// public string Name { get; set; }
		public string Desc { get; set; }
		public int Wealth { get; set; }
		public int Income { get; set; }
		public int Activeness { get; set; }
		public Dictionary<string, int> Interests { get; set; } = [];
		public Dictionary<string, int> Personality { get; set; } = [];
		public List<string> Flags { get; set; } = [];

        public List<Listing> TradeHistory { get; set; } = [];

		public Trader() { }

		public Trader(TraderSerializeable t)
		{
			ID = t.ID;
			Name = t.Name;
			Desc = t.Desc;
			Wealth = t.Wealth;
			Income = t.Income;
			Activeness = t.Activeness;
			Interests = t.Interests;
			Personality = t.Personality;
			Flags = t.Flags;
		}

		public override void _Ready()
		{
			GlobalSignals.Instance.Refund += HandleRefund;
		}

        public bool CalculateWillingness(Listing list)
        {
            if (Wealth < list.PriceOffer)
                return false;

            var tr = TradeManager.Instance.Trades.Find(t => t.ID == list.Target.ID);
            var op = tr.Options.Find(op => op.Name == list.Target.Option);
            var ps = Personality;

            double tOdds = tr.Options.Sum(t => t.Odds);

            double relativeOdds = op.Odds / tOdds;
            double avgOdds = 1 / tr.Options.Count;
            double bestOpOdds = tr.Options.Max(t => t.Odds) / tOdds;

            double minFinalOdds = op.Trend * tr.Duration;
            double maxFinalOdds = Math.Abs(op.Trend) * tr.Duration;
            double expectedFinalOdds = op.Odds + GD.RandRange(minFinalOdds, maxFinalOdds);

            var potentialWinning = bestOpOdds * 100 * list.Shares;
            var potentialLosing = potentialWinning - list.PriceOffer;
            var gamble = Math.Clamp(potentialLosing / potentialWinning, 0.0, 1);

            var opinion = GD.RandRange(-100, 100);

            foreach (var t in Interests)
            {
                foreach (var tags in tr.Tags)
                {
                    if (t.Key == tags)
                        opinion += t.Value;
                }

                foreach (var tags in op.Tags)
                {
                    if (t.Key == tags)
                        opinion += t.Value;
                }
            }

            // Personality
            var lsTime = list.Duration;
            var tTime = tr.Duration;
            var fomo = ps.TryGetValue("Fomo", out int value) ? value : 15;

            double durMod = 1 - Math.Clamp(lsTime + tTime / fomo, 0.0, 0.5);

            var willing = ps.TryGetValue("Willingness", out value) ? value : 10;
            willing -= ps.TryGetValue("Caution", out value) ? value : 10;
            willing += opinion;

            var finalThoughts = ((double)willing / 100) + gamble;

            if (finalThoughts > 0)
            {
                return true;
            }
            else
                return false;
        }

		public void DecideAction(Listing list)
		{
			var act = (double)Activeness / 100;

            if (!(Random.Shared.NextDouble() < act))
                return;

            if (!CalculateWillingness(list))
                return;

            PurchaseListing(this, list);
        }

        public void PurchaseListing(Trader trader, Listing list)
        {
            var memory = Personality.TryGetValue("Memory", out int value) ? value : 1;

            if (TradeHistory.Count > memory)
                TradeHistory.RemoveAt(0);

            TradeHistory.Add(list);
            Wealth -= list.PriceOffer;

            GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.BuyListing, trader, list);
        }

        public void HandleRefund(TradeRequest request)
        {
            if (request.Requester != Name)
                return;
        }

		public void CreateRequest(Trade trade)
		{
			var Request = new TradeRequest()
			{
				Requester = ID,
				Target = trade.ID,
				Index = trade.Index
			};
			// CurrentTrades.Add(trade.ID, 5);
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.NewTradeRequest, Request);
		}
	}
}
