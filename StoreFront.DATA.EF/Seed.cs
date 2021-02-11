//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StoreFront.DATA.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class Seed
    {
        public int SeedID { get; set; }
        public string CommonName { get; set; }
        public string ScientificName { get; set; }
        public string Description { get; set; }
        public string PlantingInstructions { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public int SpacingID { get; set; }
        public int UnitsID { get; set; }
        public int CycleID { get; set; }
        public int ProductID { get; set; }
        public int SproutID { get; set; }
        public int SeasonID { get; set; }
        public int CategoryID { get; set; }
        public int SunID { get; set; }
        public int TempID { get; set; }
        public int GeneID { get; set; }
        public int DepthID { get; set; }
        public int FrostID { get; set; }
        public byte[] ImageURL { get; set; }
    
        public virtual FrostHardy FrostHardy { get; set; }
        public virtual GeneType GeneType { get; set; }
        public virtual IdealTemp IdealTemp { get; set; }
        public virtual LifeCycle LifeCycle { get; set; }
        public virtual MinFullSun MinFullSun { get; set; }
        public virtual PlantSpacing PlantSpacing { get; set; }
        public virtual Product Product { get; set; }
        public virtual Season Season { get; set; }
        public virtual SeedCategory SeedCategory { get; set; }
        public virtual SeedDepth SeedDepth { get; set; }
        public virtual SproutIn SproutIn { get; set; }
        public virtual UnitsPerPacket UnitsPerPacket { get; set; }
    }
}