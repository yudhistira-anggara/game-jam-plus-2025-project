using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace GameJam
{
	public partial class TradesGridContainer : GridContainer
	{
		private GlobalSignals _globalSignals { get; set; }
		private TradeManager _tradeManager { get; set; }
		private PackedScene _yesNoContainer { get; set; }
		private PackedScene _sportContainer { get; set; }
		private PackedScene _listingContainer { get; set; }
		private List<TradeContainer> _tradeContainers { get; set; } = new List<TradeContainer>();
		private List<ListingContainer> _listingContainers { get; set; } = new List<ListingContainer>();
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

			_yesNoContainer = GD.Load<PackedScene>("res://Resources/UI/yes_no_container.tscn");
			_sportContainer = GD.Load<PackedScene>("res://Resources/UI/sport_container.tscn");
			_listingContainer = GD.Load<PackedScene>("res://Resources/UI/listing_container.tscn");

			_globalSignals = GlobalSignals.Instance;
			_tradeManager = TradeManager.Instance;
			_globalSignals.NewTrade += CreateTrade;
			_globalSignals.ResolveTrade += RemoveTrade;
			_globalSignals.TradeModified += UpdateTrade;

			_listingManager = ListingManager.Instance;
			_globalSignals.NewListing += CreateListing;
			_globalSignals.KillListing += DestroyListing;
			//_globalSignals.TestSignal += ClearLabel;
		}
		public override void _Process(double delta)
		{
			//for (int i = 0; i < _tradeManager.Trades.Count; i++)
			//{
			//	var tr = _tradeManager.Trades[i];
			//	var tradeContainer = _tradeContainers[i];
			//}
			//var wt = _gameManagerTimer.WaitTime;
			//var tl = _gameManagerTimer.TimeLeft;

			//var p = 1 - (wt - tl) / wt;
			//Value = p;
		}
		private async void NullCheck()
		{
			foreach (var panel in this.GetChildren())
			{
				if (panel.GetChildCount() == 0)
				{
				}
			}
			await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
		}
		public void CreateTrade(Trade trade)
		{
			NullCheck();
			if (trade.Tags.Contains("Sports") && _sportContainer is not null)
			{
				var instancedContainer = _sportContainer.Instantiate() as TradeContainer;
				CreateContainer(instancedContainer, trade);
			}
			else
			{
				var instancedContainer = _yesNoContainer.Instantiate() as TradeContainer;
				CreateContainer(instancedContainer, trade);
			}
		}

		private void CreateContainer(TradeContainer container, Trade trade)
		{
			container.SetIndex(trade.Index);
			GetEmptyPanel()?.AddChild(container);
			_tradeContainers.Add(container);
			ProgressBar progressBar = container.GetNode<ProgressBar>("Node2D/PanelContainer/ProgressBar");
			progressBar.MaxValue = trade.Duration;
			container.GetNode<PanelShake>("Node2D").SpawnEffect();
			UpdateTrade(trade);
		}
		private Node GetEmptyPanel()
		{
			foreach (var panel in this.GetChildren())
			{
				if (panel.GetChildCount() == 0)
				{
					return panel;
				}
			}
			return null;
		}
		private void UpdateTrade(Trade trade)
		{
			foreach (var tradeContainer in _tradeContainers)
			{
				var tr = tradeContainer.Trade;
				Node vBoxContainer = tradeContainer.GetNode<Node>("Node2D/PanelContainer/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer");
				if (tr.Tags.Contains("Sports") && _sportContainer is not null)
				{
					Node total = vBoxContainer.GetNode<Node>("../../HBoxContainer/Total");
					total.Set("text", $"$ {tr.Options[0].Shares}");
					Node teamButton = vBoxContainer.GetNode<Node>("HBoxContainer/TeamButton");
					Node teamButton2 = vBoxContainer.GetNode<Node>("HBoxContainer/TeamButton2");
					teamButton.Set("text", tr.Options[0].Option);
					teamButton2.Set("text", tr.Options[1].Option);
				}
				else
				{
					Node foldableContainer = vBoxContainer.GetNode<Node>("FoldableContainer");
					foldableContainer.Set("title", tr.Title);

					Node total = vBoxContainer.GetNode<Node>("../../Total");
					total.Set("text", $"$ {tr.Options[0].Shares}");
				}

				ProgressBar progressBar = tradeContainer.GetNode<ProgressBar>("Node2D/PanelContainer/ProgressBar");
				progressBar.Value = tr.Duration;
				Node panelContainer = vBoxContainer.GetNode<Node>("PanelContainer");
				Node hBoxContainer = panelContainer.GetNode<Node>("HBoxContainer");
				Node Option = hBoxContainer.GetNode<Node>("Option");
				Label Odds = hBoxContainer.GetNode<Label>("Odds");

				Node panelContainer2 = vBoxContainer.GetNode<Node>("PanelContainer2");
				Node hBoxContainer2 = panelContainer2.GetNode<Node>("HBoxContainer");
				Node Option2 = hBoxContainer2.GetNode<Node>("Option");
				Label Odds2 = hBoxContainer2.GetNode<Label>("Odds");

				Option.Set("text", tr.Options[0].Option);
				Option2.Set("text", tr.Options[1].Option);
				if (tr.Options[0].Odds < 50)
				{
					Odds.AddThemeColorOverride("font_color", new Color(1, 0, 0));
					Odds2.AddThemeColorOverride("font_color", new Color(0, 1, 0));
				}
				else
				{
					Odds.AddThemeColorOverride("font_color", new Color(0, 1, 0));
					Odds2.AddThemeColorOverride("font_color", new Color(1, 0, 0));
				}
				Odds.Set("text", tr.Options[0].Odds.ToString() + "%");
				Odds2.Set("text", tr.Options[1].Odds.ToString() + "%");
			}
		}

		public async void RemoveTrade(Trade trade, TradeOption option)
		{
			foreach (var tradeContainer in _tradeContainers)
			{
				if (tradeContainer.Trade == trade)
				{
					_tradeContainers.Remove(tradeContainer);
					tradeContainer.GetNode<PanelShake>("Node2D").PlayExplosion();
					break;
				}
			}
		}
		private void CreateListing(Listing ls)
		{
			var instancedContainer = _listingContainer.Instantiate() as ListingContainer;
			CreateContainer(instancedContainer, ls);
		}
		private void CreateContainer(ListingContainer container, Listing ls)
		{
			foreach (var panel in this.GetChildren())
			{
				if (panel.GetChildCount() > 0)
				{
					Debug.Print(panel.GetChildren()[0].Name);
					if (panel.GetChildren()[0].Name == "Hi " + $"{ls.Index}")
					{

						VBoxContainer vBoxContainer = panel.GetChildren()[0].GetNode<VBoxContainer>("Node2D/PanelContainer/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/FoldableContainer/VBoxContainer");
						vBoxContainer.AddChild(container);
						_listingContainers.Add(container);
						container.MaxValue = ls.Duration;
						//UpdateListing(ls);
					}
				}
			}
			//UpdateTrade(trade);
		}/*
		private void UpdateListing(Listing ls)
		{
			var lis = _listingManager.Listings[i];
			var listingBar = _listingContainers[i];

			listingBar.Value = lis.Duration;
			Debug.Print("hi");
			Debug.Print(lis.Duration.ToString());
			Button button = listingBar.GetNode<Button>("Button");
			button.Set("text", $"{lis.Index}.{lis.Target.ID} [{lis.Target.Option}] -> Shares: {lis.Shares}, Offer: ${lis.PriceOffer}");


			//Text += $"\n{lis.Index}.{lis.Target.ID} [{lis.Target.Option}] -> Shares: {lis.Shares}, Offer: ${lis.PriceOffer}";

		}*/
		private void DestroyListing(Listing ls)
		{
			foreach (var listingBar in _listingContainers)
			{
				if (listingBar is not null)
				{
					if (listingBar.Listing == ls)
					{
						_listingContainers.Remove(listingBar);
						listingBar.QueueFree();
						break;
					}
				}
			}
		}
	}
	public partial class TradeContainer : MarginContainer
	{
		public Trade Trade { get; set; }
		public int Index { get; set; }
		public void SetIndex(int index)
		{
			Index = index;
		}
	}
	public partial class ListingContainer : ProgressBar
	{
		public Listing Listing { get; set; }
	}
}
