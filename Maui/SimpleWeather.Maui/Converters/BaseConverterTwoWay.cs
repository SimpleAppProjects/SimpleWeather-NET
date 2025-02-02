using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Extensions;

namespace SimpleWeather.Maui.Converters;

// https://github.com/CommunityToolkit/Maui/blob/main/src/CommunityToolkit.Maui/Converters/BaseConverter.shared.cs
/// <summary>
/// Abstract class used to implement converters that implement the ConvertBack logic.
/// </summary>
/// <typeparam name="TFrom">Type of the input value.</typeparam>
/// <typeparam name="TTo">Type of the output value.</typeparam>
/// <typeparam name="TParam">Type of parameter</typeparam>
public abstract class BaseConverterTwoWay<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TFrom,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TTo, TParam> : ValueConverterExtension,
    ICommunityToolkitValueConverter
{
    /// <summary>
    /// Default value to return when <see cref="IValueConverter.Convert(object?, Type, object?, CultureInfo?)"/> throws an <see cref="Exception"/>.
    /// This value is used when <see cref="Options.ShouldSuppressExceptionsInConverters"/> is set to <see langword="true"/>.
    /// </summary>
    public abstract TTo DefaultConvertReturnValue { get; set; }

    /// <summary>
    /// Default value to return when <see cref="IValueConverter.ConvertBack(object?, Type, object?, CultureInfo?)"/> throws an <see cref="Exception"/>.
    /// This value is used when <see cref="Options.ShouldSuppressExceptionsInConverters"/> is set to <see langword="true"/>.
    /// </summary>
    public abstract TFrom DefaultConvertBackReturnValue { get; set; }

    /// <inheritdoc/>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type FromType { get; } = typeof(TFrom);

    /// <inheritdoc/>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type ToType { get; } = typeof(TTo);

    /// <summary>
    /// Type of TParam
    /// </summary>
    public Type ParamType { get; } = typeof(TParam);

    object? ICommunityToolkitValueConverter.DefaultConvertReturnValue => DefaultConvertReturnValue;
    object? ICommunityToolkitValueConverter.DefaultConvertBackReturnValue => DefaultConvertBackReturnValue;

    /// <summary>
    /// Method that will be called by <see cref="IValueConverter.Convert(object?, Type, object?, CultureInfo?)"/>.
    /// </summary>
    /// <param name="value">The object to convert <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>.</param>
    /// <param name="parameter">Optional Parameters</param>
    /// <param name="culture">Culture Info</param>
    /// <returns>An object of type <typeparamref name="TTo"/>.</returns>
    public abstract TTo ConvertFrom(TFrom value, TParam parameter, CultureInfo? culture);

    /// <summary>
    /// Method that will be called by <see cref="IValueConverter.ConvertBack(object?, Type, object?, CultureInfo?)"/>.
    /// </summary>
    /// <param name="value">Value to be converted from <typeparamref name="TTo"/> to <typeparamref name="TFrom"/>.</param>
    /// <param name="parameter">Optional Parameters</param>
    /// <param name="culture">Culture Info</param>
    /// <returns>An object of type <typeparamref name="TFrom"/>.</returns>
    public abstract TFrom ConvertBackTo(TTo value, TParam parameter, CultureInfo? culture);

    /// <inheritdoc/>
    object? ICommunityToolkitValueConverter.ConvertBack(object? value,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type targetType,
        object? parameter, CultureInfo? culture)
    {
        try
        {
            ValueConverterExtensions.ValidateTargetType<TFrom>(targetType, false);

            var converterParameter = ValueConverterExtensions.ConvertParameter<TParam>(parameter);
            var converterValue = ValueConverterExtensions.ConvertValue<TTo>(value);

            return ConvertBackTo(converterValue, converterParameter, culture);
        }
        catch (Exception ex)
        {
            return DefaultConvertBackReturnValue;
        }
    }

    /// <inheritdoc/>
    object? ICommunityToolkitValueConverter.Convert(object? value,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type targetType,
        object? parameter, CultureInfo? culture)
    {
        try
        {
            ValueConverterExtensions.ValidateTargetType<TTo>(targetType, true);

            var converterParameter = ValueConverterExtensions.ConvertParameter<TParam>(parameter);
            var converterValue = ValueConverterExtensions.ConvertValue<TFrom>(value);

            return ConvertFrom(converterValue, converterParameter, culture);
        }
        catch (Exception ex)
        {
            return DefaultConvertReturnValue;
        }
    }
}

/// <summary>
/// Abstract class used to implement converters that implement the ConvertBack logic.
/// </summary>
/// <typeparam name="TFrom">Type of the input value.</typeparam>
/// <typeparam name="TTo">Type of the output value.</typeparam>
public abstract class BaseConverterTwoWay<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TFrom,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TTo> : ValueConverterExtension,
    ICommunityToolkitValueConverter
{
    /// <summary>
    /// Default value to return when <see cref="IValueConverter.Convert(object?, Type, object?, CultureInfo?)"/> throws an <see cref="Exception"/>.
    /// This value is used when <see cref="Options.ShouldSuppressExceptionsInConverters"/> is set to <see langword="true"/>.
    /// </summary>
    public abstract TTo DefaultConvertReturnValue { get; set; }

    /// <summary>
    /// Default value to return when <see cref="IValueConverter.ConvertBack(object?, Type, object?, CultureInfo?)"/> throws an <see cref="Exception"/>.
    /// This value is used when <see cref="Options.ShouldSuppressExceptionsInConverters"/> is set to <see langword="true"/>.
    /// </summary>
    public abstract TFrom DefaultConvertBackReturnValue { get; set; }

    /// <inheritdoc/>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type FromType { get; } = typeof(TFrom);

    /// <inheritdoc/>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type ToType { get; } = typeof(TTo);

    object? ICommunityToolkitValueConverter.DefaultConvertReturnValue => DefaultConvertReturnValue;
    object? ICommunityToolkitValueConverter.DefaultConvertBackReturnValue => DefaultConvertBackReturnValue;

    /// <summary>
    /// Method that will be called by <see cref="IValueConverter.Convert(object?, Type, object?, CultureInfo?)"/>.
    /// </summary>
    /// <param name="value">The object to convert <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>.</param>
    /// <param name="culture">Culture Info</param>
    /// <returns>An object of type <typeparamref name="TTo"/>.</returns>
    public abstract TTo ConvertFrom(TFrom value, CultureInfo? culture);

    /// <summary>
    /// Method that will be called by <see cref="IValueConverter.ConvertBack(object?, Type, object?, CultureInfo?)"/>.
    /// </summary>
    /// <param name="value">Value to be converted from <typeparamref name="TTo"/> to <typeparamref name="TFrom"/>.</param>
    /// <param name="culture">Culture Info</param>
    /// <returns>An object of type <typeparamref name="TFrom"/>.</returns>
    public abstract TFrom ConvertBackTo(TTo value, CultureInfo? culture);

    /// <inheritdoc/>
    object? ICommunityToolkitValueConverter.ConvertBack(object? value,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type targetType,
        object? parameter, CultureInfo? culture)
    {
        try
        {
            ValueConverterExtensions.ValidateTargetType<TFrom>(targetType, false);

            var converterValue = ValueConverterExtensions.ConvertValue<TTo>(value);

            return ConvertBackTo(converterValue, culture);
        }
        catch (Exception ex)
        {
            return DefaultConvertBackReturnValue;
        }
    }

    /// <inheritdoc/>
    object? ICommunityToolkitValueConverter.Convert(object? value,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type targetType,
        object? parameter, CultureInfo? culture)
    {
        try
        {
            ValueConverterExtensions.ValidateTargetType<TTo>(targetType, true);

            var converterValue = ValueConverterExtensions.ConvertValue<TFrom>(value);

            return ConvertFrom(converterValue, culture);
        }
        catch (Exception ex)
        {
            return DefaultConvertReturnValue;
        }
    }
}

// https://github.com/CommunityToolkit/Maui/blob/main/src/CommunityToolkit.Maui/Extensions/ValueConverterExtension.shared.cs
internal static class ValueConverterExtensions
{
    private static bool IsNullable(Type type)
    {
        if (!type.IsValueType)
        {
            return true; // ref-type
        }

        if (Nullable.GetUnderlyingType(type) is not null)
        {
            return true; // Nullable<T>
        }

        return false; // value-type
    }

    private static bool IsNullable<T>() => IsNullable(typeof(T));

    private static bool
        IsValidTargetType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TTarget>(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            in Type targetType, bool shouldAllowNullableValueTypes)
    {
        if (IsConvertingToString(targetType) && CanBeConvertedToString())
        {
            return true;
        }

        // Is targetType a Nullable Value Type? Eg TTarget is bool and targetType is bool?
        if (shouldAllowNullableValueTypes && targetType.IsValueType && IsValidNullableValueType(targetType))
        {
            return true;
        }

        try
        {
            var instanceOfT = default(TTarget);
            instanceOfT ??= (TTarget?)Activator.CreateInstance(targetType);

            var result = Convert.ChangeType(instanceOfT, targetType);

            return result is not null;
        }
        catch
        {
            return false;
        }

        static bool IsConvertingToString(in Type targetType) => targetType == typeof(string);

        static bool CanBeConvertedToString() => typeof(TTarget).GetMethods()
            .Any(x => x.Name is nameof(ToString) && x.ReturnType == typeof(string));

        static bool IsValidNullableValueType(Type targetType)
        {
            if (!IsNullable(targetType))
            {
                return false;
            }

            var underlyingType = Nullable.GetUnderlyingType(targetType) ??
                                 throw new InvalidOperationException("Non-nullable are not valid");

            return underlyingType == typeof(TTarget);
        }
    }

    internal static void
        ValidateTargetType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TTarget>(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type targetType,
            bool shouldAllowNullableValueTypes)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        // Ensure TTo can be assigned to the given Target Type
        if (!typeof(TTarget)
                .IsAssignableFrom(
                    targetType) // Ensure TTarget can be assigned from targetType. Eg TTarget is IEnumerable and targetType is IList
            && !IsValidTargetType<TTarget>(targetType,
                shouldAllowNullableValueTypes)) // Ensure targetType be converted to TTarget? Eg `Convert.ChangeType()` returns a non-null value
        {
            throw new ArgumentException($"targetType needs to be assignable from {typeof(TTarget)}.",
                nameof(targetType));
        }
    }

#pragma warning disable CS8603 // Possible null reference return. If TParam is null (e.g. `string?`), a null return value is expected
    internal static TParam ConvertParameter<TParam>(object? parameter) => parameter switch
    {
        null when IsNullable<TParam>() => default,
        null when !IsNullable<TParam>() => throw new ArgumentNullException(nameof(parameter),
            $"Value cannot be null because {nameof(TParam)} is not nullable."),
        TParam convertedParameter => convertedParameter,
        _ => throw new ArgumentException($"Parameter needs to be of type {typeof(TParam)}", nameof(parameter))
    };
#pragma warning restore CS8603 // Possible null reference return.

#pragma warning disable CS8603 // Possible null reference return. If TValue is null (e.g. `string?`), a null return value is expected
    internal static TValue ConvertValue<TValue>(object? value) => value switch
    {
        null when IsNullable<TValue>() => default,
        null when !IsNullable<TValue>() => throw new ArgumentNullException(nameof(value),
            $"Value cannot be null because {nameof(TValue)} is not nullable"),
        TValue convertedValue => convertedValue,
        _ => throw new ArgumentException($"Value needs to be of type {typeof(TValue)}", nameof(value))
    };
#pragma warning restore CS8603 // Possible null reference return.
}