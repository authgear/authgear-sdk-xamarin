namespace MauiSample;

public partial class LabeledSwitch : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        "Text",
        typeof(string),
        typeof(LabeledSwitch),
        string.Empty);
    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
        "IsToggled",
        typeof(bool),
        typeof(LabeledSwitch),
        false);
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    public bool IsToggled
    {
        get { return (bool)GetValue(IsToggledProperty); }
        set { SetValue(IsToggledProperty, value); }
    }
    public LabeledSwitch()
    {
        InitializeComponent();
    }
}