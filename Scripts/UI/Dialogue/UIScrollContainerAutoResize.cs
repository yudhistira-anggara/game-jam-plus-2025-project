using Godot;
using System;

namespace GameJam
{
    [Tool]
    public partial class UIScrollContainerAutoResize : MarginContainer
    {
        private ScrollContainer _ScrollContainer;
        private MarginContainer _MarginContainer;
        private int _margin { get; set; }
        [Export]
        private int _height { get; set; } = 0;
        [Export]
        private int _maxOptions { get; set; } = 1;

        public override void _Ready()
        {
            _ScrollContainer = GetNodeOrNull<ScrollContainer>("ScrollContainer");
            _MarginContainer = this;

            if (_ScrollContainer == null || _MarginContainer == null)
                return;

            _margin = _MarginContainer.GetThemeConstant("margin_bottom");

            // GD.Print(_margin + _height);

            // SetContainerMaximumSize();
        }

        public void SetContainerMaximumSize()
        {
            if (_ScrollContainer == null || _MarginContainer == null)
                return;

            var childCount = _ScrollContainer.GetChild(0).GetChildCount();

            if (childCount > 0 && childCount <= _maxOptions)
            {
                var newY = _height + ((_height + _margin) * (childCount - 1));
                _ScrollContainer.CustomMinimumSize = new Vector2(0, newY);
                // GD.Print($"{_height}, {_margin}, {childCount}");
            }

            // GD.Print(childCount);

            // _ScrollContainer.CustomMinimumSize;
        }

        public override void _Process(double delta)
        {
            SetContainerMaximumSize();
        }

    }
}