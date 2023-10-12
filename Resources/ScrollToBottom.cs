using Godot;
using System;
using Range = Godot.Range;

public partial class ScrollToBottom : ScrollContainer
{
	private ScrollBar scrollbar;
	
	public override void _Ready()
	{
		scrollbar = GetVScrollBar();
		scrollbar.Changed += OnScrollbarChange;
	}

	private void OnScrollbarChange()
	{
		scrollbar.Value = scrollbar.MaxValue;
	}
}
