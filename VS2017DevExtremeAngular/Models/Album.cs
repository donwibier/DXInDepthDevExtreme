using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public partial class Album
    {
        public Album()
        {
            Track = new HashSet<Track>();
        }
		[Key]
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }

        public Artist Artist { get; set; }
        public ICollection<Track> Track { get; set; }
    }
}
