using System;
using System.Linq;
using MFlow.API.MVVM;
using MFlow.Data;

namespace MFlow.Operation.Adapters.Portals.DayPlanning
{
    class DayPlanningViewModel : BindableBase
    {
        ItemViewModel[] _DayPoints;
        Category[] _Categories;
        Category _Category;
        string _Name;

        public DayPlanningViewModel()
        {
            AddNewItemCommand = new DelegateCommand(
                () =>
                {
                    AddDayPoint?.Invoke(Name, Category);
                    Name = string.Empty;
                }, 
                () => Category != null && !string.IsNullOrEmpty(Name));
            
            FinishCommand = new DelegateCommand(
                () => FinishDayPlanning?.Invoke(), 
                () => DayPoints != null && DayPoints.Length > 0);

            ManageCategoriesCommand = new DelegateCommand(() => ManageCategories?.Invoke());
        }
        
        public ItemViewModel[] DayPoints
        {
            get => _DayPoints;
            set => SetProperty(ref _DayPoints, value);
        }

        public Category[] Categories
        {
            get => _Categories;
            set => SetProperty(ref _Categories, value);
        }

        public Category Category
        {
            get => _Category;
            set
            {
                if (!SetProperty(ref _Category, value))
                    return;
                
                AddNewItemCommand.Invalidate();
            }
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

        public DelegateCommand FinishCommand { get; }
        
        public DelegateCommand ManageCategoriesCommand { get; }
        
        public event Action<string, Category> AddDayPoint;

        public event Action FinishDayPlanning;

        public event Action<Guid, string> ChangeDayPointName;

        public event Action<Guid> RemoveDayPoint;

        public event Action ManageCategories;

        public void Update(WorkItem[] items)
        {
            DayPoints = items
                .Select(CreateDayPointItem)
                .ToArray();
            
            FinishCommand.Invalidate();
        }

        public void Update(Category[] categories)
        {
            Categories = categories;
            Category = categories.Length > 0 ? categories[0] : null;
        }
        
        ItemViewModel CreateDayPointItem(WorkItem item)
        {
            return new(item.Id, item.Name, 
                () =>
                {
                    var newName = InputView.Show("Name ändern", "Name", item.Name);
                    if (string.IsNullOrEmpty(newName))
                        return;
                    
                    ChangeDayPointName?.Invoke(item.Id, newName);
                },
                () => RemoveDayPoint?.Invoke(item.Id));
        }
    }
}