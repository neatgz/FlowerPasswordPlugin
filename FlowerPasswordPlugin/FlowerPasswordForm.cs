using System;
using System.Drawing;
using System.Windows.Forms;
using KeePassLib;
using KeePassLib.Security;

namespace FlowerPasswordPlugin
{
	public sealed class FlowerPasswordForm : Form
	{
		private readonly PwEntry m_entry;
		private readonly bool m_hasEntry;
		private readonly bool m_bIsChinese;

		private Label lblMemory;
		private TextBox txtMemory;
		private Button btnShowMemory;
		private Button btnCopyMemory;

		private Label lblCode;
		private TextBox txtCode;
		private Button btnShowCode;
		private Button btnCopyCode;

		private Label lblLength;
		private NumericUpDown numLength;

		private Label lblResult;
		private TextBox txtResult;
		private Button btnShowResult;
		private Button btnCopyResult;

		private Button btnGenerate;
		private Button btnApply;
		private Button btnCancel;
		private CheckBox chkStore;

		private const string FieldCode = "FP_Key";
		private const string FieldMemory = "FP_Master";

		private static string m_iconFontName;
		private static string m_iconEyeShow;
		private static string m_iconEyeHide;
		private static string m_iconClipboard;
		private static string m_iconClipboardDone;

		static FlowerPasswordForm()
		{
			InitializeIconResources();
		}

		private static void InitializeIconResources()
		{
			var fontCollection = new System.Drawing.Text.InstalledFontCollection();
			bool hasFluentIcons = false;
			foreach (var font in fontCollection.Families)
			{
				if (font.Name == "Segoe Fluent Icons")
				{
					hasFluentIcons = true;
					break;
				}
			}

			if (hasFluentIcons)
			{
				m_iconFontName = "Segoe Fluent Icons";
				m_iconEyeShow = "\uE7B3";
				m_iconEyeHide = "\uED1A";
			}
			else
			{
				m_iconFontName = "Segoe MDL2 Assets";
				m_iconEyeShow = "\uF78D";
				m_iconEyeHide = "\uECE4";
			}

			m_iconClipboard = "\uE8C8";
			m_iconClipboardDone = "\uF78C";
		}

		/// <summary>
		/// <paramref name="entry"/> 为 <c>null</c> 时仅生成密码，不显示「应用并保存」与保存选项（主菜单）。
		/// </summary>
		public FlowerPasswordForm(PwEntry entry)
		{
			m_entry = entry;
			m_hasEntry = entry != null;
			m_bIsChinese = FlowerPasswordUi.IsChineseLanguage();

			InitializeComponent();
			LoadFields();
		}

		private string T(string zh, string en) { return m_bIsChinese ? zh : en; }

		private void InitializeComponent()
		{
			Text = T("花密 (Flower Password) 密码生成", "Flower Password Generator");
			Size = new Size(580, 400);
			MinimumSize = new Size(520, 350);
			FormBorderStyle = FormBorderStyle.Sizable;
			MaximizeBox = true;
			MinimizeBox = false;
			StartPosition = FormStartPosition.CenterParent;

			TableLayoutPanel mainLayout = new TableLayoutPanel();
			mainLayout.Dock = DockStyle.Fill;
			mainLayout.ColumnCount = 3;
			mainLayout.RowCount = 7;
			mainLayout.Padding = new Padding(15);

			mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
			mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

			mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

			Font iconFont = new Font(m_iconFontName, 10f);
			Size iconBtnSize = new Size(35, 28);

			Func<TextBox, Button, Button, FlowLayoutPanel> createActionPanel = (tb, btnShow, btnCopy) =>
			{
				FlowLayoutPanel flp = new FlowLayoutPanel { WrapContents = false, Dock = DockStyle.Fill, Margin = new Padding(0) };

				btnShow.Text = tb.PasswordChar == '*' ? m_iconEyeShow : m_iconEyeHide;
				btnShow.Font = iconFont;
				btnShow.Width = iconBtnSize.Width;
				btnShow.Height = iconBtnSize.Height;
				btnShow.Margin = new Padding(5, 6, 2, 0);
				btnShow.FlatStyle = FlatStyle.System;
				btnShow.Click += (s, e) => {
					tb.PasswordChar = tb.PasswordChar == '*' ? '\0' : '*';
					btnShow.Text = tb.PasswordChar == '*' ? m_iconEyeShow : m_iconEyeHide;
				};

				btnCopy.Text = m_iconClipboard;
				btnCopy.Font = iconFont;
				btnCopy.Width = iconBtnSize.Width;
				btnCopy.Height = iconBtnSize.Height;
				btnCopy.Margin = new Padding(2, 6, 0, 0);
				btnCopy.FlatStyle = FlatStyle.System;
				btnCopy.Click += (s, e) =>
				{
					if (!string.IsNullOrEmpty(tb.Text))
					{
						Clipboard.SetText(tb.Text);
						btnCopy.Text = m_iconClipboardDone;
						var timer = new System.Windows.Forms.Timer { Interval = 1500 };
						timer.Tick += (timer_s, timer_e) =>
						{
							btnCopy.Text = m_iconClipboard;
							timer.Stop();
							timer.Dispose();
						};
						timer.Start();
					}
				};

				flp.Controls.Add(btnShow);
				flp.Controls.Add(btnCopy);
				return flp;
			};

			lblMemory = new Label { Text = T("记忆密码:", "Master Pwd:"), AutoSize = true, Anchor = AnchorStyles.Left };
			txtMemory = new TextBox { Dock = DockStyle.Fill, PasswordChar = '*', Margin = new Padding(0, 10, 0, 0) };
			btnShowMemory = new Button();
			btnCopyMemory = new Button();
			mainLayout.Controls.Add(lblMemory, 0, 0);
			mainLayout.Controls.Add(txtMemory, 1, 0);
			mainLayout.Controls.Add(createActionPanel(txtMemory, btnShowMemory, btnCopyMemory), 2, 0);

			lblCode = new Label { Text = T("区分代号:", "Site Key:"), AutoSize = true, Anchor = AnchorStyles.Left };
			txtCode = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 0) };
			btnShowCode = new Button();
			btnCopyCode = new Button();
			mainLayout.Controls.Add(lblCode, 0, 1);
			mainLayout.Controls.Add(txtCode, 1, 1);
			mainLayout.Controls.Add(createActionPanel(txtCode, btnShowCode, btnCopyCode), 2, 1);

			lblLength = new Label { Text = T("密码长度:", "Length:"), AutoSize = true, Anchor = AnchorStyles.Left };
			numLength = new NumericUpDown
			{
				Width = 70,
				Minimum = 1,
				Maximum = 16,
				Value = 16,
				Margin = new Padding(0, 10, 0, 0)
			};
			mainLayout.Controls.Add(lblLength, 0, 2);
			mainLayout.Controls.Add(numLength, 1, 2);

			lblResult = new Label { Text = T("生成结果:", "Result:"), AutoSize = true, Anchor = AnchorStyles.Left };
			txtResult = new TextBox { Dock = DockStyle.Fill, ReadOnly = true, PasswordChar = '*', Margin = new Padding(0, 10, 0, 0) };
			btnShowResult = new Button();
			btnCopyResult = new Button();
			mainLayout.Controls.Add(lblResult, 0, 3);
			mainLayout.Controls.Add(txtResult, 1, 3);
			mainLayout.Controls.Add(createActionPanel(txtResult, btnShowResult, btnCopyResult), 2, 3);

			chkStore = new CheckBox
			{
				Text = T("保存记忆密码和区分代号到自定义字段", "Save inputs to custom fields"),
				AutoSize = true,
				Anchor = AnchorStyles.Left,
				Checked = true,
				Margin = new Padding(0, 15, 0, 15)
			};
			mainLayout.Controls.Add(chkStore, 0, 4);
			mainLayout.SetColumnSpan(chkStore, 3);

			FlowLayoutPanel buttonPanel = new FlowLayoutPanel
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.RightToLeft,
				Margin = new Padding(0)
			};

			btnCancel = new Button { Text = T("取消", "Cancel"), AutoSize = true, MinimumSize = new Size(85, 35), FlatStyle = FlatStyle.System };
			btnApply = new Button { Text = T("应用并保存", "Apply & Save"), AutoSize = true, MinimumSize = new Size(115, 35), FlatStyle = FlatStyle.System };
			btnGenerate = new Button { Text = T("生成", "Generate"), AutoSize = true, MinimumSize = new Size(85, 35), FlatStyle = FlatStyle.System };

			btnCancel.Click += (s, e) => Close();
			btnApply.Click += (s, e) => Apply();
			btnGenerate.Click += (s, e) => Generate();

			buttonPanel.Controls.Add(btnCancel);
			buttonPanel.Controls.Add(btnApply);
			buttonPanel.Controls.Add(btnGenerate);

			mainLayout.Controls.Add(buttonPanel, 0, 6);
			mainLayout.SetColumnSpan(buttonPanel, 3);

			Controls.Add(mainLayout);

			if (!m_hasEntry)
			{
				btnApply.Visible = false;
				chkStore.Visible = false;
			}
		}

		private void LoadFields()
		{
			if (!m_hasEntry) return;

			string code = m_entry.Strings.ReadSafe(FieldCode);
			if (string.IsNullOrEmpty(code))
				code = m_entry.Strings.ReadSafe(PwDefs.TitleField);
			txtCode.Text = code;
			txtMemory.Text = m_entry.Strings.ReadSafe(FieldMemory);
		}

		private void Generate()
		{
			try
			{
				string full = FlowerPasswordEngine.Generate(txtMemory.Text, txtCode.Text);
				int len = (int)numLength.Value;
				len = Math.Max(1, Math.Min(len, full.Length));
				txtResult.Text = full.Substring(0, len);
			}
			catch (Exception ex)
			{
				txtResult.Text = string.Empty;
				MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void Apply()
		{
			if (!m_hasEntry) return;

			Generate();
			if (string.IsNullOrEmpty(txtResult.Text))
			{
				MessageBox.Show(this,
					T("请先输入记忆密码和区分代号。", "Please enter master password and site key."),
					Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			m_entry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, txtResult.Text));
			if (chkStore.Checked)
			{
				m_entry.Strings.Set(FieldCode, new ProtectedString(false, txtCode.Text));
				m_entry.Strings.Set(FieldMemory, new ProtectedString(true, txtMemory.Text));
			}
			m_entry.Touch(true);
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
