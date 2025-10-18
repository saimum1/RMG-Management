using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DotNetWbapi.Models
{
    public class PackagingList
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Main Form Section
        [Required]
        [StringLength(100)]
        public string Company { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Buyer { get; set; } = string.Empty;

        [StringLength(50)]
        public string Style { get; set; } = string.Empty;

        [StringLength(50)]
        public string PurchaseOrder { get; set; } = string.Empty;

        [StringLength(50)]
        public string StyleBuyer { get; set; } = string.Empty;

        [StringLength(50)]
        public string ShippedBy { get; set; } = string.Empty;

        [StringLength(100)]
        public string ShortDescription { get; set; } = string.Empty;

        [StringLength(50)]
        public string Size { get; set; } = string.Empty;

        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [StringLength(100)]
        public string Packing { get; set; } = string.Empty;

        [StringLength(50)]
        public string PcsPerSet { get; set; } = string.Empty;

        [StringLength(50)]
        public string ProductNo { get; set; } = string.Empty;

        [StringLength(50)]
        public string CountryOfOrigin { get; set; } = string.Empty;

        [StringLength(50)]
        public string PortOfLoading { get; set; } = string.Empty;

        [StringLength(50)]
        public string CountryOfDestination { get; set; } = string.Empty;

        [StringLength(50)]
        public string PortOfDischarge { get; set; } = string.Empty;

        [StringLength(50)]
        public string Incoterm { get; set; } = string.Empty;

        [StringLength(50)]
        public string OrmsNo { get; set; } = string.Empty;

        [StringLength(50)]
        public string LmkgNo { get; set; } = string.Empty;

        [StringLength(50)]
        public string OrmsStyleNo { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "draft";

        // Buyer Specific Fields
        [StringLength(50)]
        public string ItemNo { get; set; } = string.Empty;

        [StringLength(50)]
        public string Pod { get; set; } = string.Empty;

        [StringLength(50)]
        public string Boi { get; set; } = string.Empty;

        [StringLength(50)]
        public string Wwwk { get; set; } = string.Empty;

        [StringLength(50)]
        public string NoOfColor { get; set; } = string.Empty;

        [StringLength(50)]
        public string KeyCode { get; set; } = string.Empty;

        [StringLength(50)]
        public string SupplierCode { get; set; } = string.Empty;

        // Additional Fields
        [StringLength(50)]
        public string Cartons { get; set; } = string.Empty;

        [StringLength(50)]
        public string OrderQtyPac { get; set; } = string.Empty;

        [StringLength(50)]
        public string OrderQtyPcs { get; set; } = string.Empty;

        [StringLength(50)]
        public string ShipQtyPac { get; set; } = string.Empty;

        [StringLength(50)]
        public string ShipQtyPcs { get; set; } = string.Empty;

        [StringLength(50)]
        public string Destination { get; set; } = string.Empty;

        // JSON fields for complex data
        [Required]
        public string PackingDetailsJson { get; set; } = string.Empty;

        [Required]
        public string DeclarationAnswersJson { get; set; } = string.Empty;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Helper methods for JSON serialization
        public List<PackingDetail> GetPackingDetails()
        {
            if (string.IsNullOrEmpty(PackingDetailsJson))
                return new List<PackingDetail>();
            
            try
            {
                return JsonSerializer.Deserialize<List<PackingDetail>>(PackingDetailsJson) ?? new List<PackingDetail>();
            }
            catch
            {
                return new List<PackingDetail>();
            }
        }

        public void SetPackingDetails(List<PackingDetail> details)
        {
            try
            {
                PackingDetailsJson = JsonSerializer.Serialize(details);
            }
            catch
            {
                PackingDetailsJson = "[]";
            }
        }

        public List<DeclarationAnswer> GetDeclarationAnswers()
        {
            if (string.IsNullOrEmpty(DeclarationAnswersJson))
                return new List<DeclarationAnswer>();
            
            try
            {
                return JsonSerializer.Deserialize<List<DeclarationAnswer>>(DeclarationAnswersJson) ?? new List<DeclarationAnswer>();
            }
            catch
            {
                return new List<DeclarationAnswer>();
            }
        }

        public void SetDeclarationAnswers(List<DeclarationAnswer> answers)
        {
            try
            {
                DeclarationAnswersJson = JsonSerializer.Serialize(answers);
            }
            catch
            {
                DeclarationAnswersJson = "[]";
            }
        }
    }

    public class PackingDetail
    {
        public int Sl { get; set; }
        public string NoOfCarton { get; set; } = string.Empty;
        public string Start { get; set; } = string.Empty;
        public string End { get; set; } = string.Empty;
        public string SizeName { get; set; } = string.Empty;
        public string Ratio { get; set; } = string.Empty;
        public string ArticleNo { get; set; } = string.Empty;
        public string PcsPack { get; set; } = string.Empty;
        public string PacCarton { get; set; } = string.Empty;
        public string OrderQty { get; set; } = string.Empty;
        public string TotalPcs { get; set; } = string.Empty;
        public string TotalPacs { get; set; } = string.Empty;
        public string GWt { get; set; } = string.Empty;
        public string NWt { get; set; } = string.Empty;
        public string TotalWt { get; set; } = string.Empty;
        public string L { get; set; } = string.Empty;
        public string W { get; set; } = string.Empty;
        public string H { get; set; } = string.Empty;
        public string CBM { get; set; } = string.Empty;
    }



    public class PackingDetailForExport
{
    public string NoOfCarton { get; set; } = string.Empty;
    public string Start { get; set; } = string.Empty;
    public string End { get; set; } = string.Empty;
    public string SizeName { get; set; } = string.Empty;
    public string Ratio { get; set; } = string.Empty;
    public string ArticleNo { get; set; } = string.Empty;
    public string PcsPack { get; set; } = string.Empty;
    public string PacCarton { get; set; } = string.Empty;
    public string OrderQty { get; set; } = string.Empty;
    public string TotalPcs { get; set; } = string.Empty;
    public string TotalPacs { get; set; } = string.Empty;
    public string GWt { get; set; } = string.Empty;
    public string NWt { get; set; } = string.Empty;
    public string TotalWt { get; set; } = string.Empty;
    public string L { get; set; } = string.Empty;
    public string W { get; set; } = string.Empty;
    public string H { get; set; } = string.Empty;
    public string CBM { get; set; } = string.Empty;
}

    public class DeclarationAnswer
    {
        public int QuestionNumber { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public DateTime? AnswerDate { get; set; }
    }
}