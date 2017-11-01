using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBitcoin.Indexer.Tests
{
	class TestUtils
	{
		internal static void EnsureNew(string folderName)
		{
			if(Directory.Exists(folderName))
				Directory.Delete(folderName, true);
			while(true)
			{
				try
				{
					Directory.CreateDirectory(folderName);
					break;
				}
				catch
				{
				}
			}

		}
	}
}
