using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.Netcode.MultiprocessRuntimeTests
{
    // [TestFixture(1, new string[] { "default-win:test-win" })]
    // [TestFixture(2, new string[] { "default-mac:test-mac" })]
    // [TestFixture(3, new string[] { "default-win:test-win"  , "default-mac:test-mac" })]
    // [TestFixture(4, new string[] { "default-win:test-win"  , "default-win:test-win2" })]
    // [TestFixture(5, new string[] { "default-mac:test-mac2" , "default-mac:test-mac" })]
    public class BokkenInterfaceTests : BaseMultiprocessTests
    {
        protected override bool IsPerformanceTest => false;

        public BokkenInterfaceTests()
        {
            MultiprocessLogger.Log("Bokken Interface Tests ... Constructor");
            if (!MultiprocessOrchestration.ShouldRunMultiMachineTests())
            {
                Assert.Ignore("Bokken Interface Tests not enabled");
            }
        }

        [OneTimeSetUp]
        public void SetUpBokkenInterfaceTestSuite()
        {
            MultiprocessLogger.Log("SetupBokkenInterfaceTests... start");
        }

        [UnitySetUp]
        public IEnumerator SetupBokkenInterfaceTests()
        {
            while (IsSceneLoading)
            {
                MultiprocessLogger.Log($"Waiting on IsSceneLoading {IsSceneLoading} and {m_HasSceneLoaded}");
                yield return new WaitForSecondsRealtime(1.0f);
            }
        }

        [UnityTest]
        public IEnumerator CheckPreconditions()
        {
            MultiprocessLogger.Log($"Are Clients Connected: {WorkerCount}, {m_ConnectedClientsList.Count}");
            MultiprocessLogger.Log($" {MultiprocessOrchestration.MultiprocessDirInfo.FullName}: {MultiprocessOrchestration.MultiprocessDirInfo.Exists}\n" +
                $" {BokkenMachine.PathToDll}\n" +
                $" {MultiprocessOrchestration.UserProfile_Home}");

            var pathTodll = new FileInfo(BokkenMachine.PathToDll);

            Assert.True(pathTodll.Exists, "The Bokken API Dll exists");

            var externalProcess = BokkenMachine.ExecuteCommand("--help", true);

            Assert.True(externalProcess.HasExited, "The process should have exited");

            string externalProcessStdOut = externalProcess.StandardOutput.ReadToEnd();

            Assert.IsNotNull(externalProcessStdOut, "The help output should not be null");

            string externalProcessStdErr = externalProcess.StandardError.ReadToEnd();

            Assert.True(string.IsNullOrEmpty(externalProcessStdErr), $"The help command error stream should be null but was {externalProcessStdErr}");

            MultiprocessLogger.Log("Before yield");
            yield return new WaitForSeconds(0.1f);
            MultiprocessLogger.Log("after yield");
        }
    }
}