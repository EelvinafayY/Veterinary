//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Veterinary.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Recipe
    {
        public int RecipeId { get; set; }
        public Nullable<int> RecordId { get; set; }
        public Nullable<int> ItemId { get; set; }
    
        public virtual Inventory Inventory { get; set; }
        public virtual MedicalRecords MedicalRecords { get; set; }
    }
}