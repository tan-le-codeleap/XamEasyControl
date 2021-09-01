using System.Collections.Generic;
using Xamarin.Forms;

namespace XamEasyControl
{
    public partial class MainPage : ContentPage
    {
	public MainPage()
	{
	    InitializeComponent();

	    ComboboxControl.Source = new Dictionary<string, string>
	    {
		{ "Cat", "Cat" },
		{ "Dog", "Dog" },
		{ "Tiger", "Tiger" }
	    };
	}
    }
}
