namespace FlowerPasswordPlugin
{
	internal static class FlowerPasswordUi
	{
		internal static bool IsChineseLanguage()
		{
			try
			{
				string langFile = KeePass.Program.Config.Application.LanguageFile;
				if (string.IsNullOrEmpty(langFile)) return false;
				string lf = langFile.ToLowerInvariant();
				return lf.Contains("chinese") || lf.Contains("zh-") || lf.Contains("simplified") ||
					lf.Contains("traditional") || lf.Contains("hans") || lf.Contains("hant");
			}
			catch { return false; }
		}

		/// <summary>右键菜单与主菜单（工具）共用条目标题（含省略号）。</summary>
		internal static string MenuItemCaption
		{
			get { return IsChineseLanguage() ? "花密(Flower Password)..." : "Flower Password..."; }
		}
	}
}
