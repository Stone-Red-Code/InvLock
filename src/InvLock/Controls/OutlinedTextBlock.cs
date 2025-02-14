using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace InvLock.Controls;

[ContentProperty("Text")]
public class OutlinedTextBlock : FrameworkElement, IAddChild
{
    #region Private Fields

    private Geometry? _textGeometry;

    #endregion Private Fields

    #region Private Methods

    /// <summary>
    /// Invoked when a dependency property has changed. Generate a new FormattedText object to display.
    /// </summary>
    /// <param name="d">OutlineText object whose property was updated.</param>
    /// <param name="e">Event arguments for the dependency property.</param>
    private static void OnOutlineTextInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((OutlinedTextBlock)d).CreateText();
    }

    #endregion Private Methods

    #region FrameworkElement Overrides

    /// <summary>
    /// Create the outline geometry based on the formatted text.
    /// </summary>
    public void CreateText()
    {
        FontStyle fontStyle = FontStyles.Normal;
        FontWeight fontWeight = FontWeights.Medium;

        if (Bold)
        {
            fontWeight = FontWeights.Bold;
        }

        if (Italic)
        {
            fontStyle = FontStyles.Italic;
        }

        // Create the formatted text based on the properties set.
        FormattedText formattedText = new FormattedText(
            Text,
            CultureInfo.GetCultureInfo("en-us"),
            FlowDirection.LeftToRight,
            new Typeface(Font, fontStyle, fontWeight, FontStretches.Normal),
            FontSize,
            Brushes.Black, // This brush does not matter since we use the geometry of the text.
            1);

        // Build the geometry object that represents the text.
        _textGeometry = formattedText.BuildGeometry(new Point(0, 0));

        MinWidth = formattedText.Width;
        MinHeight = formattedText.Height;
    }

    /// <summary>
    /// OnRender override draws the geometry of the text and optional highlight.
    /// </summary>
    /// <param name="drawingContext">Drawing context of the OutlineText control.</param>
    protected override void OnRender(DrawingContext drawingContext)
    {
        CreateText();
        drawingContext.DrawGeometry(Fill, new Pen(Stroke, StrokeThickness), _textGeometry);
    }

    #endregion FrameworkElement Overrides

    #region DependencyProperties

    /// <summary>
    /// Identifies the Bold dependency property.
    /// </summary>
    public static readonly DependencyProperty BoldProperty = DependencyProperty.Register(
        "Bold",
        typeof(bool),
        typeof(OutlinedTextBlock),
        new FrameworkPropertyMetadata(
            false,
            FrameworkPropertyMetadataOptions.AffectsRender,
            new PropertyChangedCallback(OnOutlineTextInvalidated),
            null
            )
        );

    /// <summary>
    /// Identifies the Fill dependency property.
    /// </summary>
    public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
        "Fill",
        typeof(Brush),
        typeof(OutlinedTextBlock),
        new FrameworkPropertyMetadata(
            new SolidColorBrush(Colors.LightSteelBlue),
            FrameworkPropertyMetadataOptions.AffectsRender,
            new PropertyChangedCallback(OnOutlineTextInvalidated),
            null
            )
        );

    /// <summary>
    /// Identifies the Font dependency property.
    /// </summary>
    public static readonly DependencyProperty FontProperty = DependencyProperty.Register(
        "Font",
        typeof(FontFamily),
        typeof(OutlinedTextBlock),
        new FrameworkPropertyMetadata(
            new FontFamily("Arial"),
            FrameworkPropertyMetadataOptions.AffectsRender,
            new PropertyChangedCallback(OnOutlineTextInvalidated),
            null
            )
        );

    /// <summary>
    /// Identifies the FontSize dependency property.
    /// </summary>
    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        "FontSize",
        typeof(double),
        typeof(OutlinedTextBlock),
        new FrameworkPropertyMetadata(
             48.0,
             FrameworkPropertyMetadataOptions.AffectsRender,
             new PropertyChangedCallback(OnOutlineTextInvalidated),
             null
             )
        );

    /// <summary>
    /// Identifies the Italic dependency property.
    /// </summary>
    public static readonly DependencyProperty ItalicProperty = DependencyProperty.Register(
        "Italic",
        typeof(bool),
        typeof(OutlinedTextBlock),
        new FrameworkPropertyMetadata(
             false,
             FrameworkPropertyMetadataOptions.AffectsRender,
             new PropertyChangedCallback(OnOutlineTextInvalidated),
             null
             )
        );

    /// <summary>
    /// Identifies the Stroke dependency property.
    /// </summary>
    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
        "Stroke",
        typeof(Brush),
        typeof(OutlinedTextBlock),
        new FrameworkPropertyMetadata(
             new SolidColorBrush(Colors.Teal),
             FrameworkPropertyMetadataOptions.AffectsRender,
             new PropertyChangedCallback(OnOutlineTextInvalidated),
             null
             )
        );

    /// <summary>
    /// Identifies the StrokeThickness dependency property.
    /// </summary>
    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
        "StrokeThickness",
        typeof(ushort),
        typeof(OutlinedTextBlock),
        new FrameworkPropertyMetadata(
             (ushort)0,
             FrameworkPropertyMetadataOptions.AffectsRender,
             new PropertyChangedCallback(OnOutlineTextInvalidated),
             null
             )
        );

    /// <summary>
    /// Identifies the Text dependency property.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text",
        typeof(string),
        typeof(OutlinedTextBlock),
        new FrameworkPropertyMetadata(
             "",
             FrameworkPropertyMetadataOptions.AffectsRender,
             new PropertyChangedCallback(OnOutlineTextInvalidated),
             null
             )
        );

    /// <summary>
    /// Specifies whether the font should display Bold font weight.
    /// </summary>
    public bool Bold
    {
        get => (bool)GetValue(BoldProperty);

        set => SetValue(BoldProperty, value);
    }

    /// <summary>
    /// Specifies the brush to use for the fill of the formatted text.
    /// </summary>
    public Brush Fill
    {
        get => (Brush)GetValue(FillProperty);

        set => SetValue(FillProperty, value);
    }

    /// <summary>
    /// The font to use for the displayed formatted text.
    /// </summary>
    public FontFamily Font
    {
        get => (FontFamily)GetValue(FontProperty);

        set => SetValue(FontProperty, value);
    }

    /// <summary>
    /// The current font size.
    /// </summary>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);

        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Specifies whether the font should display Italic font style.
    /// </summary>
    public bool Italic
    {
        get => (bool)GetValue(ItalicProperty);

        set => SetValue(ItalicProperty, value);
    }

    /// <summary>
    /// Specifies the brush to use for the stroke and optional highlight of the formatted text.
    /// </summary>
    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);

        set => SetValue(StrokeProperty, value);
    }

    /// <summary>
    /// The stroke thickness of the font.
    /// </summary>
    public ushort StrokeThickness
    {
        get => (ushort)GetValue(StrokeThicknessProperty);

        set => SetValue(StrokeThicknessProperty, value);
    }

    /// <summary>
    /// Specifies the text string to display.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);

        set => SetValue(TextProperty, value);
    }

    public void AddChild(object value)
    {
    }

    public void AddText(string text)
    {
        Text = text;
    }

    #endregion DependencyProperties
}