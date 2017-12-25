using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LaserWar.Global.SelectedCollection
{
	public class SelectedObservableCollection<T, TId> : ObservableCollection<T> where T : CanSelectedItem<TId>, ICloneable
	{
		private readonly CanSelectedItemManager<T, TId> m_Manager = null;

		#region SelectedItemChanged
		public event EventHandler<SelectedItemEventArgs<T, TId>> SelectedItemChanged;

		public virtual void OnSelectedItemChanged(T PrevSelectedItem, T CurSelectedItem)
		{
			if (SelectedItemChanged != null)
				SelectedItemChanged(this, new SelectedItemEventArgs<T, TId>(PrevSelectedItem, CurSelectedItem));
		}
		#endregion


		#region SelectedItem
		private static readonly string SelectedItemPropertyName = GlobalDefines.GetPropertyName<SelectedObservableCollection<T, TId>>(m => m.SelectedItem);
		private T m_SelectedItem = null;
		public T SelectedItem
		{
			get { return m_SelectedItem; }
			set
			{
				T PrevVal = m_SelectedItem;
				m_SelectedItem = value;
				if (PrevVal != null && m_SelectedItem != null && !PrevVal.ID.Equals(m_SelectedItem.ID))
					OnPropertyChanged(new PropertyChangedEventArgs(SelectedItemPropertyName));
				m_Manager.SelectedItemSchanged(PrevVal, m_SelectedItem); // Пускай менеджер сам решает, какое событие нужно вызвать
			}
		}
		#endregion


		public SelectedObservableCollection(CanSelectedItemManager<T, TId> Manager):
			base()
		{
			m_Manager = Manager;
		}


		public SelectedObservableCollection(CanSelectedItemManager<T, TId> Manager, IEnumerable<T> collection):
			base(collection)
		{
			m_Manager = Manager;
		}


		public SelectedObservableCollection(CanSelectedItemManager<T, TId> Manager, List<T> list):
			base(list)
		{
			m_Manager = Manager;
		}


		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
				SelectedItem = null;
			
			if (e.OldItems != null)
			{
				if (e.OldItems.Cast<T>().Contains(SelectedItem))
					SelectedItem = null;
			}

			if (e.NewItems != null)
			{
				foreach (T item in e.NewItems)
				{
					item.PreviewSelected -= item_PreviewSelected;
					item.PreviewSelected += item_PreviewSelected;
				}
			}
			base.OnCollectionChanged(e);
		}


		void item_PreviewSelected(object sender, EventArgs e)
		{
			m_Manager.ItemSelected(sender as T);
		}
	}
}
