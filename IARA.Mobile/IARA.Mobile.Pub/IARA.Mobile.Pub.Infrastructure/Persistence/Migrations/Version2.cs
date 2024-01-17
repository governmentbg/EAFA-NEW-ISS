using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;
using IARA.Mobile.Pub.Infrastructure.Persistence.Migrations.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.Mobile.Pub.Infrastructure.Persistence.Migrations
{
    public class Version2 : IVersion
    {
        public void Migrate(AppDbContext context)
        {
            context.CreateTable<NTerritorialUnit>();
        }
    }
}
