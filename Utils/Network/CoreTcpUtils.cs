namespace CarbonCore.Utils.Network
{
    using System;
    using System.Net.Sockets;

    using CarbonCore.Utils.Contracts.Network;

    public static class CoreTcpUtils
    {
        private const int DefaultBufferSize = 8192;
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public static void InitiateClientRead(
            ICoreTcpClient client,
            Action<CoreTcpData> callback,
            int bufferSize = DefaultBufferSize,
            long? timeOut = null)
        {
            var instruction = new PeerReadInstruction
                                  {
                                      Client = client,
                                      Callback = callback,
                                      BufferSize = bufferSize,
                                      TimeOut = timeOut,
                                      Data = new CoreTcpData(bufferSize) { Client = client }
                                  };

            BeginClientRead(instruction);
        }

        public static void InitiateClientWrite(
            ICoreTcpClient client, byte[] data, Action<CoreTcpData> callback, int bufferSize = DefaultBufferSize)
        {
            var instruction = new PeerWriteInstruction
                                  {
                                      Client = client,
                                      BufferSize = bufferSize,
                                      Data = new CoreTcpData(data),
                                      Callback = callback
                                  };

            BeginClientWrite(instruction);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void BeginClientRead(PeerReadInstruction instruction)
        {
            NetworkStream clientStream = instruction.Client.GetStream();
            if (clientStream == null)
            {
                instruction.Data.State = TcpDataState.Disconnected;
                instruction.Callback(instruction.Data);
                return;
            }
            
            clientStream.BeginRead(instruction.Data.Data, 0, instruction.BufferSize, OnClientDataRead, instruction);
        }

        private static void OnClientDataRead(IAsyncResult ar)
        {
            var instruction = (PeerReadInstruction)ar.AsyncState;
            try
            {
                var stream = instruction.Client.GetStream();
                if (stream != null)
                {
                    instruction.Data.BytesRead = stream.EndRead(ar);
                    if (instruction.Data.BytesRead > 0)
                    {
                        instruction.LastReadTime = DateTime.Now.Ticks;
                    }
                    else
                    {
                        // Check if the read timed out
                        if (instruction.TimeOut != null && instruction.LastReadTime + instruction.TimeOut > DateTime.Now.Ticks)
                        {
                            instruction.Data.State = TcpDataState.TimedOut;
                            System.Diagnostics.Trace.TraceWarning("BaseTcpPeer Timed out: {0}", instruction.Client.EndPoint);
                        }
                    }
                }
            }
            catch
            {
                instruction.Data.State = TcpDataState.Disconnected;
                System.Diagnostics.Trace.TraceWarning("BaseTcpPeer Disconnected: {0}", instruction.Client.EndPoint);
            }

            instruction.Callback(instruction.Data);

            switch (instruction.Data.State)
            {
                case TcpDataState.Connected:
                    {
                        // Queue up another read on this instruction
                        BeginClientRead(instruction);
                        break;
                    }
            }
        }

        private static void BeginClientWrite(PeerWriteInstruction instruction)
        {
            NetworkStream clientStream = instruction.Client.GetStream();
            if (clientStream == null)
            {
                instruction.Data.State = TcpDataState.Disconnected;
                instruction.Callback(instruction.Data);
                return;
            }

            instruction.BytesToRead = instruction.BufferSize;
            if (instruction.Offset + instruction.BufferSize > instruction.Data.Data.Length)
            {
                instruction.BytesToRead = instruction.Data.Data.Length - instruction.Offset;
            }

            clientStream.BeginWrite(instruction.Data.Data, instruction.Offset, instruction.BytesToRead, OnClientDataWrite, instruction);
        }

        private static void OnClientDataWrite(IAsyncResult ar)
        {
            var instruction = (PeerWriteInstruction)ar.AsyncState;
            try
            {
                var stream = instruction.Client.GetStream();
                if (stream != null)
                {
                    stream.EndWrite(ar);
                    instruction.Data.BytesWritten += instruction.BytesToRead;
                    instruction.Offset += instruction.BytesToRead;
                }
            }
            catch
            {
                instruction.Data.State = TcpDataState.Disconnected;
                System.Diagnostics.Trace.TraceWarning("BaseTcpPeer Disconnected: {0}", instruction.Client.EndPoint);
            }

            instruction.Callback(instruction.Data);

            // Check if we are done writing for this instruction
            if (instruction.Offset >= instruction.Data.Data.Length)
            {
                return;
            }

            switch (instruction.Data.State)
            {
                case TcpDataState.Connected:
                    {
                        // Queue up another read on this instruction
                        BeginClientWrite(instruction);
                        break;
                    }
            }
        }

        // -------------------------------------------------------------------
        // Internal classes
        // -------------------------------------------------------------------
        internal class PeerReadInstruction
        {
            public PeerReadInstruction()
            {
                this.LastReadTime = DateTime.Now.Ticks;
            }

            public ICoreTcpClient Client { get; set; }
            public int BufferSize { get; set; }
            public long? TimeOut { get; set; }
            public long LastReadTime { get; set; }

            public CoreTcpData Data { get; set; }

            public Action<CoreTcpData> Callback { get; set; }
        }

        internal class PeerWriteInstruction
        {
            public ICoreTcpClient Client { get; set; }
            public int Offset { get; set; }
            public int BytesToRead { get; set; }
            public int BufferSize { get; set; }

            public CoreTcpData Data { get; set; }

            public Action<CoreTcpData> Callback { get; set; }
        }
    }
}
