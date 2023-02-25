namespace SimpleWeather.Maui.Controls;

// Based: https://stackoverflow.com/a/72994290
public class TemplatedContentPresenter : TemplatedView
{
    protected Grid HostGrid;

    public static readonly BindableProperty DataTemplateSelectorProperty = BindableProperty.Create(nameof(DataTemplateSelector), typeof(DataTemplateSelector), typeof(TemplatedContentPresenter), null, propertyChanged: DataTemplateSelectorChanged);
    public static readonly BindableProperty DataTemplateProperty = BindableProperty.Create(nameof(DataTemplate), typeof(DataTemplate), typeof(TemplatedContentPresenter), null, propertyChanged: DataTemplateChanged);
    public static readonly BindableProperty DataProperty = BindableProperty.Create(nameof(Data), typeof(object), typeof(TemplatedContentPresenter), null, propertyChanged: DataChanged);

    public DataTemplateSelector DataTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(DataTemplateSelectorProperty);
        set => SetValue(DataTemplateSelectorProperty, value);
    }

    public DataTemplate DataTemplate
    {
        get => (DataTemplate)GetValue(DataTemplateProperty);
        set => SetValue(DataTemplateProperty, value);
    }

    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        HostGrid = GetTemplateChild(nameof(HostGrid)) as Grid;
    }

    private static void DataTemplateSelectorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TemplatedContentPresenter contentPresenter && newValue is DataTemplateSelector dataTemplateSelector)
        {
            BindableLayout.SetItemTemplateSelector(contentPresenter.HostGrid, dataTemplateSelector);
        }
    }
    private static void DataTemplateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TemplatedContentPresenter contentPresenter && newValue is DataTemplate dataTemplate)
        {
            BindableLayout.SetItemTemplate(contentPresenter.HostGrid, dataTemplate);
        }
    }

    private static void DataChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TemplatedContentPresenter contentPresenter)
        {
            if (newValue is View contentView)
            {
                BindableLayout.SetItemsSource(contentPresenter.HostGrid, null);
                contentPresenter.HostGrid.Clear();
                contentPresenter.HostGrid.Add(contentView);
            }
            else
            {
                BindableLayout.SetItemsSource(contentPresenter.HostGrid, new object[] { newValue });
            }
        }
    }
}
