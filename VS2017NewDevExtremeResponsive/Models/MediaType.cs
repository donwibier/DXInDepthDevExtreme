using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public partial class MediaType
    {
        public MediaType()
        {
            Track = new HashSet<Track>();
        }
		[Key]
        public int MediaTypeId { get; set; }
        public string Name { get; set; }

        public ICollection<Track> Track { get; set; }
    }
}
