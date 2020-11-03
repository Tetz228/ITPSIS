namespace WpfApp1
{
    using System;
    using System.Collections.Generic;
    
    public partial class Dish
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Dish()
        {
            this.Ordering_dishes = new HashSet<Ordering_dishes>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public int Fk_type_dish { get; set; }
        public decimal Price { get; set; }
    
        public virtual Types_dishes Types_dishes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ordering_dishes> Ordering_dishes { get; set; }
    }
}