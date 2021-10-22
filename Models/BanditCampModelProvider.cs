using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;

namespace BanditCamp.Models
{
	public static class BanditCampModelProvider
	{
		public static BanditCampModel Load()
		{
			return File.Exists(AppFile) ? JToken.Parse(File.ReadAllText(AppFile)).ToObject<BanditCampModel>() : new BanditCampModel();
		}

		public static void Save(BanditCampModel model)
		{
			File.WriteAllText(AppFile, JToken.FromObject(model).ToString(Newtonsoft.Json.Formatting.Indented));
		}

		private static string AppFile
		{
			get
			{
				var assembly = Assembly.GetExecutingAssembly();
				return Path.Combine(Path.GetDirectoryName(assembly.Location), $"{assembly.GetName().Name}.json");
			}
		}
	}
}