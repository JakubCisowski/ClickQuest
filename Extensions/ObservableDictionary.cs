// Licensed by Daniel Cazzulino under the MIT License
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickQuest.Data;
using ClickQuest.Pages;
using ClickQuest.Player;

namespace ClickQuest.Extensions
{
	[DebuggerDisplay ("Count={Count}")]
	public class ObservableDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		readonly IDictionary<TKey, TValue> dictionary;

		public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };

		public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

		// We have to update buffs only when working with specialization amount, because otherwise it leads to stack overflow!
		private bool _isSpecializationAmount;


		public ObservableDictionary (bool isSpecializationAmount = false)
			: this (new Dictionary<TKey, TValue> ())
		{
			_isSpecializationAmount = isSpecializationAmount;
		}


		public ObservableDictionary (IDictionary<TKey, TValue> dictionary, bool isSpecializationAmount = false)
		{
			this.dictionary = dictionary;

			_isSpecializationAmount = isSpecializationAmount;
		}

		void AddWithNotification (KeyValuePair<TKey, TValue> item)
		{
			AddWithNotification (item.Key, item.Value);
		}

		void AddWithNotification (TKey key, TValue value)
		{
			dictionary.Add (key, value);

			CollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Add,
				new KeyValuePair<TKey, TValue> (key, value)));
			PropertyChanged (this, new PropertyChangedEventArgs ("Count"));
			PropertyChanged (this, new PropertyChangedEventArgs ("Keys"));
			PropertyChanged (this, new PropertyChangedEventArgs ("Values"));
		}

		bool RemoveWithNotification (TKey key)
		{
			TValue value;
			if (dictionary.TryGetValue (key, out value) && dictionary.Remove (key)) {
				CollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Remove,
					new KeyValuePair<TKey, TValue> (key, value)));
				PropertyChanged (this, new PropertyChangedEventArgs ("Count"));
				PropertyChanged (this, new PropertyChangedEventArgs ("Keys"));
				PropertyChanged (this, new PropertyChangedEventArgs ("Values"));

				return true;
			}

			return false;
		}

		void UpdateWithNotification (TKey key, TValue value)
		{
			TValue existing;
			if (dictionary.TryGetValue (key, out existing)) {
				dictionary[key] = value;

				CollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Replace,
					new KeyValuePair<TKey, TValue> (key, value),
					new KeyValuePair<TKey, TValue> (key, existing)));
				PropertyChanged (this, new PropertyChangedEventArgs ("Values"));

				// Update buffs and specializations interface - HeroStats.
				if (_isSpecializationAmount)
				{
					User.Instance.CurrentHero?.Specialization.UpdateBuffs();
				}
				(Database.Pages["Town"] as TownPage).StatsFrame.Refresh();
			} else {
				AddWithNotification (key, value);
			}
		}


		protected void RaisePropertyChanged(PropertyChangedEventArgs args)
		{
			PropertyChanged (this, args);
		}

		#region IDictionary<TKey,TValue> Members


		public void Add (TKey key, TValue value)
		{
			AddWithNotification (key, value);
		}


		public bool ContainsKey (TKey key)
		{
			return dictionary.ContainsKey (key);
		}


		public ICollection<TKey> Keys
		{
			get { return dictionary.Keys; }
		}

		public bool Remove (TKey key)
		{
			return RemoveWithNotification (key);
		}


		public bool TryGetValue (TKey key, out TValue value)
		{
			return dictionary.TryGetValue (key, out value);
		}


		public ICollection<TValue> Values
		{
			get { return dictionary.Values; }
		}


		public TValue this[TKey key]
		{
			get { return dictionary[key]; }
			set { UpdateWithNotification (key, value); }
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> item)
		{
			AddWithNotification (item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear ()
		{
			((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Clear ();

			CollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
			PropertyChanged (this, new PropertyChangedEventArgs ("Count"));
			PropertyChanged (this, new PropertyChangedEventArgs ("Keys"));
			PropertyChanged (this, new PropertyChangedEventArgs ("Values"));
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains (item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo (array, arrayIndex);
		}

		int ICollection<KeyValuePair<TKey, TValue>>.Count
		{
			get { return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Count; }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly; }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> item)
		{
			return RemoveWithNotification (item.Key);
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator ();
		}

		#endregion
	}
}