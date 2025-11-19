using Godot;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using GameJam;

namespace GameJam
{
	public partial class DialoguePanel : Control
	{
		/*
		[Export] private RichTextLabel LineLabel;
		[Export] private Button NextButton;

		[Export] public float TypeSpeed = 0.03f;

		private DialogueSystem _controller;
		private bool _isTyping = false;
		private string _visibleText = "";
		private string _rawText = "";

		public override void _Ready()
		{
			_controller = GetParent().GetNode<DialogueSystem>("DialogueSystem");

			_controller.LineChanged += OnLineChanged;
			_controller.DialogueFinished += OnFinished;

			NextButton.Pressed += () =>
			{
				if (_isTyping)
				{
					_isTyping = false;    // skip typing + pauses
					return;
				}

				_controller.Next();
			};
		}

		private async void OnLineChanged(string text)
		{
			await ShowTypewriter(text);
		}

		private async Task ShowTypewriter(string input)
		{
			_isTyping = true;
			LineLabel.Text = "";
			_visibleText = "";
			_rawText = input;

			var tokens = TokenizeRichText(input);

			for (int i = 0; i < tokens.Count; i++)
			{
				if (!_isTyping)
				{
					// Skip instantly to full text
					LineLabel.Text = _rawText;
					LineLabel.VisibleCharacters = -1;
					return;
				}

				var t = tokens[i];

				if (t.Type == TokenType.Text)
				{
					for (int c = 0; c < t.Content.Length; c++)
					{
						_visibleText += t.Content[c];
						LineLabel.Text = _visibleText;
						LineLabel.VisibleCharacters = -1;

						await Task.Delay((int)(TypeSpeed * 1000));

						if (!_isTyping)
							break;
					}
				}
				else if (t.Type == TokenType.Tag)
				{
					// Insert tag immediately; make sure RichText stays valid
					_visibleText += t.Content;
					LineLabel.Text = _visibleText;
					LineLabel.VisibleCharacters = -1;
				}
				else if (t.Type == TokenType.Wait)
				{
					double durationSeconds = t.WaitTime;
					ulong start = Time.GetTicksMsec();
					// compute milliseconds as rounded ulong
					ulong ms = (ulong)Math.Round(durationSeconds * 1000.0);
					ulong target = start + ms;

					// debug: uncomment to log parsed values
					// GD.Print($"[Dialogue] wait token parsed: {durationSeconds} s -> {ms} ms");

					while (Time.GetTicksMsec() < target)
					{
						if (!_isTyping)
							break;

						await Task.Yield();
					}
				}
			}

			_isTyping = false;
		}

		private void OnFinished()
		{
			Visible = false;
		}

		// ---------------------------
		// Tokenizer for BBCode + waits
		// ---------------------------

		private enum TokenType { Text, Tag, Wait }

		private class Token
		{
			public TokenType Type;
			public string Content;
			public double WaitTime;
		}

		private System.Collections.Generic.List<Token> TokenizeRichText(string input)
		{
			var tokens = new System.Collections.Generic.List<Token>();

			var bbcodeTag = new Regex(@"\[.+?\]");
			var waitTag = new Regex(@"<wait=([\d.]+)>");

			int index = 0;

			while (index < input.Length)
			{
				Match bb = bbcodeTag.Match(input, index);
				Match wt = waitTag.Match(input, index);

				int next = NextLowest(bb.Success ? bb.Index : int.MaxValue,
									  wt.Success ? wt.Index : int.MaxValue,
									  input.Length);

				// Text before next tag
				if (next > index)
				{
					tokens.Add(new Token
					{
						Type = TokenType.Text,
						Content = input.Substring(index, next - index)
					});
				}

				if (bb.Success && bb.Index == next)
				{
					tokens.Add(new Token
					{
						Type = TokenType.Tag,
						Content = bb.Value
					});
					index = bb.Index + bb.Length;
				}
				else if (wt.Success && wt.Index == next)
				{
					tokens.Add(new Token
					{
						Type = TokenType.Wait,
						Content = wt.Value,
						WaitTime = double.Parse(wt.Groups[1].Value, CultureInfo.InvariantCulture)
					});
					index = wt.Index + wt.Length;
				}
				else
				{
					break;
				}
			}

			return tokens;
		}

		private int NextLowest(int a, int b, int max)
		{
			int m = Math.Min(a, b);
			return m == int.MaxValue ? max : m;
		}
		*/
	}
}