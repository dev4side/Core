using System;

namespace Core.Data
{
    public interface IProjection
    {
        string ProjectionName { get; }
        string ProjectionAlias { get; }
        IJoin Join { get; }
        IGroupBy GroupBy { get; }
    }
}
