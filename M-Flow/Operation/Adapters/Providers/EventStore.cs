using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MFlow.Data;

namespace MFlow.Operation.Adapters.Providers
{
    /// <summary>
    /// Stores the events and persists it using json.
    /// </summary>
    public class EventStore : IEventStore
    {
        #region Fields

        readonly Dictionary<Guid, List<Event>> _Events;
        readonly string _Folder;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="EventStore" />
        /// </summary>
        /// <param name="folder">The folder where the events will be stored.</param>
        public EventStore(string folder)
        {
            _Folder = folder;
            _Events = CreateEventCache(folder);
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Gets the store for the entity with the specified identifier. 
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The found store or null.</returns>
        List<Event> GetEntityStore(Guid id)
        {
            return _Events.TryGetValue(id, out var entityStore) ? entityStore : null;
        }

        /// <summary>
        /// Creates the store for the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The created store.</returns>
        List<Event> CreateEntityStore(Guid id)
        {
            var entityStore = new List<Event>();
            _Events.Add(id, entityStore);
            return entityStore;
        }
        
        /// <summary>
        /// Adds the event into the entity store. 
        /// </summary>
        /// <param name="event">The event to add.</param>
        /// <param name="store">The entity store.</param>
        static void AddToEntityStore(Event @event, ICollection<Event> store)
        {
            store.Add(@event);
        }

        /// <summary>
        /// Persists the specified event.
        /// </summary>
        /// <param name="event">The event to persist.</param>
        /// <param name="folder">The folder where the event will be persisted.</param>
        static void PersistEvent(Event @event, string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            
            var fileName = Path.Combine(folder, $"{@event.EntityId}.json");
            var line = JsonSerializer.Serialize(@event);
            File.AppendAllLines(fileName, new [] { line });
        }

        /// <summary>
        /// Creates an event cache from the specified folder.
        /// </summary>
        /// <param name="folder">The folder where the events are.</param>
        /// <returns>The event cache.</returns>
        static Dictionary<Guid, List<Event>> CreateEventCache(string folder)
        {
            var eventFiles = GetEventFiles(folder);
            var events = GetEvents(eventFiles);
            return BuildCache(events);
        }

        /// <summary>
        /// Gets the event files.
        /// </summary>
        /// <param name="folder">The folder where the events are.</param>
        /// <returns>The event files.</returns>
        static IEnumerable<string> GetEventFiles(string folder)
        {
            return !Directory.Exists(folder) ? Array.Empty<string>() : Directory.EnumerateFiles(folder, "*.json");
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <param name="eventFiles">The event files where the events are contained.</param>
        /// <returns>The events.</returns>
        static IEnumerable<Event> GetEvents(IEnumerable<string> eventFiles)
        {
            foreach (var eventFile in eventFiles)
            {
                var lines = File.ReadAllLines(eventFile);
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var @event = JsonSerializer.Deserialize<Event>(line);
                    yield return @event;
                }
            }
        }

        /// <summary>
        /// Builds the cache from the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <returns>The cache.</returns>
        static Dictionary<Guid, List<Event>> BuildCache(IEnumerable<Event> events)
        {
            return events
                .GroupBy(o => o.EntityId)
                .ToDictionary(
                    o => o.Key, 
                    o => o.OrderBy(i => i.TimeStamp).ToList());
        }

        #endregion
        
        #region Implementation of IEventStore

        /// <summary>
        /// Sets the specified event into the store.
        /// </summary>
        /// <param name="event">The event to store.</param>
        public void Set(Event @event)
        {
            var entityStore = GetEntityStore(@event.EntityId) ?? CreateEntityStore(@event.EntityId);
            AddToEntityStore(@event, entityStore);
            PersistEvent(@event, _Folder);
        }

        /// <summary>
        /// Gets all available identifiers in the store.
        /// </summary>
        /// <returns>The identifiers.</returns>
        public IEnumerable<Guid> GetIds()
        {
            return _Events.Keys;
        }

        /// <summary>
        /// Gets all events from the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The events.</returns>
        public IEnumerable<Event> Get(Guid id)
        {
            var entityStore = GetEntityStore(id);
            return entityStore;
        }

        #endregion
    }
}