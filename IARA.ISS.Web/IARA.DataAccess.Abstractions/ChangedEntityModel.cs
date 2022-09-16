using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IARA.DataAccess.Abstractions
{
    public class ChangedEntityModel
    {
        public EntityState State { get; set; }
        public Type Type { get; set; }
        public object OldValues { get; set; }
        public object NewValues { get; set; }
        public EntityEntry EntityEntry { get; internal set; }
    }
}
