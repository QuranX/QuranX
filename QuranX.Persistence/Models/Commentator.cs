namespace QuranX.Persistence.Models
{
	public class Commentator
	{
		public string Code { get; set; }
		public string Description { get; set; }

		public Commentator() { }

		public Commentator(string code, string description)
		{
			Code = code;
			Description = description;
		}
	}
}
