using System;
using System.Linq;
using MFlow.API.MVVM;
using MFlow.Data;

namespace MFlow.Operation.Adapters.Portals.CategoryManagement
{
    class CategoryManagementViewModel : BindableBase
    {
        ItemViewModel[] _Categories;
        string _Name;

        public CategoryManagementViewModel()
        {
            AddNewItemCommand = new DelegateCommand(
                () =>
                {
                    AddCategory?.Invoke(Name);
                    Name = string.Empty;
                }, 
                () => !string.IsNullOrEmpty(Name));
            
            OkCommand = new DelegateCommand(
                () => FinishCategoryManagement?.Invoke(), 
                () => Categories != null && Categories.Length > 0);
        }

        public ItemViewModel[] Categories
        {
            get => _Categories;
            set => SetProperty(ref _Categories, value);
        }
        
        public string Name
        {
            get => _Name;
            set
            {
                if (!SetProperty(ref _Name, value))
                    return;
                
                AddNewItemCommand.Invalidate();
            }
        }
        
        public DelegateCommand AddNewItemCommand { get; }

        public DelegateCommand OkCommand { get; }
        
        public event Action<string> AddCategory;

        public event Action FinishCategoryManagement;

        public event Action<Guid, string> ChangeCategoryName;

        public event Action<Guid> RemoveCategory;

        public void Update(Category[] items)
        {
            Categories = items
                .Select(CreateCategoryItem)
                .ToArray();
            
            OkCommand.Invalidate();
        }
        
        ItemViewModel CreateCategoryItem(Category item)
        {
            return new(item.Id, item.Name, 
                () =>
                {
                    var newName = InputView.Show("Name ändern", "Name", item.Name);
                    if (string.IsNullOrEmpty(newName))
                        return;
                    
                    ChangeCategoryName?.Invoke(item.Id, newName);
                },
                () => RemoveCategory?.Invoke(item.Id));
        }
    }
}