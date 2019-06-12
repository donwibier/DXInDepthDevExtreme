using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Models.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AlbumsController : Controller
    {
        private ChinookContext _context;

        public AlbumsController(ChinookContext context) {
            this._context = context;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions) {
            var album = _context.Album.Select(i => new {
                i.AlbumId,
                i.Title,
                i.ArtistId
            });
            return Json(DataSourceLoader.Load(album, loadOptions));
        }

        [HttpPost]
        public IActionResult Post(string values) {
            var model = new Album();
            var _values = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, _values);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Album.Add(model);
            _context.SaveChanges();

            return Json(result.Entity.AlbumId);
        }

        [HttpPut]
        public IActionResult Put(int key, string values) {
            var model = _context.Album.FirstOrDefault(item => item.AlbumId == key);
            if(model == null)
                return StatusCode(409, "Album not found");

            var _values = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, _values);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        public void Delete(int key) {
            var model = _context.Album.FirstOrDefault(item => item.AlbumId == key);

            _context.Album.Remove(model);
            _context.SaveChanges();
        }


        [HttpGet]
        public IActionResult ArtistLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Artist
                         orderby i.Name
                         select new {
                             Value = i.ArtistId,
                             Text = i.Name
                         };
            return Json(DataSourceLoader.Load(lookup, loadOptions));
        }

        private void PopulateModel(Album model, IDictionary values) {
            string ALBUM_ID = nameof(Album.AlbumId);
            string TITLE = nameof(Album.Title);
            string ARTIST_ID = nameof(Album.ArtistId);

            if(values.Contains(ALBUM_ID)) {
                model.AlbumId = Convert.ToInt32(values[ALBUM_ID]);
            }

            if(values.Contains(TITLE)) {
                model.Title = Convert.ToString(values[TITLE]);
            }

            if(values.Contains(ARTIST_ID)) {
                model.ArtistId = Convert.ToInt32(values[ARTIST_ID]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}