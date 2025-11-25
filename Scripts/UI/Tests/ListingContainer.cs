using Godot;
using System;

namespace GameJam
{
	public partial class ListingContainer : ProgressBar
	{
		public Listing Listing { get; set; }
		public void SetListing(Listing listing)
		{
			Listing = listing;
		}
	}
}
