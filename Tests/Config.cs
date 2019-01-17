using JsonSettings;

namespace Tests
{
	public static class Config
	{
		public static T GetValue<T>(string path)
		{
			return JsonConfig.GetValue<T>("..\\..\\..\\settings.json", path);
		}
	}
}