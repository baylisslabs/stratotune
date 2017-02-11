using System;

namespace bit.projects.iphone.chromatictuner.model
{
    public interface IEntity<IdType>
    {
        IdType Id { get; }      
    }
}

