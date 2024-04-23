using System;

namespace quest_web.Models
{
    public interface IHasTimestamps
    {
        DateTime? creationDate { get; set; }
        DateTime? updatedDate { get; set; }
    }
}
