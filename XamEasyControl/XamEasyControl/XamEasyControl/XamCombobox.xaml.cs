using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamEasyControl.Models;
using XamEasyControl.Common;
using FFImageLoading.Svg.Forms;
using System.Linq;

namespace XamEasyControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XamCombobox : ContentView
    {
	private static XamCombobox _thisView;
	private static Frame _mainFrame;
	private static StackLayout _mainStack;
	private static Label _placeHolder;
	private static StackLayout _selectedItemStack;
	private static StackLayout _sourceStack;

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
	public static readonly BindableProperty ExpandIconProperty = BindableProperty.Create(nameof(ExpandIcon), typeof(string), typeof(XamCombobox), "arrow_down.svg", propertyChanged: onExpandIconChanged);
	public static readonly BindableProperty CloseIconProperty = BindableProperty.Create(nameof(CloseIcon), typeof(string), typeof(XamCombobox), "close.svg", propertyChanged: onCloseIconChanged);

	public XamCombobox()
	{
	    InitializeComponent();

	    // main frame
	    _mainFrame = new Frame
	    {
		Padding = 1,
		BorderColor = BorderColor,
		CornerRadius = Corner,
		IsClippedToBounds = true,
		HasShadow = false
	    };
	    var mainFrameFocus = new TapGestureRecognizer();
	    mainFrameFocus.Tapped += OnFocus_Tapped;
	    _mainStack = new StackLayout { Spacing = 0 };
	    _mainStack.GestureRecognizers.Add(mainFrameFocus);
	    _mainFrame.Content = _mainStack;

	    // selected item stack
	    _selectedItemStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.StartAndExpand, Padding = ItemPadding / 2 };

	    // placeholder
	    var placeHolderStack = new StackLayout
	    {
		Orientation = StackOrientation.Horizontal
	    };
	    _placeHolder = new Label
	    {
		HorizontalOptions = LayoutOptions.StartAndExpand,
		Margin = ItemPadding * 1.5,
		Text = PlaceHolderText,
		FontSize = FontSize,
		TextColor = TextColor
	    };

	    placeHolderStack.Children.Add(_placeHolder);
	    placeHolderStack.Children.Add(_selectedItemStack);
	    placeHolderStack.Children.Add(new SvgCachedImage
	    {
		Source = ExpandIcon,
		Aspect = Aspect.AspectFit,
		HeightRequest = 15,
		WidthRequest = 20,
		Margin = new Thickness(4, 0, 4, 0),
		VerticalOptions = LayoutOptions.CenterAndExpand
	    });

	    // source stack
	    _sourceStack = new StackLayout
	    {
		IsVisible = false,
		Spacing = 0
	    };

	    _mainStack.Children.Add(placeHolderStack);
	    _mainStack.Children.Add(_sourceStack);

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

	private static void onCloseIconChanged(BindableObject bindable, object oldValue, object newValue)
	{
	}

	private static void onExpandIconChanged(BindableObject bindable, object oldValue, object newValue)
	{
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
	    _sourceStack.IsVisible = !_sourceStack.IsVisible;
	}

	private static void OnItemSelected_Tapped(object sender, EventArgs e)
	{
	    var view = sender as StackLayout;
	    string key = (e as TappedEventArgs).Parameter as string;

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
	    _placeHolder.IsVisible = _thisView.SelectedItems.IsNullOrEmpty();
	}

	private static void OnDeleteItemSelected_Tapped(object sender, EventArgs e)
	{
	    var key = (e as TappedEventArgs).Parameter as string;
	    _thisView.SelectedItems.Remove(key);

	    // 1: reload selected item
	    _selectedItemStack.Children.Remove(((sender as SvgCachedImage).Parent as StackLayout).Parent as Frame);

	    // 2: reload source item color
	    foreach(StackLayout item in _sourceStack.Children)
	    {
		if ((item.Children[1] as Label).Text.Equals(key))
		{
		    item.BackgroundColor = Color.White;
		}
	    }

	    _placeHolder.IsVisible = _thisView.SelectedItems.IsNullOrEmpty();
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

	    stack.Children.Add(new Frame
	    {
		Padding = 0,
		HeightRequest = 1,
		BackgroundColor = _thisView.BorderColor
	    });

	    stack.Children.Add(new Label
	    {
		Padding = _thisView.ItemPadding,
		Text = key,
		VerticalOptions = LayoutOptions.CenterAndExpand,
		VerticalTextAlignment = TextAlignment.Center,
		FontSize = _thisView.FontSize,
		TextColor = _thisView.TextColor
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
		HasShadow = false,
		Padding = _thisView.ItemPadding,
		CornerRadius = _thisView.Corner,
		BackgroundColor = _thisView.SelectedColor,
		VerticalOptions = LayoutOptions.CenterAndExpand,
		Content = content
	    };
	}
    }
}