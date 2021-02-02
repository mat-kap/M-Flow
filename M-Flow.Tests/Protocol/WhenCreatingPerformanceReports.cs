using System;
using System.Text;
using MFlow.Data;
using MFlow.Integration;
using Xunit;

namespace M_Flow.Tests.Protocol
{
    public class WhenCreatingPerformanceReports
    {
        [Fact]
        public void ShouldResultBeRight()
        {
            var workingDays = CreateWorkingDays();
            
            var (report, suggestedFileName) = Processor.CreatePerformanceReport(workingDays);

            Assert.Equal("Leistungsbericht Januar 2021.txt", suggestedFileName);
            
            var expectedReport = GetExpectedReport();
            Assert.Equal(expectedReport, report);
        }

        static WorkingDay[] CreateWorkingDays()
        {
            var today = new DateTime(2021, 1, 30);
            var point1Id = Guid.NewGuid();
            var point2Id = Guid.NewGuid();
            var point4Id = Guid.NewGuid();
            return new[]
            {
                new WorkingDay
                {
                    Date = today - TimeSpan.FromDays(2),
                    WorkingPoints = 
                    {
                        new WorkingPoint
                        {
                            Category = "Category 1",
                            Id = point1Id,
                            Name = "Point 1",
                            WorkingTime = TimeSpan.FromMinutes(300.0)
                        },
                        new WorkingPoint
                        {
                            Category = "Category 1",
                            Id = point2Id,
                            Name = "Point 2",
                            WorkingTime = TimeSpan.FromMinutes(50.0)
                        }
                    }
                },
                new WorkingDay
                {
                    Date = today,
                    WorkingPoints = 
                    {
                        new WorkingPoint
                        {
                            Category = "Category 1",
                            Id = point1Id,
                            Name = "Point 1",
                            WorkingTime = TimeSpan.FromMinutes(25.0)
                        },
                        new WorkingPoint
                        {
                            Category = "Category 1",
                            Id = point2Id,
                            Name = "Point 2",
                            WorkingTime = TimeSpan.FromMinutes(25.0)
                        },
                        new WorkingPoint
                        {
                            Category = "Category 3",
                            Id = point4Id,
                            Name = "Point 4",
                            WorkingTime = TimeSpan.FromMinutes(25.0)
                        }
                    }
                }
            };
        }

        static string GetExpectedReport()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Leistungsbericht Januar 2021");
            builder.AppendLine();
            builder.AppendLine("  Category 1 (6h 40min)");
            builder.AppendLine("    - Point 1      5h 25min");
            builder.AppendLine("    - Point 2      1h 15min");
            builder.AppendLine();
            builder.AppendLine("  Category 3 (25min)");
            builder.AppendLine("    - Point 4         25min");
            builder.AppendLine();
            
            return builder.ToString();
        }
    }
}