using Godot;
using System;

namespace GameJam
{
	public partial class TradeContainer : MarginContainer
	{
		public Trade Trade { get; set; }
		public void SetTrade(Trade trade)
		{
			Trade = trade;
		}
	}
}
