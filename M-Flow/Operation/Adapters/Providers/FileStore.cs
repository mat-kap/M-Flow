using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MFlow.Operation.Adapters.Providers
{
    /// <summary>
    /// Implements an item store which uses files.
    /// </summary>
    /// <typeparam name="T">The type if the stored items.</typeparam>
    public class FileStore<T> : IItemStore<T>
    {
        #region Fields

        readonly string _Folder;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="FileStore{T}"/>
        /// </summary>
        /// <param name="folder">The folder where the items will be stored.</param>
        public FileStore(string folder)
        {
            _Folder = folder;
        }

        #endregion
        
        #region Methods
        
        /// <summary>
        /// Gets the file name for the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The file name.</returns>
        string GetItemFileName(Guid id)
        {
            return Path.Combine(_Folder, $"{id}.json");
        }

        /// <summary>
        /// Gets the item file names.
        /// </summary>
        /// <returns>The item file names.</returns>
        IEnumerable<string> GetItemFiles()
        {
            return Directory.Exists(_Folder) ? Directory.EnumerateFiles(_Folder, "*.json") : Array.Empty<string>();
        }

        /// <summary>
        /// Gets the item identifiers.
        /// </summary>
        /// <param name="itemFiles">The item file names.</param>
        /// <returns>The item identifiers.</returns>
        static IEnumerable<Guid> GetItemIds(IEnumerable<string> itemFiles)
        {
            foreach (var itemFile in itemFiles)
            {
                var name = Path.GetFileNameWithoutExtension(itemFile);
                if (Guid.TryParse(name, out var id))
                    yield return id;
            }
        }

        /// <summary>
        /// Builds the content of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The content.</returns>
        static string BuildContent(T item)
        {
            return JsonSerializer.Serialize(item);
        }
        
        /// <summary>
        /// Builds the item from the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The built item.</returns>
        static T BuildItem(string content)
        {
            return JsonSerializer.Deserialize<T>(content);
        }

        /// <summary>
        /// Writes the content to the specified file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="content">The content.</param>
        static void WriteIntoFile(string fileName, string content)
        {
            var folder = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            
            File.WriteAllText(fileName, content);
        }
        
        /// <summary>
        /// Reads the content of the specified file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The read content.</returns>
        static string ReadFromFile(string fileName)
        {
            return File.Exists(fileName) ? File.ReadAllText(fileName) : string.Empty;
        }

        /// <summary>
        /// Deletes the file with the specified file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        static void DeleteFile(string fileName)
        {
            if (!File.Exists(fileName))
                return;
            
            File.Delete(fileName);
        }

        #endregion

        #region Implementation of IItemStore<T>

        /// <summary>
        /// Gets the item identifiers.
        /// </summary>
        /// <returns>The item identifiers.</returns>
        public IEnumerable<Guid> GetIds()
        {
            var itemFiles = GetItemFiles();
            return GetItemIds(itemFiles);
        }

        /// <summary>
        /// Writes the specified item into the store.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        /// <param name="item">The item.</param>
        public void Write(Guid id, T item)
        {
            var fileName = GetItemFileName(id);
            var itemContent = BuildContent(item);
            WriteIntoFile(fileName, itemContent);
        }
        
        /// <summary>
        /// Reads the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        /// <returns>The read item.</returns>
        public T Read(Guid id)
        {
            var fileName = GetItemFileName(id);
            var content = ReadFromFile(fileName);
            return BuildItem(content);
        }

        /// <summary>
        /// Deletes the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        public void Delete(Guid id)
        {
            var fileName = GetItemFileName(id);
            DeleteFile(fileName);
        }

        #endregion
    }
}