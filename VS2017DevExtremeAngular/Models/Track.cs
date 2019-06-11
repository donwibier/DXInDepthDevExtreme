using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public partial class Track
    {
        public Track()
        {
            InvoiceLine = new HashSet<InvoiceLine>();
            PlaylistTrack = new HashSet<PlaylistTrack>();
        }
		[Key]
        public int TrackId { get; set; }
        public string Name { get; set; }
        public int? AlbumId { get; set; }
        public int MediaTypeId { get; set; }
        public int? GenreId { get; set; }
        public string Composer { get; set; }
        public int Milliseconds { get; set; }
        public int? Bytes { get; set; }
        public decimal UnitPrice { get; set; }

        public Album Album { get; set; }
        public Genre Genre { get; set; }
        public MediaType MediaType { get; set; }
        public ICollection<InvoiceLine> InvoiceLine { get; set; }
        public ICollection<PlaylistTrack> PlaylistTrack { get; set; }
    }
}
