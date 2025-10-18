using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetWbapi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using DotNetWbapi.Dtos;
using Microsoft.EntityFrameworkCore;


namespace DotNetWbapi.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        // public DbSet<DeliveryChallan> DeliveryChallans { get; set; }
        public DbSet<GatepassHeader> GatepassHeaders { get; set; }
        // public DbSet<GatepassDetail> GatepassDetails { get; set; }
        public DbSet<DeliveryChallanHeaderCreation> DeliveryChallanHeaderCreation { get; set; }
        // public DbSet<DeliveryChallanDetailCreation> DeliveryChallanDetailCreation { get; set; }
        // public DbSet<PendingDeliveryItemCreation> PendingDeliveryItemCreation { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<DeclarationSetting> DeclarationSettings { get; set; }
         public DbSet<PackagingList> PackagingLists { get; set; }



          public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateMapping> TemplateMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Existing configurations
            
            // Configure Template relationships
            modelBuilder.Entity<Template>()
                .HasMany(t => t.Mappings)
                .WithOne(m => m.Template)
                .HasForeignKey(m => m.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
     
    }
}