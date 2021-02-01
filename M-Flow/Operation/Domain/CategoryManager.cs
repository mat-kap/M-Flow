using System;
using System.Linq;
using MFlow.Data;
using MFlow.Operation.Adapters.Providers;

namespace MFlow.Operation.Domain
{
    /// <summary>
    /// Manages the categories.
    /// </summary>
    public class CategoryManager
    {
        #region Fields

        readonly IEventStore _Store;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="CategoryManager"/>
        /// </summary>
        /// <param name="store">The event store.</param>
        public CategoryManager(IEventStore store)
        {
            _Store = store;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <returns>The categories.</returns>
        public Category[] GetAll()
        {
            var ids = _Store.GetIds();
            return ids
                .Select(BuildCategory)
                .Where(o => o != null)
                .OrderBy(o => o.Name)
                .ToArray();
        }

        /// <summary>
        /// Creates and saves a category item.
        /// </summary>
        /// <param name="name">The name.</param>
        public void CreateCategory(string name)
        {
            _Store.Set(new Event
            {
                Type = "CategoryCreated",
                EntityId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                Data = new()
                {
                    { "Name", name }
                }
            });
        }

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="id">The identifier of the category.</param>
        public void DeleteCategory(Guid id)
        {
            _Store.Set(new Event
            {
                Type = "ItemDeleted",
                EntityId = id,
                TimeStamp = DateTime.Now
            });
        }

        /// <summary>
        /// Changes the category name.
        /// </summary>
        /// <param name="id">The identifier of the category.</param>
        /// <param name="name">The new name.</param>
        public void ChangeCategoryName(Guid id, string name)
        {
            _Store.Set(new Event
            {
                Type = "NameChanged",
                EntityId = id, 
                TimeStamp = DateTime.Now,
                Data = new()
                {
                    { "Name", name }
                }
            });
        }
        
        /// <summary>
        /// Builds the category with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the category.</param>
        /// <returns>The built category or null if deleted.</returns>
        Category BuildCategory(Guid id)
        {
            Category category = null;
            
            var events = _Store.Get(id);
            foreach (var @event in events)
            {
                switch (@event.Type)
                {
                    case "CategoryCreated":
                        category ??= new Category
                        {
                            Id = id, 
                            Name = @event.Data["Name"]
                        };
                        break;
                    case "NameChanged":
                        if (category != null)
                            category.Name = @event.Data["Name"];
                        break;
                    case "ItemDeleted":
                        category = null;
                        break;
                }
            }

            return category;
        }
        
        #endregion
    }
}