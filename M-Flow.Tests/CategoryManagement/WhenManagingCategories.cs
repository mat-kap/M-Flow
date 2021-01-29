using MFlow.Integration;
using Xunit;

namespace M_Flow.Tests.CategoryManagement
{
    public class WhenManagingCategories
    {
        [Fact]
        public void ShouldBeCategoriesGotRight()
        {
            var categoryStore = TestFactory.CreateCategoryStore();
            var workItemStore = TestFactory.CreateWorkItemStore();
            var timeServer = TestFactory.CreateTimeServer();

            var sut = new Processor(workItemStore, categoryStore, timeServer);
            var categories = sut.GetCategories();
            
            var category = TestFactory.CreateTestCategory();
            var expectedCategories = new[] {category};
            Assert.Equal(expectedCategories, categories);
        }
        
        [Fact]
        public void ShouldBeCategoryAddedRight()
        {
            var categoryStore = TestFactory.CreateCategoryStore();
            var workItemStore = TestFactory.CreateWorkItemStore();
            var timeServer = TestFactory.CreateTimeServer();

            var sut = new Processor(workItemStore, categoryStore, timeServer);
            var categories = sut.AddCategory("Kategorie 2");
            
            Assert.Equal(2, categories.Length);
            Assert.Contains(categories, o => o.Name == "Kategorie 2");
        }
        
        [Fact]
        public void ShouldBeCategoryRemovedRight()
        {
            var categoryStore = TestFactory.CreateCategoryStore();
            var workItemStore = TestFactory.CreateWorkItemStore();
            var timeServer = TestFactory.CreateTimeServer();
            var category = TestFactory.CreateTestCategory();

            var sut = new Processor(workItemStore, categoryStore, timeServer);
            var categories = sut.RemoveCategory(category.Id);
            
            Assert.Empty(categories);
        }

        [Fact]
        public void ShouldBeCategoryNameChangedRight()
        {
            var categoryStore = TestFactory.CreateCategoryStore();
            var workItemStore = TestFactory.CreateWorkItemStore();
            var timeServer = TestFactory.CreateTimeServer();
            var category = TestFactory.CreateTestCategory();

            var sut = new Processor(workItemStore, categoryStore, timeServer);
            var categories = sut.ChangeCategoryName(category.Id, "Geänderte Kategorie");
            
            Assert.Single(categories);
            Assert.Contains(categories, o => o.Name == "Geänderte Kategorie");
        }
    }
}