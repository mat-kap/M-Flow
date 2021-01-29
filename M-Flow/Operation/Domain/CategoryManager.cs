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

        readonly IItemStore<Category> _Store;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="CategoryManager"/>
        /// </summary>
        /// <param name="store">The category store.</param>
        public CategoryManager(IItemStore<Category> store)
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
            return ids.Select(_Store.Read).OrderBy(o => o.Name).ToArray();
        }

        /// <summary>
        /// Creates and saves a category item.
        /// </summary>
        /// <param name="name">The name.</param>
        public void CreateCategory(string name)
        {
            var item = new Category
            {
                Id = Guid.NewGuid(),
                Name = name
            };
            
            Save(item);
        }

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="id">The identifier of the category.</param>
        public void DeleteCategory(Guid id)
        {
            var item = Get(id);
            Delete(item);
        }

        /// <summary>
        /// Changes the category name.
        /// </summary>
        /// <param name="id">The identifier of the category.</param>
        /// <param name="name">The new name.</param>
        public void ChangeCategoryName(Guid id, string name)
        {
            var item = Get(id);
            item.Name = name;
            Save(item);
        }

        /// <summary>
        /// Saves the category.
        /// </summary>
        /// <param name="item">The category.</param>
        void Save(Category item)
        {
            _Store.Write(item.Id, item);
        }

        /// <summary>
        /// Deletes the category.
        /// </summary>
        /// <param name="item">The category.</param>
        void Delete(Category item)
        {
            _Store.Delete(item.Id);
        }

        /// <summary>
        /// Gets the category with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the category.</param>
        /// <returns>The category.</returns>
        Category Get(Guid id)
        {
            return _Store.Read(id);
        }

        #endregion
    }
}