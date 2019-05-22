using System;
using System.Collections.Generic;

namespace CarBase.Business
{

    public partial class Model
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Model()
        {
            Versions = new List<CarVersion>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int? PriceFrom { get; set; }

        public string Link { get; set; }

        public int BrandId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<CarVersion> Versions { get; set; }
    }
}
