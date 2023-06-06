// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
namespace Microsoft.AspNetCore.Components.Endpoints.Binding;

internal abstract class DictionaryConverter<TDictionary> : FormDataConverter<TDictionary>
{
}

internal sealed class DictionaryConverter<TDictionary, TDictionaryPolicy, TBuffer, TKey, TValue> : DictionaryConverter<TDictionary>
    where TKey : ISpanParsable<TKey>
    where TDictionaryPolicy : IDictionaryBufferAdapter<TDictionary, TBuffer, TKey, TValue>
{
    private readonly FormDataConverter<TValue> _valueConverter;

    public DictionaryConverter(FormDataConverter<TValue> elementConverter)
    {
        ArgumentNullException.ThrowIfNull(elementConverter);

        _valueConverter = elementConverter;
    }

    internal override bool TryRead(
        ref FormDataReader context,
        Type type,
        FormDataMapperOptions options,
        [NotNullWhen(true)] out TDictionary? result,
        out bool found)
    {
        TValue currentValue;
        TBuffer buffer;
        bool foundCurrentValue;
        bool currentElementSuccess;
        bool succeded = true;

        var keys = context.GetKeys();
        found = keys.HasValues();
        if (!found)
        {
            result = default!;
            return true;
        }

        buffer = TDictionaryPolicy.CreateBuffer();

        // We can't precompute dictionary anyKeys ahead of time,
        // so the moment we find a dictionary, we request the list of anyKeys
        // for the current location, which will involve parsing the form data anyKeys
        // and building a tree of anyKeys.
        var keyCount = 0;
        var maxCollectionSize = options.MaxCollectionSize;

        foreach (var key in keys)
        {
            context.PushPrefix(key.Value.Span);
            currentElementSuccess = _valueConverter.TryRead(ref context, typeof(TValue), options, out currentValue!, out foundCurrentValue);
            context.PopPrefix(key.Value.Span);

            if (!TKey.TryParse(key.Value.Span[1..^1], CultureInfo.InvariantCulture, out var keyValue))
            {
                succeded = false;
                // Will report an error about unparsable key here.

                // Continue trying to bind the rest of the dictionary.
                continue;
            }

            TDictionaryPolicy.Add(ref buffer, keyValue!, currentValue);
            keyCount++;
            if (keyCount == maxCollectionSize)
            {
                succeded = false;
                break;
            }
        }

        result = TDictionaryPolicy.ToResult(buffer);
        return succeded;
    }
}