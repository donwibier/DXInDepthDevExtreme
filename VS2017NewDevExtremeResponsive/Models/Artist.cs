using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public partial class Artist
    {
        public Artist()
        {
            Album = new HashSet<Album>();
        }
		[Key()]
        public int ArtistId { get; set; }
		[Required]
		public string Name { get; set; }

        public ICollection<Album> Album { get; set; }
    }
}
