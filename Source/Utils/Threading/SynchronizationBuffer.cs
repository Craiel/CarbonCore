namespace CarbonCore.Utils.Threading
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Diagnostics;

    // One-way synchronization buffer, Works only in single writer, single reader environment
    public class SynchronizationBuffer<T> 
        where T : class, new()
    {
        private const int BufferCount = 3;

        private readonly List<T> buffer;
        private readonly List<State> bufferState;
        private readonly List<uint> timestamp;

        private uint timestampId;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SynchronizationBuffer()
        {
            this.buffer = new List<T>();
            this.bufferState = new List<State>();
            this.timestamp = new List<uint>();

            this.timestampId = 0u;
            for (int k = 0; k < BufferCount; k++)
            {
                this.buffer.Add(new T());
                this.timestamp.Add(this.timestampId);
                this.bufferState.Add(State.Free);
            }
        }

        private enum State
        {
            Read,
            Write,
            Free,
        }

        private enum Relevance
        {
            Latest,
            Oldest,
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SynchronizationDataLock<T> AquireReadLock()
        {
            return this.AquireLock(Relevance.Latest, State.Read);
        }

        public SynchronizationDataLock<T> AquireWriteLock()
        {
            return this.AquireLock(Relevance.Oldest, State.Write);
        }

        public void ReleaseLock(SynchronizationDataLock<T> dataLock)
        {
            lock (this)
            {
                Diagnostic.Assert(this.bufferState[dataLock.Id] != State.Free, "Can't release free buffer");

                switch (this.bufferState[dataLock.Id])
                {
                    case State.Write:
                        this.timestamp[dataLock.Id] = ++this.timestampId;
                        break;
                }

                this.bufferState[dataLock.Id] = State.Free;
            }
        }

        private SynchronizationDataLock<T> AquireLock(Relevance relevance, State operation)
        {
            lock (this)
            {
                int bestMatch = -1;
                for (int k = 0; k < this.bufferState.Count; k++)
                {
                    if (this.bufferState[k] == State.Free)
                    {
                        if (bestMatch == -1 ||
                            (relevance == Relevance.Latest ?
                                this.timestamp[bestMatch] < this.timestamp[k] :
                                this.timestamp[bestMatch] > this.timestamp[k]))
                        {
                            bestMatch = k;
                        }
                    }
                }

                Diagnostic.Assert(bestMatch != -1, "Can't aquire buffer");

                this.bufferState[bestMatch] = operation;
                return new SynchronizationDataLock<T>(this.buffer[bestMatch], bestMatch);
            }
        }
    }
}
