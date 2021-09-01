using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamEasyControl.Models;
using XamEasyControl.Common;

namespace XamEasyControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XamCombobox : ContentView
    {
	public static readonly BindableProperty ModeProperty = BindableProperty.Create(nameof(Mode), typeof(ComboboxSelectionMode), typeof(XamCombobox), ComboboxSelectionMode.Multible);
	public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(Dictionary<string, string>), typeof(XamCombobox), null, propertyChanged: OnSourceChanged);
	public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(nameof(SelectedItems), typeof(Dictionary<string, string>), typeof(XamCombobox), new Dictionary<string, string>(), BindingMode.TwoWay, propertyChanged: OnSelectedItemsChanged);
	public static readonly BindableProperty OnUnFocusCommandProperty = BindableProperty.Create(nameof(OnUnFocusCommand), typeof(ICommand), typeof(XamCombobox), null);

	public static readonly BindableProperty PlaceHolderTextProperty = BindableProperty.Create(nameof(PlaceHolderText), typeof(string), typeof(XamCombobox), null, propertyChanged: OnPlaceHolderTextChanged);
	public static readonly BindableProperty CornerProperty = BindableProperty.Create(nameof(Corner), typeof(int), typeof(XamCombobox), 0, propertyChanged: OnCornerChanged);
	public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(int), typeof(XamCombobox), 16, propertyChanged: OnFontSizeChanged);
	public static readonly BindableProperty ItemSpacingProperty = BindableProperty.Create(nameof(ItemSpacing), typeof(int), typeof(XamCombobox), 4, propertyChanged: OnItemSpacingChanged);
	public static readonly BindableProperty ItemPaddingProperty = BindableProperty.Create(nameof(ItemPadding), typeof(int), typeof(XamCombobox), 4, propertyChanged: OnItemPaddingChanged);
	public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(XamCombobox), Color.LightGray, propertyChanged: OnTextColorChanged);
	public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(XamCombobox), Color.LightGray, propertyChanged: OnBorderColorChanged);
	public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(XamCombobox), Color.FromHex("#F2F5FB"));
	public static readonly BindableProperty ExpandIconProperty = BindableProperty.Create(nameof(ExpandIcon), typeof(string), typeof(XamCombobox), "arrow_down.svg", propertyChanged: onExpandIconChanged);

	public XamCombobox()
	{
	    InitializeComponent();
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

	private static void OnItemPaddingChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    (bindable as XamCombobox).BorderFrame.Padding = (int)newValue;
	}

	private static void OnItemSpacingChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    (bindable as XamCombobox).DisplaySelectedItems.Spacing = (int)newValue;
	}

	private static void OnCornerChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    (bindable as XamCombobox).BorderFrame.CornerRadius = (int)newValue;
	}

	private static void OnFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    (bindable as XamCombobox).NoItemSelected.FontSize = (int)newValue;
	}

	private static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    (bindable as XamCombobox).NoItemSelected.TextColor = (Color)newValue;
	}

	private static void OnBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    (bindable as XamCombobox).BorderFrame.BorderColor = (Color)newValue;
	}

	private static void onExpandIconChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    (bindable as XamCombobox).ExpandIconImg.Source = (string)newValue;
	}

	private static void OnPlaceHolderTextChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    (bindable as XamCombobox).NoItemSelected.Text = (string)newValue;
	}

	private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    var view = bindable as XamCombobox;
	    var source = newValue as Dictionary<string, string>;

	    if (source == null || !source.Any())
	    {
		return;
	    }

	    BindableLayout.SetItemsSource(view.DisplaySourceItems, source.Keys);
	}

	private static void OnSelectedItemsChanged(BindableObject bindable, object oldValue, object newValue)
	{
	    var view = bindable as XamCombobox;
	    var source = newValue as Dictionary<string, string>;

	    // Display selected items
	    if (!source.IsNullOrEmpty())
	    {
		view.DisplaySelectedItems.ItemsSource = source.Keys;
	    }
	    else
	    {
		view.DisplaySelectedItems.ItemsSource = new List<string>();
	    }

	    Device.BeginInvokeOnMainThread(() =>
	    {
		if (source != null && source.Any())
		{
		    // Check items selected and change color
		    var selectedKey = view.SelectedItems.Keys;
		    foreach (StackLayout item in view.DisplaySourceItems.Children)
		    {
			if (selectedKey.Contains((item.Children[1] as Label).Text))
			{
			    item.BackgroundColor = view.SelectedColor;
			}
			else
			{
			    item.BackgroundColor = Color.White;
			}
		    }

		    view.NoItemSelected.IsVisible = false;
		}
		else
		{
		    view.NoItemSelected.IsVisible = true;

		    // Handle clear item selected
		    foreach (StackLayout item in view.DisplaySourceItems.Children)
		    {
			item.BackgroundColor = Color.White;
		    }
		}
	    });
	}

	private void OnFocus_Tapped(object sender, EventArgs e)
	{
	    Device.BeginInvokeOnMainThread(() =>
	    {
		if (Source == null || !Source.Any())
		{
		    return;
		}

		bool isFocus = !DisplaySourceItems.IsVisible;

		DisplaySourceItems.IsVisible = isFocus;
	    });
	}

	private void OnItemSelected_Tapped(object sender, EventArgs e)
	{
	    Device.BeginInvokeOnMainThread(() =>
	    {
		var view = sender as StackLayout;
		string key = (e as TappedEventArgs).Parameter as string;

		// Update data
		if (Mode == ComboboxSelectionMode.Multible)
		{
		    if (SelectedItems != null && SelectedItems.ContainsKey(key))
		    {
			view.BackgroundColor = Color.White;
			SelectedItems.Remove(key);
		    }
		    else
		    {
			view.BackgroundColor = SelectedColor;
			SelectedItems.Add(key, Source[key]);
		    }
		}
		else
		{
		    SelectedItems.Clear();
		    SelectedItems.Add(key, Source[key]);

		    // Handle color
		    foreach (StackLayout item in DisplaySourceItems.Children)
		    {
			if ((item.Children[1] as Label).Text.Equals(key))
			{
			    item.BackgroundColor = SelectedColor;
			}
			else
			{
			    item.BackgroundColor = Color.White;
			}
		    }
		}

		// Update UI
		DisplaySelectedItems.ItemsSource = new List<string>(SelectedItems.Keys);
		if (!SelectedItems.Keys.Any())
		{
		    NoItemSelected.IsVisible = true;
		}
		else
		{
		    NoItemSelected.IsVisible = false;
		}
	    });
	}

	private void OnDeleteItemSelected_Tapped(object sender, EventArgs e)
	{
	    var key = (e as TappedEventArgs).Parameter as string;
	    SelectedItems.Remove(key);
	    DisplaySelectedItems.ItemsSource = new List<string>(SelectedItems.Keys);

	    if (!SelectedItems.Any())
	    {
		NoItemSelected.IsVisible = true;
	    }
	    else
	    {
		NoItemSelected.IsVisible = false;
	    }

	    foreach (StackLayout item in DisplaySourceItems.Children)
	    {
		if ((item.Children[1] as Label).Text.Equals(key))
		{
		    item.BackgroundColor = Color.White;
		}
	    }
	}
    }
}