using System;
using System.Collections.Generic;

namespace MFlow.Data
{
    /// <summary>
    /// Holds the data of a work item.
    /// </summary>
    public class WorkItem : IEquatable<WorkItem>
    {
        #region Properties
        
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; init; } 
        
        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime Creation { get; init; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the associated category.  
        /// </summary>
        public Guid CategoryId { get; init; }

        /// <summary>
        /// Gets or sets the count of the working phases. 
        /// </summary>
        public List<DateTime> WorkingPhases { get; } = new();
        
        /// <summary>
        /// Gets or sets the finished state. 
        /// </summary>
        public bool IsFinished { get; set; }

        #endregion

        #region Equality members

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(WorkItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!Id.Equals(other.Id) || !Creation.Equals(other.Creation) || Name != other.Name ||
                !CategoryId.Equals(other.CategoryId) || IsFinished != other.IsFinished || 
                WorkingPhases.Count != other.WorkingPhases.Count)
            {
                return false;
            }

            for (var index = 0; index < WorkingPhases.Count; index++)
            {
                if (WorkingPhases[index] != other.WorkingPhases[index])
                    return false;
            }

            return true;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((WorkItem) obj);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Creation, Name, CategoryId, WorkingPhases, IsFinished);
        }

        #endregion

        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}