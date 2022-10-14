using NStack;
using Terminal.Gui;

namespace SahneeBotAdminConsole;

public class ConsoleOverviewGui
{
	public void Setup()
	{
		var Win = new Window ($"SAHNEE BOT") {
			X = 0,
			Y = 0,
			Width = Dim.Fill (),
			Height = Dim.Fill (),
		};
		Application.Top.Add (Win);
		var items = new List<ustring>();
		items.Add("item 1");
		items.Add("item 2");
		items.Add("item 3");
		items.Add("item 4");

		// ListView
		var lbListView = new Label("Listview")
		{
			ColorScheme = Colors.TopLevel,
			X = 0,
			Width = Dim.Percent(40)
		};

		var listview = new ListView(items)
		{
			X = 0,
			Y = Pos.Bottom(lbListView) + 1,
			Height = Dim.Fill(2),
			Width = Dim.Percent(40)
		};
		listview.SelectedItemChanged += (ListViewItemEventArgs e) => lbListView.Text = items[listview.SelectedItem];
		Win.Add(lbListView, listview);

		var _scrollBar = new ScrollBarView(listview, true);

		_scrollBar.ChangedPosition += () =>
		{
			listview.TopItem = _scrollBar.Position;
			if (listview.TopItem != _scrollBar.Position)
			{
				_scrollBar.Position = listview.TopItem;
			}

			listview.SetNeedsDisplay();
		};

		_scrollBar.OtherScrollBarView.ChangedPosition += () =>
		{
			listview.LeftItem = _scrollBar.OtherScrollBarView.Position;
			if (listview.LeftItem != _scrollBar.OtherScrollBarView.Position)
			{
				_scrollBar.OtherScrollBarView.Position = listview.LeftItem;
			}

			listview.SetNeedsDisplay();
		};

		listview.DrawContent += (e) =>
		{
			_scrollBar.Size = listview.Source.Count - 1;
			_scrollBar.Position = listview.TopItem;
			_scrollBar.OtherScrollBarView.Size = listview.Maxlength - 1;
			_scrollBar.OtherScrollBarView.Position = listview.LeftItem;
			_scrollBar.Refresh();
		};

		// ComboBox
		var lbComboBox = new Label("ComboBox")
		{
			ColorScheme = Colors.TopLevel,
			X = Pos.Right(lbListView) + 1,
			Width = Dim.Percent(40)
		};

		var comboBox = new ComboBox()
		{
			X = Pos.Right(listview) + 1,
			Y = Pos.Bottom(lbListView) + 1,
			Height = Dim.Fill(2),
			Width = Dim.Percent(40)
		};
		comboBox.SetSource(items);

		comboBox.SelectedItemChanged += (ListViewItemEventArgs text) => lbComboBox.Text = text.Value.ToString();
		Win.Add(lbComboBox, comboBox);

		var scrollBarCbx = new ScrollBarView(comboBox.Subviews[1], true);

		scrollBarCbx.ChangedPosition += () =>
		{
			((ListView)comboBox.Subviews[1]).TopItem = scrollBarCbx.Position;
			if (((ListView)comboBox.Subviews[1]).TopItem != scrollBarCbx.Position)
			{
				scrollBarCbx.Position = ((ListView)comboBox.Subviews[1]).TopItem;
			}

			comboBox.SetNeedsDisplay();
		};

		scrollBarCbx.OtherScrollBarView.ChangedPosition += () =>
		{
			((ListView)comboBox.Subviews[1]).LeftItem = scrollBarCbx.OtherScrollBarView.Position;
			if (((ListView)comboBox.Subviews[1]).LeftItem != scrollBarCbx.OtherScrollBarView.Position)
			{
				scrollBarCbx.OtherScrollBarView.Position = ((ListView)comboBox.Subviews[1]).LeftItem;
			}

			comboBox.SetNeedsDisplay();
		};

		comboBox.DrawContent += (e) =>
		{
			scrollBarCbx.Size = comboBox.Source.Count;
			scrollBarCbx.Position = ((ListView)comboBox.Subviews[1]).TopItem;
			scrollBarCbx.OtherScrollBarView.Size = ((ListView)comboBox.Subviews[1]).Maxlength - 1;
			scrollBarCbx.OtherScrollBarView.Position = ((ListView)comboBox.Subviews[1]).LeftItem;
			scrollBarCbx.Refresh();
		};


		var btnMoveUp = new Button("Move _Up")
		{
			X = 1,
			Y = Pos.Bottom(lbListView),
		};
		btnMoveUp.Clicked += () => { listview.MoveUp(); };

		var btnMoveDown = new Button("Move _Down")
		{
			X = Pos.Right(btnMoveUp) + 1,
			Y = Pos.Bottom(lbListView),
		};
		btnMoveDown.Clicked += () => { listview.MoveDown(); };

		Win.Add(btnMoveUp, btnMoveDown);
		Application.Run();
	}
}