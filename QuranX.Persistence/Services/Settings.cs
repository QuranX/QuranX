namespace QuranX.Persistence.Services
{
	public interface ISettings
	{
		string DataPath { get; }
	}

	public class Settings : ISettings
	{
		public string DataPath { get; private set; }

		public Settings(string dataPath)
		{
			DataPath = dataPath;
		}
	}
}
