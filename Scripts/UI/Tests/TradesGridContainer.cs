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
		private List<ListingContainer> _garbageContainers { get; set; } = new List<ListingContainer>();

		public override void _Ready()
		{
			if (GlobalSignals.Instance is null)
			{
				GD.PushError($"[{GetType().Name}]");
				return;
			}

			if (TradeManager.Instance is null)
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
			_globalSignals.AddListing += CreateListing;
			_globalSignals.KillListing += DestroyListing;
		}

		/*
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
		*/

		private async void NullCheck()
		{
			foreach (var panel in this.GetChildren())
			{
				if (panel.GetChildCount() == 0)
				{
				}
			}
			await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
			NullCheck();
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
			if (container is null)
			{
				GD.PushError($"[{GetType().Name}]");
				return;
			}
			container.SetTrade(trade);
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
				if (tradeContainer.Trade == trade)
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

					VBoxContainer vBoxListingContainer = vBoxContainer.GetNode<VBoxContainer>("FoldableContainer/VBoxContainer");
					Label notification = tradeContainer.GetNode<Label>("Node2D/Notification");
					notification.Text = $"{vBoxListingContainer.GetChildCount()}";

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
			UpdateListingContainer();
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

		private void CreateListing(Trade tr, Listing ls)
		{
			var instancedContainer = _listingContainer.Instantiate() as ListingContainer;
			CreateListingContainer(instancedContainer, ls, tr);
		}
		
		private void CreateListingContainer(ListingContainer container, Listing ls, Trade trade)
		{
			if (container.GetParent() == null)
			{
				foreach (var panel in this.GetChildren())
				{
					if (panel.GetChildCount() > 0)
					{
						if (panel.GetChildren()[0] is TradeContainer)
						{
							var tradeContainer = panel.GetChildren()[0] as TradeContainer;

							// Check if the casting succeeded and the TradeContainer is not null
							if (tradeContainer != null)
							{
								// Now you can safely access the Trade property
								if (tradeContainer.Trade == trade)
								{
									VBoxContainer vBoxContainer = tradeContainer.GetNode<VBoxContainer>("Node2D/PanelContainer/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/FoldableContainer/VBoxContainer");
									container.SetListing(ls);
									vBoxContainer.AddChild(container);
									_listingContainers.Add(container);
									container.MaxValue = ls.Duration;
									UpdateListingContainer();
									UpdateTrade(trade);
								}
							}
						}
					}
					//UpdateTrade(ls);
				}
			}
		}
		/*
		private void UpdateListing(Listing ls)
		{
			foreach (var listingBar in _listingContainers)
			{
				if (listingBar is not null)
				{
					if (listingBar.Listing == ls)
					{
						var ts = TimeSpan.FromSeconds(ls.Duration);
						var st = ts.ToString(@"mm\:ss");
						listingBar.Value = ls.Duration;
						Button button = listingBar.GetNode<Button>("Button");
						button.Set("text", $"{ls.Index}.{st} [{ls.Target.Option}] -> Shares: {ls.Shares}, Offer: ${ls.PriceOffer}");

						break;
					}
				}
			}
		}
		*/

		private void UpdateListingContainer()
		{
			foreach (var listingBar in _listingContainers)
			{
				if (listingBar is not null)
				{
					if (_listingManager.Listings.Contains(listingBar.Listing))
					{
						var ls = listingBar.Listing;
						listingBar.Value = ls.Duration;
						Button button = listingBar.GetNode<Button>("Button");
						var ts = TimeSpan.FromSeconds(ls.Duration);
						var st = ts.ToString(@"mm\:ss");
						button.Set("text", $"{ls.Index}.{st} [{ls.TargetOption}] -> Shares: {ls.Shares}, Offer: ${ls.PriceOffer}");
					}
				}
			}
			CollectGarbage();
		}

		private void CollectGarbage()
		{
			foreach (var listingBar in _listingContainers)
			{
				if (listingBar is not null)
				{
					if (!_listingManager.Listings.Contains(listingBar.Listing))
					{
						_garbageContainers.Add(listingBar);
					}
				}
			}
			CleanGarbage();
		}

		private void CleanGarbage()
		{
			foreach (var listingBar in _garbageContainers)
			{
				if (listingBar is not null)
				{
					DestroyListing(listingBar.Listing);
				}
			}
			_garbageContainers.Clear();
		}

		private void DestroyListing(Listing ls)
		{
			foreach (var listingBar in _listingContainers)
			{
				if (listingBar is not null)
				{
					if (listingBar.Listing == ls)
					{
						listingBar.GetParent().RemoveChild(listingBar);
						_listingContainers.Remove(listingBar);
						listingBar.QueueFree();
						break;
					}
				}
			}
		}
		
		/*
		private void DestroyListing(Listing ls, int index)
		{
			var listingBar = _listingContainers[index];
				if (listingBar is not null)
				{
					if (listingBar.Listing == ls)
					{
						listingBar.GetParent().RemoveChild(listingBar);
						_listingContainers.Remove(listingBar);
						listingBar.QueueFree();
					}
				}

		}

		private void DestroyListingBar(ListingContainer listingBar)
		{
			if (listingBar is not null)
			{
				listingBar.GetParent().RemoveChild(listingBar);
				_listingContainers.Remove(listingBar);
				listingBar.QueueFree();
			}
		}
		*/
	}
}
