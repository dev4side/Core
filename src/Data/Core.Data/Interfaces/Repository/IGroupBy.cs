using System;

namespace Core.Data
{
    public interface IGroupBy
    {
        bool StopGroupBy { get; }
        string GetGroupBy();
    }
}
