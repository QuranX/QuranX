namespace QuranX.Web.Models
{
	public class Translator
	{
		public readonly string Code;
		public readonly string Name;

		public Translator(string code, string name)
		{
			Code = code;
			Name = name;
		}
	}
}