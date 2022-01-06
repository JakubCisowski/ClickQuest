// ObservableDictionary.cs is based on Daniel Cazzulino's implementation
// to be found here: https://gist.github.com/kzu/cfe3cb6e4fe3efea6d24

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace ClickQuest.Game.DataTypes.Collections;

[DebuggerDisplay("Count={Count}")]
public class ObservableDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
{
	public delegate void SpecializationCollectionUpdatedEventHandler(object sender, EventArgs e);

	private readonly IDictionary<TKey, TValue> _dictionary;

	public ObservableDictionary() : this(new Dictionary<TKey, TValue>())
	{
	}

	public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
	{
		_dictionary = dictionary;
	}

	public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };

	public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

	// Custom events.

	// Handle updating specialization text and ToolTips - only for Specialization.SpecializationAmounts.
	// Handle specialization buffs, amounts and threshold updates - for collections in Specialization.
	public event SpecializationCollectionUpdatedEventHandler SpecializationCollectionUpdated;

	private void AddWithNotification(KeyValuePair<TKey, TValue> item)
	{
		AddWithNotification(item.Key, item.Value);
	}

	private void AddWithNotification(TKey key, TValue value)
	{
		_dictionary.Add(key, value);

		CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
		PropertyChanged(this, new PropertyChangedEventArgs("Count"));
		PropertyChanged(this, new PropertyChangedEventArgs(nameof(Keys)));
		PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
	}

	private bool RemoveWithNotification(TKey key)
	{
		TValue value;
		if (_dictionary.TryGetValue(key, out value) && _dictionary.Remove(key))
		{
			CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value)));
			PropertyChanged(this, new PropertyChangedEventArgs("Count"));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Keys)));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));

			return true;
		}

		return false;
	}

	private void UpdateWithNotification(TKey key, TValue value)
	{
		TValue existing;
		if (_dictionary.TryGetValue(key, out existing))
		{
			_dictionary[key] = value;

			CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, existing)));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));

			// Custom events.

			// Update interface when Specialization Amount is changed.
			// Also refresh stats when any Specialization collection is changed.
			SpecializationCollectionUpdated?.Invoke(this, new EventArgs());
		}
		else
		{
			AddWithNotification(key, value);
		}
	}

	protected void RaisePropertyChanged(PropertyChangedEventArgs args)
	{
		PropertyChanged(this, args);
	}

	public void Add(TKey key, TValue value)
	{
		AddWithNotification(key, value);
	}

	public bool ContainsKey(TKey key)
	{
		return _dictionary.ContainsKey(key);
	}

	public ICollection<TKey> Keys => _dictionary.Keys;

	public bool Remove(TKey key)
	{
		return RemoveWithNotification(key);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return _dictionary.TryGetValue(key, out value);
	}

	public ICollection<TValue> Values => _dictionary.Values;

	public TValue this[TKey key]
	{
		get => _dictionary[key];
		set => UpdateWithNotification(key, value);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		AddWithNotification(item);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Clear()
	{
		_dictionary.Clear();

		CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		PropertyChanged(this, new PropertyChangedEventArgs("Count"));
		PropertyChanged(this, new PropertyChangedEventArgs(nameof(Keys)));
		PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		return _dictionary.Contains(item);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		_dictionary.CopyTo(array, arrayIndex);
	}

	int ICollection<KeyValuePair<TKey, TValue>>.Count => _dictionary.Count;

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => _dictionary.IsReadOnly;

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		return RemoveWithNotification(item.Key);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return _dictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _dictionary.GetEnumerator();
	}
}