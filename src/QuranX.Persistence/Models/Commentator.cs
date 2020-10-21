namespace QuranX.Persistence.Models
{
	public class Commentator
	{
		public string Code { get; }
		public string Description { get; }

		public Commentator(string code, string description)
		{
			Code = code;
			Description = description;
		}
	}
}
