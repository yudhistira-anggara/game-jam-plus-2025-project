using Godot;
using System;
using System.Collections.Generic;

public partial class LineChart : Control
{
	[Export] public float LineWidth = 5f;
	[Export] public Color LineColor = Colors.White;
	[Export] public Color BgColor = new Color(0.1f, 0.1f, 0.1f, 1f);
	[Export] public string XLabelText = "";
	[Export] public string YLabelText = "";
	[Export] public int XTicks = 7;
	[Export] public int YTicks = 7;

	private bool xNumerical = true;
	private bool yNumerical = true;

	private float? minX, maxX, minY, maxY;

	private float lineRectWidth;
	private float lineRectHeight;
	private float lineRectX;
	private float lineRectY;

	private Line2D line;
	private Control lineContainer;
	private Label xLabel;
	private Label yLabel;
	private ColorRect background;
	private Container xTicksContainer;
	private Container yTicksContainer;

	// Sample data - you can expose this via [Export] if desired
	private List<Godot.Collections.Dictionary> data = new List<Godot.Collections.Dictionary>()
	{
		new Godot.Collections.Dictionary() { {"x", "MON"}, {"y", 7.0f} },
		new Godot.Collections.Dictionary() { {"x", "TUE"}, {"y", 8.0f} },
		new Godot.Collections.Dictionary() { {"x", "WED"}, {"y", 3.0f} },
		new Godot.Collections.Dictionary() { {"x", "THU"}, {"y", 5.0f} },
		new Godot.Collections.Dictionary() { {"x", "FRI"}, {"y", 4.0f} },
		new Godot.Collections.Dictionary() { {"x", "SAT"}, {"y", 6.0f} },
		new Godot.Collections.Dictionary() { {"x", "SUN"}, {"y", 1.0f} }
	};

	public override void _Ready()
	{
		// Get references (adjust node names/path if different)
		lineContainer = GetNode<Control>("LineContainer");
		xLabel = GetNode<Label>("XLabel");
		yLabel = GetNode<Label>("YLabel");
		background = lineContainer.GetNode<ColorRect>("Background");
		xTicksContainer = GetNode<Container>("XTicksContainer");
		yTicksContainer = GetNode<Container>("YTicksContainer");

		// Create and style the Line2D
		line = new Line2D();
		line.Width = LineWidth;
		line.DefaultColor = LineColor;
		line.Antialiased = true;
		lineContainer.AddChild(line);

		// Apply labels and background
		xLabel.Text = XLabelText;
		yLabel.Text = YLabelText;
		background.Color = BgColor;

		AnalyzeData();
		CalculateMinMax();
		GenerateTicks();
		DrawLineChart();

		// Wait one frame so layout is calculated
		CallDeferred(nameof(DeferredResizeAndRedraw));
	}

	private void DeferredResizeAndRedraw()
	{
		// Now rect_size is correct after children were resized
		lineRectWidth = lineContainer.RectSize.x;
		lineRectHeight = lineContainer.RectSize.y;

		if (lineRectWidth <= 0 || lineRectHeight <= 0) return;

		lineRectX = lineRectWidth / XTicks;
		lineRectY = lineRectHeight / YTicks;
		lineRectWidth = lineRectX * (XTicks - 1);
		lineRectHeight = lineRectY * (YTicks - 1);

		DrawLineChart();
	}

	private void AnalyzeData()
	{
		xNumerical = true;
		yNumerical = true;

		foreach (var point in data)
		{
			var xVal = point["x"];
			var yVal = point["y"];

			if (!(xVal is int || xVal is float || xVal is double))
				xNumerical = false;
			if (!(yVal is int || yVal is float || yVal is double))
				yNumerical = false;
		}
	}

	private void CalculateMinMax()
	{
		minX = null; maxX = null; minY = null; maxY = null;

		for (int i = 0; i < data.Count; i++)
		{
			float xVal = GetValue(data[i]["x"], i);
			float yVal = GetValue(data[i]["y"], i);

			if (!minX.HasValue || xVal < minX) minX = xVal;
			if (!maxX.HasValue || xVal > maxX) maxX = xVal;
			if (!minY.HasValue || yVal < minY) minY = yVal;
			if (!maxY.HasValue || yVal > maxY) maxY = yVal;
		}

		// Safety fallback
		if (!minX.HasValue) minX = 0;
		if (!maxX.HasValue) maxX = 1;
		if (!minY.HasValue) minY = 0;
		if (!maxY.HasValue) maxY = 1;
	}

	private void GenerateTicks()
	{
		// Clear previous ticks
		foreach (Node child in xTicksContainer.GetChildren())
			child.QueueFree();
		foreach (Node child in yTicksContainer.GetChildren())
			child.QueueFree();

		// X-axis ticks
		for (int i = 0; i < XTicks; i++)
		{
			var tick = new Label();
			tick.SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;
			tick.HorizontalAlignment = HorizontalAlignment.Center;

			if (xNumerical && i < data.Count)
			{
				float value = minX.Value + i * (maxX.Value - minX.Value) / (XTicks - 1);
				tick.Text = Mathf.Round(value).ToString();
			}
			else if (i < data.Count)
			{
				tick.Text = data[i]["x"].ToString();
			}
			xTicksContainer.AddChild(tick);
		}

		// Y-axis ticks (from bottom to top: high to low visually, but we reverse index)
		for (int i = YTicks - 1; i >= 0; i--)
		{
			var tick = new Label();
			tick.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
			tick.VerticalAlignment = VerticalAlignment.Center;

			if (yNumerical)
			{
				float value = minY.Value + i * (maxY.Value - minY.Value) / (YTicks - 1);
				tick.Text = Mathf.Round(value).ToString();
			}
			else
			{
				int dataIndex = YTicks - i - 1;
				if (dataIndex < data.Count)
					tick.Text = data[dataIndex]["y"].ToString();
			}
			yTicksContainer.AddChild(tick);
		}
	}

	private void DrawLineChart()
	{
		line.ClearPoints();

		for (int i = 0; i < data.Count; i++)
		{
			float xVal = GetValue(data[i]["x"], i);
			float yVal = GetValue(data[i]["y"], i);

			float scaledX = ScaleX(xVal);
			float scaledY = ScaleY(yVal);

			line.AddPoint(new Vector2(scaledX, scaledY));
		}
	}

	private float ScaleX(float val)
	{
		float dx = maxX.Value - minX.Value;
		if (dx == 0) return lineRectX / 2;
		return ((val - minX.Value) * lineRectWidth / dx) + lineRectX / 2;
	}

	private float ScaleY(float val)
	{
		float dy = maxY.Value - minY.Value;
		if (dy == 0) return lineRectHeight / 2;
		return lineRectHeight - ((val - minY.Value) * lineRectHeight / dy) + lineRectY / 2;
	}

	private float GetValue(Variant val, int index)
	{
		if (val.VariantType == Variant.Type.Int || 
			val.VariantType == Variant.Type.Float ||
			val.VariantType == Variant.Type.Real)
		{
			return (float)val;
		}
		return index; // fallback to index for categorical
	}

	// Optional: Redraw when resized
	public override void _Notification(int what)
	{
		if (what == NotificationResized)
		{
			CallDeferred(nameof(DeferredResizeAndRedraw));
		}
	}
}
