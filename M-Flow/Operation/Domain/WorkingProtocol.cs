using System;
using System.Collections.Generic;
using System.Linq;
using MFlow.Data;

namespace MFlow.Operation.Domain
{
    /// <summary>
    /// Operates on the working protocol.
    /// </summary>
    public static class WorkingProtocol
    {
        /// <summary>
        /// Creates a protocol for the specified year and month.
        /// </summary>
        /// <param name="year">The requested year.</param>
        /// <param name="month">The requested month.</param>
        /// <param name="workItems">The available work items.</param>
        /// <param name="categories">The available categories.</param>
        /// <param name="concentrationPhaseDuration">The duration of a concentration phase.</param>
        /// <param name="breakPhaseDuration">The break phase duration.</param>
        /// <returns>The working days.</returns>
        public static WorkingDay[] CreateProtocol(int year, int month, IEnumerable<WorkItem> workItems, 
            Category[] categories, double concentrationPhaseDuration, double breakPhaseDuration)
        {
            var workingDates = GetWorkingDates(year, month);
            var workItemsOfMonth = GetWorkItemsOfMonth(year, month, workItems);
            return CreateWorkingDays(workingDates, workItemsOfMonth, categories, concentrationPhaseDuration, breakPhaseDuration);
        }

        /// <summary>
        /// Creates the details of the specified work item.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="category">The category name.</param>
        /// <param name="concentrationPhaseDuration">The duration of a concentration phase.</param>
        /// <param name="breakPhaseDuration">The break phase duration.</param>
        /// <returns>The details of the work item.</returns>
        public static WorkItemDetails CreateDetails(WorkItem workItem, Category category, 
            double concentrationPhaseDuration, double breakPhaseDuration)
        {
            var dayGroupedPhases = workItem.WorkingPhases.GroupBy(o => new DateTime(o.Year, o.Month, o.Day));
            var workingDays = dayGroupedPhases.Select(o => new WorkItemDetailsEntry
            {
                Date = o.Key,
                Time = TimeSpan.FromMinutes(concentrationPhaseDuration + breakPhaseDuration) * o.Count()
            }).ToArray();
            
            return new()
            {
                Id = workItem.Id,
                Name = workItem.Name,
                Category = category?.Name ?? "-",
                WorkingDays = workingDays
            };
        }
        
        /// <summary>
        /// Gets the category name from the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the category.</param>
        /// <param name="categories">The available categories.</param>
        /// <returns>The category name.</returns>
        static string GetCategory(Guid id, IEnumerable<Category> categories)
        {
            return categories.FirstOrDefault(o => o.Id == id)?.Name ?? "-";
        }

        /// <summary>
        /// Gets the working dates of the specified year and month.
        /// </summary>
        /// <param name="year">The requested year.</param>
        /// <param name="month">The requested month.</param>
        /// <returns>The working dates.</returns>
        static IEnumerable<DateTime> GetWorkingDates(int year, int month)
        {
            var dates = new DateTime[DateTime.DaysInMonth(year, month)];
            for (var day = 1; day <= dates.Length; day++)
                dates[day - 1] = new DateTime(year, month, day);
            
            return dates.Where(item =>
            {
                var dayOfWeek = item.Date.DayOfWeek;
                return dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
            });
        }

        /// <summary>
        /// Gets the work items of the specified month.
        /// </summary>
        /// <param name="year">The requested year.</param>
        /// <param name="month">The requested month.</param>
        /// <param name="workItems">The available work items.</param>
        /// <returns>The work items of the month.</returns>
        static WorkItem[] GetWorkItemsOfMonth(int year, int month, IEnumerable<WorkItem> workItems)
        {
            return workItems
                .Where(o => o.WorkingPhases.Any(p => p.Year == year && p.Month == month))
                .ToArray();
        }

        /// <summary>
        /// Creates the working days.
        /// </summary>
        /// <param name="workingDates">The working dates.</param>
        /// <param name="workItems">The available work items.</param>
        /// <param name="categories">The available categories.</param>
        /// <param name="concentrationPhaseDuration">The duration of a concentration phase.</param>
        /// <param name="breakPhaseDuration">The break phase duration.</param>
        /// <returns>The working days.</returns>
        static WorkingDay[] CreateWorkingDays(IEnumerable<DateTime> workingDates, WorkItem[] workItems, 
            Category[] categories, double concentrationPhaseDuration, double breakPhaseDuration)
        {
            var days = CreateEmptyWorkingDays(workingDates);
            SortWorkItemsIntoDays(days, workItems, (d, o) =>
            {
                var category = GetCategory(o.CategoryId, categories);
                CreateWorkingPoint(d, o, category, concentrationPhaseDuration, breakPhaseDuration);
            });
            CalculateTotalWorkingTime(days);
            return days;
        }

        /// <summary>
        /// Creates the empty working days. 
        /// </summary>
        /// <param name="workingDates">The working dates.</param>
        /// <returns>The empty working days.</returns>
        static WorkingDay[] CreateEmptyWorkingDays(IEnumerable<DateTime> workingDates)
        {
            return workingDates.Select(o => new WorkingDay {Date = o}).ToArray();
        }

        /// <summary>
        /// Sorts the work items into the specified working days.
        /// </summary>
        /// <param name="workingDays">The working days.</param>
        /// <param name="workItems">The work items to sort into.</param>
        /// <param name="onWorkingPoint">Called when a working point needs to be created in a working day.</param>
        static void SortWorkItemsIntoDays(IEnumerable<WorkingDay> workingDays, WorkItem[] workItems, 
            Action<WorkingDay, WorkItem> onWorkingPoint)
        {
            foreach (var workingDay in workingDays)
            {
                foreach (var workItem in workItems)
                {
                    if (workItem.WorkingPhases.Any(o => o.Day == workingDay.Date.Day))
                        onWorkingPoint(workingDay, workItem);
                }
            }
        }

        /// <summary>
        /// Calculates the total working time.
        /// </summary>
        /// <param name="workingDays">The working days.</param>
        static void CalculateTotalWorkingTime(IEnumerable<WorkingDay> workingDays)
        {
            foreach (var workingDay in workingDays)
                workingDay.TotalWorkTime = workingDay.WorkingPoints.Aggregate(TimeSpan.Zero, (t, o) => t + o.WorkingTime);
        }

        /// <summary>
        /// Creates the working point.
        /// </summary>
        /// <param name="day">The working day.</param>
        /// <param name="item">The work item.</param>
        /// <param name="category">The category name.</param>
        /// <param name="concentrationPhaseDuration">The duration of a concentration phase.</param>
        /// <param name="breakPhaseDuration">The break phase duration.</param>
        static void CreateWorkingPoint(WorkingDay day, WorkItem item, string category, 
            double concentrationPhaseDuration, double breakPhaseDuration)
        {
            var workingTime = TimeSpan.FromMinutes(concentrationPhaseDuration + breakPhaseDuration) *
                              item.WorkingPhases.Count(o => o.Day == day.Date.Day);
            
            day.WorkingPoints.Add(new WorkingPoint
            {
                Id = item.Id,
                Name = item.Name,
                WorkingTime = workingTime,
                Category = category
            });
        }
    }
}