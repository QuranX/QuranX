namespace QuranX.Persistence.Models
{
	public class Dictionary
	{
		public string Code { get; }
		public string Name { get; }
		public string Copyright { get; }

		public Dictionary(string code, string name, string copyright)
		{
			Code = code;
			Name = name;
			Copyright = copyright;
		}
	}
}
