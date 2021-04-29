using CQRSMeetup.Core.Enums;
using CQRSMeetup.WriteDomain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace CQRSMeetup.WriteApi.Infrastructures.Database
{
    public class SeedDataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CQRSMeetupDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<CQRSMeetupDbContext>>()))
            {
                // Look for any board games.
                if (context.StockEntities.Any())
                {
                    return;   // Data was already seeded
                }
                for (int i = 1; i < 10; i++)
                {
                    var tempLocation = new LocationEntity();
                    tempLocation.Code = "LocationCode" + i;
                    tempLocation.Description = "LocationDescription" + i;
                    tempLocation.Add();
                    context.Add(tempLocation);
                }
                for (int i = 1; i < 10; i++)
                {
                    var tempProduct = new ProductEntity();
                    tempProduct.Code = "ProductCode" + i;
                    tempProduct.Description = "ProductDescription" + i;
                    tempProduct.SerialNo = "SerialNo" + i;
                    tempProduct.Add();
                    context.Add(tempProduct);
                }
                for (int i = 1; i < 5; i++)
                {
                    var tempStock = new StockEntity();
                    tempStock.LocationId = i;
                    tempStock.ProductId = i;
                    tempStock.StockStatusId = StockStatuses.STANDARD;
                    tempStock.Quantity = i;
                    tempStock.Add();
                    context.Add(tempStock);
                }
                context.SaveChanges();

                var test = context.StockEntities.Include(x => x.Product).Include(x => x.Location).ToList();
                var a = JsonConvert.SerializeObject(test.FirstOrDefault());
            }
        }
    }
}
