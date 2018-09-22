namespace QuranX.Persistence.Models
{
	public class Commentator
	{
		public string Code { get; set; }
		public string Description { get; set; }
		public bool IsTafsir { get; set; }

		public Commentator() { }

		public Commentator(string code, string description, bool isTafsir)
		{
			Code = code;
			Description = description;
			IsTafsir = isTafsir;
		}
	}
}
