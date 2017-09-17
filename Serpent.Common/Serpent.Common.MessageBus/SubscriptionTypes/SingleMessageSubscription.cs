using System;
using System.Collections.Generic;
using System.Text;

namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System.Threading.Tasks;

    public class SingleMessageSubscription<TMessageType> : BusSubscription<TMessageType>
    {
        public SingleMessageSubscription()
        {
            
        }

        public override Task HandleMessageAsync(TMessageType message)
        {
            throw new NotImplementedException();
        }
    }
}
