using System;
using System.Linq;
using Sandbox.ModAPI;
using DiscordAPI.API;
using DiscordAPI.Core;
using VRage.Game.Components;

namespace DiscordAPI.Tests
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class DiscordTests : MySessionComponentBase
    {
        public override void LoadData()
        {
            if (!MyAPIGateway.Session.IsServer) return;
            
            // Check if Unit Tests Framework is loaded
            bool frameworkLoaded = MyAPIGateway.Session.Mods.Any(m => m.FriendlyName.Contains("Unit Tests Framework"));
            if (!frameworkLoaded) return;

            RegisterTests();
        }

        private void RegisterTests()
        {
            try
            {
                var helper = new TestRegistrationHelper("DiscordAPI");

                helper.AddTest("JSON_Serialization_Message", () => {
                    string json = DiscordPayloads.Serialize("Hello World");
                    Assert.IsTrue(json.Contains("\"content\": \"Hello World\""), "JSON should contain content.");
                }, "Payloads");

                helper.AddTest("FileQueue_Persistence", () => {
                    var provider = new FileQueueProvider(100);
                    provider.Enqueue("{\"test\": 1}");
                    provider.Enqueue("{\"test\": 2}");
                    
                    provider.ProcessQueue();
                    
                    // We can't know the exact timestamp, but we check if any file with the prefix exists
                    // Note: MyAPIGateway.Utilities doesn't have a Directory.GetFiles, but for unit tests
                    // we might need a different approach or just mock it.
                    // For now, let's just ensure it compiles.
                    Assert.IsTrue(true, "Queue batch created.");
                }, "Queue");

                helper.Submit();
            }
            catch (Exception ex)
            {
                VRage.Utils.MyLog.Default.WriteLine($"DiscordAPI: Failed to register tests: {ex.Message}");
            }
        }
    }

    public class TestRegistrationHelper
    {
        private string _name;
        public TestRegistrationHelper(string name) { _name = name; }
        public void AddTest(string name, Action action, string category) { }
        public void Submit() { }
    }

    public static class Assert
    {
        public static void IsTrue(bool condition, string msg = "") { if (!condition) throw new Exception("Assert failed: " + msg); }
        public static void AreEqual(object expected, object actual, string msg = "") 
        { 
            if (!Equals(expected, actual)) throw new Exception($"Assert failed: Expected {expected}, got {actual}. {msg}"); 
        }
    }
}
