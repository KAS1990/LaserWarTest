using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaserWar.Commands;
using LaserWar.Global;

namespace LaserWar.Global.SelectedCollection
{
	public class CanSelectedItem<TId> : Notifier, ICloneable
	{
		public TId ID { get; private set; }
		public int Index { get; private set; }

		
		#region Selected
		public event EventHandler Selected;

		public virtual void OnSelected()
		{
			if (Selected != null)
				Selected(this, new EventArgs());
		}
		#endregion


		#region PreviewSelected
		/// <summary>
		/// Менеджер должен подписаться на это событие
		/// </summary>
		public event EventHandler PreviewSelected;

		public virtual void OnPreviewSelected()
		{
			if (PreviewSelected != null)
				PreviewSelected(this, new EventArgs());
		}
		#endregion


		#region IsSelected
		private static readonly string IsSelectedPropertyName = GlobalDefines.GetPropertyName<CanSelectedItem<TId>>(m => m.IsSelected);

		private bool m_IsSelected = false;

		public bool IsSelected
		{
			get { return m_IsSelected; }
			set
			{
				bool PrevVal = m_IsSelected;
				m_IsSelected = value;
				if (PrevVal != m_IsSelected)
					OnPropertyChanged(IsSelectedPropertyName);
				OnPreviewSelected(); // Пускай менеджер сам решает, какое событие нужно вызвать
			}
		}
		#endregion


		#region SelectCommand
		private readonly RelayCommand m_SelectCommand;
		/// <summary>
		///
		/// </summary>
		public RelayCommand SelectCommand
		{
			get { return m_SelectCommand; }
		}
		#endregion


		public void Reselect()
		{
			OnReselected();
		}


		protected virtual void OnReselected()
		{

		}


		public CanSelectedItem(TId id, int index)
		{
			ID = id;
			Index = index;

			m_SelectCommand = new RelayCommand(arg => IsSelected = true);
		}


		object ICloneable.Clone()
		{
			CanSelectedItem<TId> result = new CanSelectedItem<TId>(this.ID, this.Index)
			{
				IsSelected = this.IsSelected
			};

			return result;
		}
	}
}
