using System;
using System.Collections.Generic;
using MFlow.Data;

namespace MFlow.Operation.Adapters.Providers
{
    /// <summary>
    /// Stores events through the event identifier.  
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Sets the specified event into the store.
        /// </summary>
        /// <param name="event">The event to store.</param>
        void Set(Event @event);

        /// <summary>
        /// Gets all available identifiers in the store.
        /// </summary>
        /// <returns>The identifiers.</returns>
        IEnumerable<Guid> GetIds();
        
        /// <summary>
        /// Gets all events from the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The events.</returns>
        IEnumerable<Event> Get(Guid id);
    }
}