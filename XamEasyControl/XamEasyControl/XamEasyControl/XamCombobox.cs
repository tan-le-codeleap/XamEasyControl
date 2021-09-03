using AiForms.Layouts;
using FFImageLoading.Svg.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using XamEasyControl.Common;
using XamEasyControl.Models;

namespace XamEasyControl
{
    public class XamCombobox : ContentView
    {
	private static XamCombobox _thisView;
	private static Frame _mainFrame;
	private static Frame _placeHolderLine;
	private static StackLayout _mainStack;
	private static Label _placeHolder;
	private static WrapLayout _selectedItemStack;
	private static StackLayout _sourceStack;
	private static SvgCachedImage _expandIcon;

	public static readonly BindableProperty ModeProperty = BindableProperty.Create(nameof(Mode), typeof(ComboboxSelectionMode), typeof(XamCombobox), ComboboxSelectionMode.Multible);
	public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(Dictionary<string, string>), typeof(XamCombobox), null, propertyChanged: OnSourceChanged);
	public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(nameof(SelectedItems), typeof(Dictionary<string, string>), typeof(XamCombobox), new Dictionary<string, string>(), BindingMode.TwoWay);
	public static readonly BindableProperty OnUnFocusCommandProperty = BindableProperty.Create(nameof(OnUnFocusCommand), typeof(ICommand), typeof(XamCombobox), null);

	public static readonly BindableProperty PlaceHolderTextProperty = BindableProperty.Create(nameof(PlaceHolderText), typeof(string), typeof(XamCombobox), "Select from the list", propertyChanged: OnPlaceHolderTextChanged);
	public static readonly BindableProperty CornerProperty = BindableProperty.Create(nameof(Corner), typeof(int), typeof(XamCombobox), 0, propertyChanged: OnCornerChanged);
	public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(int), typeof(XamCombobox), 16, propertyChanged: OnFontSizeChanged);
	public static readonly BindableProperty ItemSpacingProperty = BindableProperty.Create(nameof(ItemSpacing), typeof(int), typeof(XamCombobox), 4, propertyChanged: OnItemSpacingChanged);
	public static readonly BindableProperty ItemPaddingProperty = BindableProperty.Create(nameof(ItemPadding), typeof(int), typeof(XamCombobox), 8, propertyChanged: OnItemPaddingChanged);
	public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(XamCombobox), Color.LightGray, propertyChanged: OnTextColorChanged);
	public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(XamCombobox), Color.LightGray, propertyChanged: OnBorderColorChanged);
	public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(XamCombobox), Color.FromHex("#F2F5FB"));
	public static readonly BindableProperty ExpandIconProperty = BindableProperty.Create(nameof(ExpandIcon), typeof(string), typeof(XamCombobox), "arrow_down.svg", propertyChanged: OnExpandIconChanged);
	public static readonly BindableProperty CloseIconProperty = BindableProperty.Create(nameof(CloseIcon), typeof(string), typeof(XamCombobox), "close.svg");
	public static readonly BindableProperty SeperatorVisibilityProperty = BindableProperty.Create(nameof(SeperatorVisibility), typeof(bool), typeof(XamCombobox), true);

	public XamCombobox()
	{
	    // main frame
	    _mainFrame = new Frame
	    {
		Padding = 0,
		BorderColor = BorderColor,
		CornerRadius = Corner,
		IsClippedToBounds = true,
		HasShadow = false
	    };

	    _placeHolder = new Label
	    {
		Margin = ItemPadding * 1.5,
		Text = PlaceHolderText,
		FontSize = FontSize,
		HorizontalOptions = LayoutOptions.StartAndExpand,
		TextColor = TextColor
	    };

	    _selectedItemStack = new WrapLayout
	    {
		IsVisible = false,
		Spacing = ItemSpacing,
		Margin = ItemPadding / 2
	    };

	    var focusTapped = new TapGestureRecognizer();
	    focusTapped.Tapped += OnFocus_Tapped;
	    var placeHolderStack = new StackLayout();
	    placeHolderStack.GestureRecognizers.Add(focusTapped);
	    placeHolderStack.Children.Add(_placeHolder);
	    placeHolderStack.Children.Add(_selectedItemStack);

	    var placeHolderGrib = new Grid
	    {
		RowDefinitions =
		{
		    new RowDefinition { Height = GridLength.Auto },
		},
		ColumnDefinitions =
		{
		    new ColumnDefinition { Width = GridLength.Star },
		    new ColumnDefinition { Width = 25 },
		}
	    };
	    placeHolderGrib.Children.Add(placeHolderStack, 0, 0);

	    _expandIcon = new SvgCachedImage
	    {
		Source = ExpandIcon,
		Aspect = Aspect.AspectFit,
		HeightRequest = 15,
		HorizontalOptions = LayoutOptions.CenterAndExpand,
		WidthRequest = 20,
		Margin = new Thickness(4, 0, 4, 0),
	    };
	    _expandIcon.GestureRecognizers.Add(focusTapped);
	    placeHolderGrib.Children.Add(_expandIcon, 1, 0);

	    _placeHolderLine = new Frame
	    {
		HeightRequest = 1,
		HasShadow = false,
		Padding = 0,
		BackgroundColor = BorderColor,
		IsVisible = false,
	    };

	    _sourceStack = new StackLayout
	    {
		IsVisible = false,
		Spacing = 0
	    };

	    _mainStack = new StackLayout { Spacing = 0 };

	    _mainStack.Children.Add(placeHolderGrib);
	    _mainStack.Children.Add(_placeHolderLine);
	    _mainStack.Children.Add(_sourceStack);

	    _mainFrame.Content = _mainStack;
	    Content = _mainFrame;
	}

	#region Properties
	public ICommand OnUnFocusCommand
	{
	    get { return (ICommand)GetValue(OnUnFocusCommandProperty); }
	    set { SetValue(OnUnFocusCommandProperty, value); }
	}

	public ComboboxSelectionMode Mode
	{
	    get { return (ComboboxSelectionMode)GetValue(ModeProperty); }
	    set { SetValue(ModeProperty, value); }
	}

	public int Corner
	{
	    get { return (int)GetValue(CornerProperty); }
	    set { SetValue(CornerProperty, value); }
	}

	public int FontSize
	{
	    get { return (int)GetValue(FontSizeProperty); }
	    set { SetValue(FontSizeProperty, value); }
	}

	public int ItemSpacing
	{
	    get { return (int)GetValue(ItemSpacingProperty); }
	    set { SetValue(ItemSpacingProperty, value); }
	}

	public int ItemPadding
	{
	    get { return (int)GetValue(ItemPaddingProperty); }
	    set { SetValue(ItemPaddingProperty, value); }
	}

	public Color TextColor
	{
	    get { return (Color)GetValue(TextColorProperty); }
	    set { SetValue(TextColorProperty, value); }
	}

	public Color SelectedColor
	{
	    get { return (Color)GetValue(SelectedColorProperty); }
	    set { SetValue(SelectedColorProperty, value); }
	}

	public string PlaceHolderText
	{
	    get { return (string)GetValue(PlaceHolderTextProperty); }
	    set { SetValue(PlaceHolderTextProperty, value); }
	}

	public Color BorderColor
	{
	    get { return (Color)GetValue(BorderColorProperty); }
	    set { SetValue(BorderColorProperty, value); }
	}

	public string ExpandIcon
	{
	    get { return (string)GetValue(ExpandIconProperty); }
	    set { SetValue(ExpandIconProperty, value); }
	}

	public string CloseIcon
	{
	    get { return (string)GetValue(CloseIconProperty); }
	    set { SetValue(CloseIconProperty, value); }
	}

	public bool SeperatorVisibility
	{
	    get { return (bool)GetValue(SeperatorVisibilityProperty); }
	    set { SetValue(SeperatorVisibilityProperty, value); }
	}

	public Dictionary<string, string> Source
	{
	    get { return (Dictionary<string, string>)GetValue(SourceProperty); }
	    set { SetValue(SourceProperty, value); }
	}

	public Dictionary<string, string> SelectedItems
	{
	    get { return (Dictionary<string, string>)GetValue(SelectedItemsProperty); }
	    set { SetValue(SelectedItemsProperty, value); }
	}
	#endregion

	#region binding event
	private static void OnExpandIconChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    _expandIcon.Source = (string)newValue;
	}

	private static void OnItemPaddingChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    _selectedItemStack.Padding = (int)newValue;
	}

	private static void OnItemSpacingChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    _selectedItemStack.Spacing = (int)newValue;
	}

	private static void OnCornerChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    _mainFrame.CornerRadius = (int)newValue;
	}

	private static void OnFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    _placeHolder.FontSize = (int)newValue;
	}

	private static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    _placeHolder.TextColor = (Color)newValue;
	}

	private static void OnBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    _mainFrame.BorderColor = (Color)newValue;
	}

	private static void OnPlaceHolderTextChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    _placeHolder.Text = (string)newValue;
	}

	#endregion

	private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    var source = newValue as Dictionary<string, string>;

	    if (source.IsNullOrEmpty())
	    {
		return;
	    }
	    _thisView = bindable as XamCombobox;

	    foreach (var item in source.Keys)
	    {
		_sourceStack.Children.Add(CreateSourceItem(item));
	    }
	}

	private void OnFocus_Tapped(object sender, EventArgs e)
	{
	    Device.BeginInvokeOnMainThread(() =>
	    {
		_sourceStack.IsVisible = !_sourceStack.IsVisible;
		_placeHolderLine.IsVisible = _sourceStack.IsVisible;
	    });
	}

	private static void OnItemSelected_Tapped(object sender, EventArgs e)
	{
	    var view = sender as StackLayout;
	    string key = (e as TappedEventArgs).Parameter as string;

	    if (_thisView.Mode == ComboboxSelectionMode.Multible)
	    {
		if (!_thisView.SelectedItems.IsNullOrEmpty() && _thisView.SelectedItems.ContainsKey(key))
		{
		    view.BackgroundColor = Color.White;
		    _thisView.SelectedItems.Remove(key);

		    var removeItem = _selectedItemStack.Children.FirstOrDefault(p => (((p as Frame).Content as StackLayout).Children[0] as Label).Text.Equals(key));
		    if (removeItem != null)
		    {
			_selectedItemStack.Children.Remove(removeItem);
		    }
		}
		else
		{
		    view.BackgroundColor = _thisView.SelectedColor;
		    _thisView.SelectedItems.Add(key, _thisView.Source[key]);

		    _selectedItemStack.Children.Add(CreateTagItem(key));
		}

		// update ui on selected stack
		Device.BeginInvokeOnMainThread(() =>
		{
		    _placeHolder.IsVisible = _thisView.SelectedItems.IsNullOrEmpty();
		    _selectedItemStack.IsVisible = !_thisView.SelectedItems.IsNullOrEmpty();
		});
	    }
	    else
	    {
		_thisView.SelectedItems = new Dictionary<string, string>
		{
		    { key, _thisView.Source[key] }
		};

		// 1. update selected color source list
		foreach (StackLayout item in _sourceStack.Children)
		{
		    if ((item.Children[0] as Label).Text.Equals(key))
		    {
			item.BackgroundColor = _thisView.SelectedColor;
		    }
		    else
		    {
			item.BackgroundColor = Color.White;
		    }
		}

		// 2. Update seletced item tag
		_placeHolder.Text = key;
	    }
	}

	private static void OnDeleteItemSelected_Tapped(object sender, EventArgs e)
	{
	    var key = (e as TappedEventArgs).Parameter as string;
	    _thisView.SelectedItems.Remove(key);

	    // 1: reload selected item
	    _selectedItemStack.Children.Remove(((sender as SvgCachedImage).Parent as StackLayout).Parent as Frame);

	    // 2: reload source item color
	    foreach (StackLayout item in _sourceStack.Children)
	    {
		if ((item.Children[0] as Label).Text.Equals(key))
		{
		    item.BackgroundColor = Color.White;
		}
	    }

	    Device.BeginInvokeOnMainThread(() =>
	    {
		_selectedItemStack.IsVisible = !_thisView.SelectedItems.IsNullOrEmpty();
		_placeHolder.IsVisible = _thisView.SelectedItems.IsNullOrEmpty();
	    });
	}

	private static StackLayout CreateSourceItem(string key)
	{
	    var itemSelected = new TapGestureRecognizer
	    {
		CommandParameter = key
	    };
	    itemSelected.Tapped += OnItemSelected_Tapped;

	    var stack = new StackLayout
	    {
		Spacing = 0,
		BackgroundColor = _thisView.SelectedItems.ContainsKey(key) ? _thisView.SelectedColor : Color.White
	    };

	    stack.GestureRecognizers.Add(itemSelected);

	    stack.Children.Add(new Label
	    {
		Padding = _thisView.ItemPadding,
		Text = key,
		VerticalOptions = LayoutOptions.CenterAndExpand,
		VerticalTextAlignment = TextAlignment.Center,
		FontSize = _thisView.FontSize,
		TextColor = _thisView.TextColor
	    });

	    stack.Children.Add(new Frame
	    {
		IsVisible = _thisView.SeperatorVisibility,
		Padding = 0,
		HeightRequest = 1,
		BackgroundColor = _thisView.BorderColor
	    });

	    return stack;
	}

	private static Frame CreateTagItem(string key)
	{
	    var content = new StackLayout
	    {
		Orientation = StackOrientation.Horizontal,
		Spacing = 10
	    };
	    content.Children.Add(new Label { Text = key, FontSize = _thisView.FontSize, TextColor = _thisView.TextColor });

	    var deleteItemSelected = new TapGestureRecognizer
	    {
		CommandParameter = key
	    };
	    deleteItemSelected.Tapped += OnDeleteItemSelected_Tapped;
	    var closeItem = new SvgCachedImage
	    {
		Source = _thisView.CloseIcon,
		Aspect = Aspect.AspectFit,
		HeightRequest = 15,
		WidthRequest = 15,
		VerticalOptions = LayoutOptions.CenterAndExpand
	    };
	    closeItem.GestureRecognizers.Add(deleteItemSelected);
	    content.Children.Add(closeItem);

	    return new Frame
	    {
		Padding = _thisView.ItemPadding,
		CornerRadius = _thisView.Corner,
		BackgroundColor = _thisView.SelectedColor,
		VerticalOptions = LayoutOptions.CenterAndExpand,
		IsClippedToBounds = true,
		HasShadow = false,
		Content = content
	    };
	}
    }
}