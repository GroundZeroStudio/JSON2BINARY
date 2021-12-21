using System;
using System.ComponentModel;

namespace JSON2BINARY
{
	[Bindable(true)]
	public class JsonFileInfo
	{
		[Bindable(true)]
		public string FileName { get; set; }

		[Bindable(true)]
		public bool IsSelect { get; set; }

		public string FilePath { get; set; }

		public JsonFileInfo()
		{
		}
	}
}
