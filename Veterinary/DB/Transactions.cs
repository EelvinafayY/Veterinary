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
    
    public partial class Transactions
    {
        public int TransactionId { get; set; }
        public int AppointmentId { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public int PaymentMethodId { get; set; }
    
        public virtual Appointments Appointments { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
    }
}