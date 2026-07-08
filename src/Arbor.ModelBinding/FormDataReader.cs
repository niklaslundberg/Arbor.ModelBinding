using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Arbor.ModelBinding.Core;

/// <summary>
/// Encapsulates a flat form collection and provides sequential access.
/// The current prototype uses string keys; a future span-based reader
/// can replace the backing store without changing generated binder shape.
/// </summary>
public sealed class FormDataReader
{
    private readonly IReadOnlyList<KeyValuePair<string, StringValues>> _pairs;
    private int _index = -1;

    public FormDataReader(IEnumerable<KeyValuePair<string, StringValues>> pairs)
    {
        if (pairs is null)
            throw new ArgumentNullException(nameof(pairs));

        _pairs = pairs as IReadOnlyList<KeyValuePair<string, StringValues>>
                 ?? new List<KeyValuePair<string, StringValues>>(pairs);
    }

    public int Count => _pairs.Count;

    public bool MoveNext()
    {
        _index++;
        return _index < _pairs.Count;
    }

    public string CurrentKey => _pairs[_index].Key;

    public StringValues CurrentValue => _pairs[_index].Value;

    public ReadOnlySpan<char> CurrentKeySpan => CurrentKey.AsSpan();

    public int CurrentIndex => _index;

    public void Reset()
    {
        _index = -1;
    }
}
