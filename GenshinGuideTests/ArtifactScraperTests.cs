using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenshinGuide.Tests
{
	[TestClass()]
	public class ArtifactScraperTests
	{
		[TestMethod()]
		public void ScbanArtifactsTest()
		{
			Navigation.Initialize("GenshinImpact");
			Navigation.AddDelay(50);

			GenshinData data = new GenshinData();
			data.GatherData(new bool[3] { false, true, false });
			GOOD g = new GOOD(data);
			Console.WriteLine(g.artifacts.Count);
		}
	}
}