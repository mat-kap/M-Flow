using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MFlow.API;
using MFlow.Data;

namespace MFlow.Operation.Domain
{
    /// <summary>
    /// Handles the creation of a performance report.
    /// </summary>
    public static class PerformanceReport
    {
        /// <summary>
        /// Determines the date from the specified working dates.
        /// </summary>
        /// <param name="workingDays">The working days.</param>
        /// <returns>The name of the month and the year.</returns>
        public static Tuple<string, int> DetermineDate(WorkingDay[] workingDays)
        {
            var date = workingDays[0].Date;
            var monthName = $"{date:MMMM}";
            var year = date.Year;
            return Tuple.Create(monthName, year);
        }

        /// <summary>
        /// Extracts the working points from the specified working days.
        /// </summary>
        /// <param name="workingDays">The working days.</param>
        /// <returns>The working points.</returns>
        public static IEnumerable<WorkingPoint> ExtractWorkingPoints(IEnumerable<WorkingDay> workingDays)
        {
            return workingDays.SelectMany(o => o.WorkingPoints).ToArray();
        }

        /// <summary>
        /// Creates the protocol groups from the working pints.
        /// </summary>
        /// <param name="workingPoints">The working points.</param>
        /// <returns>The protocol groups.</returns>
        public static ProtocolGroup[] CreateGroups(IEnumerable<WorkingPoint> workingPoints)
        {
            return ProcessWorkingPoints(workingPoints, (point, groups) =>
            {
                var group = GetGroup(point, groups);
                var entry = GetEntry(point, group);
                AddWorkingTimeToEntry(point, entry);
            });
        }
        
        /// <summary>
        /// Calculates the total times of each group.
        /// </summary>
        /// <param name="groups">The protocol groups.</param>
        /// <returns>The protocol groups.</returns>
        public static ProtocolGroup[] CalculateGroupWorkingTimes(ProtocolGroup[] groups)
        {
            foreach (var group in groups)
            {
                var totalTime = group.WorkingPoints.Aggregate(TimeSpan.Zero, (t, o) => t + o.Time);
                group.TotalTime = totalTime;
            }
            
            return groups;
        }

        /// <summary>
        /// Composes the report.
        /// </summary>
        /// <param name="monthName">The name of the month.</param>
        /// <param name="year">The year.</param>
        /// <param name="groups">The protocol groups.</param>
        /// <returns>The report as text.</returns>
        public static string ComposeReport(string monthName, int year, ProtocolGroup[] groups)
        {
            var (maxEntryNameLength, maxEntryTimeLength) = DetermineMaxEntryLengths(groups);
            return BuildText(monthName, year, groups, maxEntryNameLength, maxEntryTimeLength);
        }

        
        /// <summary>
        /// Suggests the file name.
        /// </summary>
        /// <param name="monthName">The name of the month.</param>
        /// <param name="year">The year.</param>
        /// <returns>The suggested file name.</returns>
        public static string SuggestFileName(string monthName, int year)
        {
            return $"Leistungsbericht {monthName} {year}.txt";
        }

        /// <summary>
        /// Determines the maximum lengths of the entry name and entry time.
        /// </summary>
        /// <param name="groups">The protocol groups.</param>
        /// <returns>The maximum entry name length and the maximum entry time length.</returns>
        static Tuple<int, int> DetermineMaxEntryLengths(IEnumerable<ProtocolGroup> groups)
        {
            var maxNameLength = 0;
            var maxTimeLength = 0;
            foreach (var entry in groups.SelectMany(o => o.WorkingPoints))
            {
                maxNameLength = Math.Max(maxNameLength, entry.Name.Length);
                
                var time = Formatting.FormatTime(entry.Time);
                maxTimeLength = Math.Max(maxTimeLength, time.Length);
            }
            return Tuple.Create(maxNameLength, maxTimeLength);
        }

        /// <summary>
        /// Builds the report text.
        /// </summary>
        /// <param name="monthName">The name of the month.</param>
        /// <param name="year">The year.</param>
        /// <param name="groups">The protocol groups.</param>
        /// <param name="maxEntryNameLength">The maximum entry name length.</param>
        /// <param name="maxEntryTimeLength">The maximum entry time length.</param>
        /// <returns>The report as text.</returns>
        static string BuildText(string monthName, int year, IEnumerable<ProtocolGroup> groups, 
            int maxEntryNameLength, int maxEntryTimeLength)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Leistungsbericht {monthName} {year}");
            builder.AppendLine();

            foreach (var group in groups)
            {
                var totalTime = Formatting.FormatTime(group.TotalTime);
                builder.AppendLine($"  {group.Name} ({totalTime})");

                foreach (var entry in group.WorkingPoints)
                {
                    var time = Formatting.FormatTime(entry.Time);
                    var formattedName = string.Format($"{{0,{-maxEntryNameLength}}}", entry.Name);
                    var formattedTime = string.Format($"{{0,{maxEntryTimeLength}}}", time);
                    builder.AppendLine($"    - {formattedName}      {formattedTime}");
                }
                
                builder.AppendLine();
            }
            
            return builder.ToString();
        }

        /// <summary>
        /// Processes the working points.
        /// </summary>
        /// <param name="workingPoints">The working points.</param>
        /// <param name="onWorkingPoint">Called when a working point should be processed.</param>
        /// <returns>The protocol groups.</returns>
        static ProtocolGroup[] ProcessWorkingPoints(IEnumerable<WorkingPoint> workingPoints,
            Action<WorkingPoint, Dictionary<string, ProtocolGroup>> onWorkingPoint)
        {
            var groups = new Dictionary<string, ProtocolGroup>();
            foreach (var workingPoint in workingPoints)
                onWorkingPoint(workingPoint, groups);

            return groups.Values.ToArray();
        }

        /// <summary>
        /// Gets the protocol group for the specified working point.
        /// </summary>
        /// <param name="workingPoint">The working point.</param>
        /// <param name="groups">The protocol groups.</param>
        /// <returns>The protocol group for the specified working point.</returns>
        static ProtocolGroup GetGroup(WorkingPoint workingPoint, IDictionary<string, ProtocolGroup> groups)
        {
            if (groups.TryGetValue(workingPoint.Category, out var group))
                return group;

            group = new ProtocolGroup { Name = workingPoint.Category };
            groups.Add(workingPoint.Category, group);
            return group;
        }

        /// <summary>
        /// Gets the protocol entry for the specified working point.
        /// </summary>
        /// <param name="workingPoint">The working point.</param>
        /// <param name="group">The protocol group.</param>
        /// <returns>The protocol entry for the specified working point.</returns>
        static ProtocolEntry GetEntry(WorkingPoint workingPoint, ProtocolGroup group)
        {
            var entry = group.WorkingPoints.FirstOrDefault(o => o.Name == workingPoint.Name);
            if (entry != null)
                return entry;

            entry = new ProtocolEntry {Name = workingPoint.Name, Time = TimeSpan.Zero};
            group.WorkingPoints.Add(entry);
            return entry;
        }

        /// <summary>
        /// Adds the working time to the protocol entry.
        /// </summary>
        /// <param name="workingPoint">The working point.</param>
        /// <param name="entry">The protocol entry.</param>
        static void AddWorkingTimeToEntry(WorkingPoint workingPoint, ProtocolEntry entry)
        {
            entry.Time += workingPoint.WorkingTime;
        }
    }
}