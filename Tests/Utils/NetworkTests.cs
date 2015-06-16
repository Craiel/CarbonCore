namespace CarbonCore.Tests.Utils
{
    using System.Net;
    using System.Threading;
    
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.Contracts.Network;
    using CarbonCore.Utils.Contracts.Network;
    using CarbonCore.Utils.IoC;
    using CarbonCore.Utils.Network.Packages;

    using NUnit.Framework;

    [TestFixture]
    public class NetworkTests
    {
        private const int TestPort = 11201;

        private static readonly byte[] TestData = { 10, 20, 55, 0, 1 };

        private ICarbonContainer container;

        private int clientsConnected;
        private int clientsDisconnected;
        private int serverStartCount;
        private int serverStopCount;
        private int serverClientsConnected;
        private int serverClientsDisconnected;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
            this.container = CarbonContainerAutofacBuilder.Build<UtilsModule>();
        }

        [TearDown]
        public void TearDown()
        {
            this.container.Dispose();
        }

        [Test]
        public void CoreTCPServerTest()
        {
            var server = this.container.Resolve<ICoreTcpServer>();
            server.Port = TestPort;
            server.ServerStarted += this.OnServerStarted;
            server.ServerStopped += this.OnServerStopped;
            server.ClientConnected += this.OnClientConnected;
            server.ClientDisconnected += this.OnClientDisconnected;
            server.Start();

            Assert.IsTrue(server.IsRunning, "Server must be running after Start()");
            Assert.AreEqual(1, this.serverStartCount, "Server must relay the started message");

            IPEndPoint endPoint = server.GetEndpoint();
            Assert.NotNull(endPoint, "Server must return a valid endpoint");

            var client = this.container.Resolve<ICoreTcpClient>();
            client.OnConnected += this.OnClientConnect;
            client.OnDisconnected += this.OnClientDisconnect;
            client.Connect(endPoint);
            Assert.IsTrue(client.IsConnected, "Client must connect successful");
            
            client.Send(TestData);
            
            // Now we make the client receive the test data from the server (is passed in on Connect)
            client.Receive();

            client.Disconnect();
            Assert.IsFalse(client.IsConnected, "Client must disconnect successful");
            
            Thread.Sleep(3000);

            server.Stop();
            Assert.IsFalse(server.IsRunning, "Server must not be running after Stop()");

            Assert.AreEqual(1, this.clientsConnected, "Client must relay the connect message");
            Assert.AreEqual(1, this.clientsDisconnected, "Client must relay the disconnect message");
            Assert.AreEqual(1, this.serverClientsConnected, "Server must relay client connected message");
            Assert.AreEqual(1, this.serverClientsDisconnected, "Server must relay client disconnected message");
            Assert.AreEqual(1, this.serverStopCount, "Server must relay the stopped message");
        }

        [Test]
        public void JsonNetTest()
        {
            var server = this.container.Resolve<IJsonNetServer>();
            server.Port = TestPort;
            server.Start();

            IPEndPoint endPoint = server.GetEndpoint();

            var client = this.container.Resolve<IJsonNetClient>();
            client.Connect(endPoint);
            client.SendPackage(new JsonNetPackagePing());
            client.Receive();

            Thread.Sleep(3000);
            client.Close();
            server.Stop();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnServerStarted()
        {
            this.serverStartCount++;
        }

        private void OnServerStopped()
        {
            this.serverStopCount++;
        }

        private void OnClientConnect(ICoreTcpClient client)
        {
            this.clientsConnected++;
        }

        private void OnClientDisconnect(ICoreTcpClient client)
        {
            this.clientsDisconnected++;
        }

        private void OnClientConnected(ICoreTcpClient client)
        {
            this.serverClientsConnected++;

            client.Send(TestData);
        }

        private void OnClientDisconnected(ICoreTcpClient client)
        {
            this.serverClientsDisconnected++;
        }
    }
}
