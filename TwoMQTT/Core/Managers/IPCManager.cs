using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using TwoMQTT.Core.Interfaces;

namespace TwoMQTT.Core.Managers
{
    /// <summary>
    /// An class representing a managed way to handle IPC.
    /// </summary>
    public class IPCManager<TIncoming, TOutgoing> : IIPC<TIncoming, TOutgoing>
        where TIncoming : class
        where TOutgoing : class
    {
        public IPCManager(ChannelReader<TIncoming> incoming, ChannelWriter<TOutgoing> outgoing)
        {
            this.Incoming = incoming;
            this.Outgoing = outgoing;
        }

        /// <intheritdoc />
        public async Task ReadAsync(Func<TIncoming, Task> handler, CancellationToken cancellationToken = default)
        {
            await foreach (var item in this.Incoming.ReadAllAsync(cancellationToken))
            {
                await handler(item);
            }
        }

        /// <intheritdoc />
        public ValueTask WriteAsync(TOutgoing item, CancellationToken cancellationToken = default) =>
            this.Outgoing.WriteAsync(item, cancellationToken);

        /// <summary>
        /// The channel reader used to read incoming data.
        /// </summary>
        private readonly ChannelReader<TIncoming> Incoming;

        /// <summary>
        /// The channel writer used to publish outgoing data.
        /// </summary>
        private readonly ChannelWriter<TOutgoing> Outgoing;
    }
}