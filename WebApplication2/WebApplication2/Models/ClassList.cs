//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClassList
    {
        public int Classid { get; set; }
        public int Studentid { get; set; }
        public int Id { get; set; }
    
        public virtual Class Class { get; set; }
        public virtual Student Student { get; set; }
    }
}
