namespace MadLevelManager
{
	public interface IMadLevelProfileBackend
	{
		void Start();

		string LoadProfile(string profileName);

		void SaveProfile(string profileName, string value);

		void Flush();

		bool CanWorkInEditMode();
	}
}
