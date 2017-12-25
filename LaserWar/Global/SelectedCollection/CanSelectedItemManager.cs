using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserWar.Global.SelectedCollection
{
	public class CanSelectedItemManager<T, TId> where T : CanSelectedItem<TId>, ICloneable
	{
		bool m_HandleRequestsFromChilds = true;

		#region Collection
		private SelectedObservableCollection<T, TId> m_Collection = null;

		public SelectedObservableCollection<T, TId> Collection
		{
			get { return m_Collection; }
			set
			{
				if (m_Collection != value)
				{
					m_Collection = value;
				}
			}
		}
		#endregion
				

		/// <summary>
		/// Обработка выделения элемента
		/// </summary>
		/// <param name="Item"></param>
		public void ItemSelected(T Item)
		{
			SelectedItemSchanged(m_Collection.SelectedItem, Item);
		}


		/// <summary>
		/// Обработка выделения элемента в коллекции
		/// </summary>
		/// <param name="Item"></param>
		public void SelectedItemSchanged(T PrevSelectedItem, T Item)
		{
			if (!m_HandleRequestsFromChilds)
				return;

			m_HandleRequestsFromChilds = false;

			T PrevSelectedItemCopy = null;

			if (PrevSelectedItem != null)
			{
				PrevSelectedItemCopy = PrevSelectedItem.Clone() as T;
								
				if (Item != null)
				{
					if (PrevSelectedItem.ID.Equals(Item.ID))
					{	// Повторно выделили элемент
						Item.Reselect();
					}
					else
					{
						PrevSelectedItem.IsSelected = false;
						PrevSelectedItem.OnSelected();
						Item.OnSelected();
					}
				}
				else
				{	// Deselected
					PrevSelectedItem.IsSelected = false;
					PrevSelectedItem.OnSelected();
				}

				m_Collection.SelectedItem = Item; // Обновляем m_Collection.SelectedItem

				m_HandleRequestsFromChilds = true;

				m_Collection.OnSelectedItemChanged(PrevSelectedItemCopy, Item);
			}
			else if (Item != null)
			{
				Item.IsSelected = true;
				Item.OnSelected();
								
				m_Collection.SelectedItem = Item;

				m_HandleRequestsFromChilds = true;

				m_Collection.OnSelectedItemChanged(PrevSelectedItemCopy, Item);
			}

			m_HandleRequestsFromChilds = true;
		}
	}
}
