using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public partial class Genre
    {
        public Genre()
        {
            Track = new HashSet<Track>();
        }
		[Key]
        public int GenreId { get; set; }
        public string Name { get; set; }

        public ICollection<Track> Track { get; set; }
    }
}
