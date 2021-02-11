using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StoreFront.DATA.EF//.Metadata
{

    #region FrostHardy Metadata
    public class FrostHardyMetadata
    {
        [Required(ErrorMessage = "* Required.")]
        [Display(Name = "Frost Hardy")]
        public bool HardyType { get; set; }
    }

    [MetadataType(typeof(FrostHardyMetadata))]

    public partial class FrostHardy { }

    #endregion

    #region GeneType MetaData
    public class GeneTypeMetadata
    {
        [Required(ErrorMessage = "* Required.")]
        [StringLength(10, ErrorMessage = "* The value must be 10 characters or less.")]
        [Display(Name ="Gene Type")]
        public string GeneName { get; set; }
    }

    [MetadataType(typeof(GeneTypeMetadata))]

    public partial class GeneType { }

    #endregion

    #region IdealTemps MetaData
    public class IdealTempsMetadata
    {
        [Required(ErrorMessage = "* Required")]
        [StringLength(8, ErrorMessage = "* The value must be 8 characters or less.")]
        [Display(Name = "Temperature")]
        public string Temp { get; set; }
    }
    [MetadataType(typeof(IdealTempsMetadata))]
    public partial class IdealTemp { }
    #endregion

    #region LifeCycle MetaData
    public class LifeCycleMetadata
    {
        [Required(ErrorMessage = "* Required")]
        [StringLength(10, ErrorMessage = "* The value must be 10 characters or less.")]
        [Display(Name = "Life Cycle")]
        public string CycleType { get; set; }
    }
    [MetadataType(typeof(LifeCycleMetadata))]
    public partial class LifeCycle { }
    #endregion
    
    #region MinFullSun MetaData
    public class MinFullSunMetadata
    {
        [Required(ErrorMessage = "* Required")]
        [StringLength(10, ErrorMessage = "* The value must be 10 characters or less.")]
        [Display(Name = "Sun")]
        public string SunTime { get; set; }
    }
    [MetadataType(typeof(MinFullSunMetadata))]
    public partial class MinFullSun { }
    #endregion
    
    #region PlantSpacing MetaData
    public class PlantSpacingMetadata
    {
        [Required(ErrorMessage = "* Required")]
        [StringLength(10, ErrorMessage = "* The value must be 10 characters or less.")]
        [Display(Name = "Plant Spacing")]
        public string Spacing { get; set; }
    }
    [MetadataType(typeof(PlantSpacingMetadata))]
    public partial class PlantSpacing { }
    #endregion
    
    #region Product MetaData
    public class ProductMetadata
    {
        [Required(ErrorMessage = "* Required")]
        [StringLength(20, ErrorMessage = "* The value must be 20 characters or less.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
    }
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product { }
    #endregion
    
    #region Season MetaData
    public class SeasonMetadata
    {
        [Required(ErrorMessage = "* Required")]
        [StringLength(10, ErrorMessage = "* The value must be 10 characters or less.")]
        [Display(Name = "Planting Season")]
        public string SeasonType { get; set; }
    }
    [MetadataType(typeof(SeasonMetadata))]
    public partial class Season { }
    #endregion
    
    #region SeedCategory MetaData
    public class SeedCategoryMetadata
    {
        [Required(ErrorMessage = "* Required")]
        [StringLength(10, ErrorMessage = "* The value must be 10 characters or less.")]
        [Display(Name = "Plant Category")]
        public string CategoryName { get; set; }
    }
    [MetadataType(typeof(SeedCategoryMetadata))]
    public partial class SeedCategory { }
    #endregion
    
    #region Seed MetaData
    public class SeedMetadata
    {
        [Required(ErrorMessage = "* Required")]
        [StringLength(100, ErrorMessage = "* The value must be 100 characters or less.")]
        [Display(Name = "Plant Name")]
        public string CommonName { get; set; }

        [StringLength(150, ErrorMessage = "* The value must be 150 characters or less.")]
        [Display(Name = "Plant Name")]
        [DisplayFormat(NullDisplayText = "[N/A]")]
        public string ScientificName { get; set; }
                
        [Display(Name = "Description")]
        [DisplayFormat(NullDisplayText = "[N/A]")]
        [UIHint("MultilineText")]
        public string Description { get; set; }
        
        [Display(Name = "Planting Instructions")]
        [DisplayFormat(NullDisplayText = "[N/A]")]
        [UIHint("MultilineText")]
        public string PlantingInstructions { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "* Value must be a valid number, 0 or larger")]
        [DisplayFormat(DataFormatString = "{0:c}", NullDisplayText = "[N/A]")]
        public Nullable<decimal> Cost { get; set; }

        [Display(Name = "Image")]
        public byte[] ImageURL { get; set; }
    }
    [MetadataType(typeof(SeedMetadata))]
    public partial class Seed { }
    #endregion
    
    #region SeedDepth MetaData
    public class SeedDepthMetadata
    {
        [Display(Name = "Sow Depth")]
        [Required(ErrorMessage = "* Required")]
        [Range(0, double.MaxValue, ErrorMessage = "* Value must be a valid number, 0 or larger")]
        public decimal Depth { get; set; }
    }
    [MetadataType(typeof(SeedDepthMetadata))]
    public partial class SeedDepth { }

    #endregion
    
    #region SproutIn MetaData
    public class SproutInMetadata
    {
        [Display(Name = "Sprouts-In")]
        [Required(ErrorMessage = "* Required")]
        [Range(1, double.MaxValue, ErrorMessage = "* Value must be a valid number, 0 or larger")]
        public string SproutDays { get; set; }
    }
    [MetadataType(typeof(SproutInMetadata))]
    public partial class SproutIn { }
    #endregion
    
    #region UnitsPerPacket MetaData
    public class UnitsPerPacketMetadata
    {

        [Display(Name = "Seeds per Pack")]
        [Required(ErrorMessage = "* Required")]
        [Range(1, int.MaxValue, ErrorMessage = "* Value must be a valid number, 1 or larger")]
        public int Quantity { get; set; }
    }
    [MetadataType(typeof(UnitsPerPacketMetadata))]
    public partial class UnitsPerPacket { }
    #endregion
}//end namespace
