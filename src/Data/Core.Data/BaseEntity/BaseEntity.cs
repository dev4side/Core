using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Data.Attributes;
using Core.Data.Helpers;

namespace Core.Data
{
    public abstract class BaseEntity<T> : IEquatable<BaseEntity<T>>
    {
        [CloneExclude]
        public virtual T Id { get; set; }

        #region Comparison
        public virtual bool Equals(BaseEntity<T> other)
        {
            return Id.Equals(other.Id);
        }

        public virtual int GetHashCode(BaseEntity<T> obj)
        {
            return obj.Id.GetHashCode();
        }
        #endregion  

    }
}
