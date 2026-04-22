using System;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePassLib;

namespace FlowerPasswordPlugin
{
	public sealed class FlowerPasswordPluginExt : Plugin
	{
		private IPluginHost m_host;
		private ToolStripSeparator m_sepEntry;
		private ToolStripMenuItem m_tsmiEntry;

		public override bool Initialize(IPluginHost host)
		{
			if (host == null) return false;
			m_host = host;

			m_sepEntry = new ToolStripSeparator();
			m_tsmiEntry = new ToolStripMenuItem
			{
				Text = FlowerPasswordUi.MenuItemCaption
			};
			m_tsmiEntry.Click += OnEntryMenuClick;
			m_host.MainWindow.EntryContextMenu.Items.Add(m_sepEntry);
			m_host.MainWindow.EntryContextMenu.Items.Add(m_tsmiEntry);

			return true;
		}

		public override void Terminate()
		{
			if (m_host != null)
			{
				if (m_tsmiEntry != null)
				{
					m_host.MainWindow.EntryContextMenu.Items.Remove(m_tsmiEntry);
					m_tsmiEntry.Dispose();
					m_tsmiEntry = null;
				}
				if (m_sepEntry != null)
				{
					m_host.MainWindow.EntryContextMenu.Items.Remove(m_sepEntry);
					m_sepEntry.Dispose();
					m_sepEntry = null;
				}
			}
			m_host = null;
		}

		public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
		{
			if (t != PluginMenuType.Main) return null;

			var tsmi = new ToolStripMenuItem
			{
				Text = FlowerPasswordUi.MenuItemCaption
			};
			tsmi.Click += OnMainMenuClick;
			return tsmi;
		}

		private void OnMainMenuClick(object sender, EventArgs e)
		{
			if (m_host == null) return;

			using (var f = new FlowerPasswordForm(null))
				f.ShowDialog(m_host.MainWindow);
		}

		private void OnEntryMenuClick(object sender, EventArgs e)
		{
			if (m_host == null) return;

			PwEntry[] selected = m_host.MainWindow.GetSelectedEntries();
			if (selected == null || selected.Length == 0) return;

			PwEntry entry = selected[0];

			using (var form = new FlowerPasswordForm(entry))
			{
				if (form.ShowDialog(m_host.MainWindow) == DialogResult.OK)
					m_host.MainWindow.UpdateUI(false, null, false, null, true, null, true);
			}
		}
	}
}
