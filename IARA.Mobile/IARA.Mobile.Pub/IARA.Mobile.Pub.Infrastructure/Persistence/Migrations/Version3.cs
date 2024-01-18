using IARA.Mobile.Pub.Domain.Entities.FishingTicket;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;
using IARA.Mobile.Pub.Infrastructure.Persistence.Migrations.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.Mobile.Pub.Infrastructure.Persistence.Migrations
{
    public class Version3 : IVersion
    {
        public void Migrate(AppDbContext context)
        {
            context.DropTable<FishingTicket>();
            context.CreateTable<FishingTicket>();
        }
    }
}
