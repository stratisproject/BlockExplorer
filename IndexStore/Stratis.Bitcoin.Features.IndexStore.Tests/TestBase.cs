using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Stratis.Bitcoin.Tests
{
    public class TestBase
    {
        /// <summary>Factory for creating loggers.</summary>
        protected readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Initializes logger factory for inherited tests.
        /// </summary>
        public TestBase()
        {
            this.loggerFactory = new LoggerFactory();
            DBreezeSerializer serializer = new DBreezeSerializer();
            serializer.Initialize();
        }

        /// <summary>
        /// Creates a directory and initializes a <see cref="DataFolder"/> for a test, based on the name of the class containing the test and the name of the test.
        /// </summary>
        /// <param name="caller">The calling object, from which we derive the namespace in which the test is contained.</param>
        /// <param name="callingMethod">The name of the test being executed. A directory with the same name will be created.</param>
        /// <returns>The <see cref="DataFolder"/> that was initialized.</returns>
        public static DataFolder CreateDataFolder(object caller, [System.Runtime.CompilerServices.CallerMemberName] string callingMethod = "")
        {
            string directoryPath = GetTestDirectoryPath(caller, callingMethod);
            return AssureEmptyDirAsDataFolder(directoryPath);
        }

        /// <summary>
        /// Creates a directory for a test, based on the name of the class containing the test and the name of the test.
        /// </summary>
        /// <param name="caller">The calling object, from which we derive the namespace in which the test is contained.</param>
        /// <param name="callingMethod">The name of the test being executed. A directory with the same name will be created.</param>
        /// <returns>The path of the directory that was created.</returns>
        public static string CreateTestDir(object caller, [System.Runtime.CompilerServices.CallerMemberName] string callingMethod = "")
        {
            string directoryPath = GetTestDirectoryPath(caller, callingMethod);
            return AssureEmptyDir(directoryPath);
        }

        /// <summary>
        /// Gets the path of the directory that <see cref="CreateTestDir(object, string)"/> or <see cref="CreateDataFolder(object, string)"/> would create.        
        /// </summary>
        /// <remarks>The path of the directory is of the form TestCase/{testClass}/{testName}.</remarks>
        /// <param name="caller">The calling object, from which we derive the namespace in which the test is contained.</param>
        /// <param name="callingMethod">The name of the test being executed. A directory with the same name will be created.</param>
        /// <returns>The path of the directory.</returns>
        public static string GetTestDirectoryPath(object caller, [System.Runtime.CompilerServices.CallerMemberName] string callingMethod = "")
        {
            return Path.Combine("TestCase", caller.GetType().Name, callingMethod);
        }

        private static DataFolder AssureEmptyDirAsDataFolder(string dir)
        {
            var dataFolder = new DataFolder(new NodeSettings { DataDir = AssureEmptyDir(dir) });
            return dataFolder;
        }

        private static string AssureEmptyDir(string dir)
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);

            Directory.CreateDirectory(dir);

            return dir;
        }
    }
}