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
    public class ArtistsController : Controller
    {
        private ChinookContext _context;

        public ArtistsController(ChinookContext context) {
            this._context = context;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions) {
            var artist = _context.Artist.Select(i => new {
                i.ArtistId,
                i.Name
            });
            return Json(DataSourceLoader.Load(artist, loadOptions));
        }

        [HttpPost]
        public IActionResult Post(string values) {
            var model = new Artist();
            var _values = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, _values);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Artist.Add(model);
            _context.SaveChanges();

            return Json(result.Entity.ArtistId);
        }

        [HttpPut]
        public IActionResult Put(int key, string values) {
            var model = _context.Artist.FirstOrDefault(item => item.ArtistId == key);
            if(model == null)
                return StatusCode(409, "Artist not found");

            var _values = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, _values);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        public void Delete(int key) {
            var model = _context.Artist.FirstOrDefault(item => item.ArtistId == key);

            _context.Artist.Remove(model);
            _context.SaveChanges();
        }


        private void PopulateModel(Artist model, IDictionary values) {
            string ARTIST_ID = nameof(Artist.ArtistId);
            string NAME = nameof(Artist.Name);

            if(values.Contains(ARTIST_ID)) {
                model.ArtistId = Convert.ToInt32(values[ARTIST_ID]);
            }

            if(values.Contains(NAME)) {
                model.Name = Convert.ToString(values[NAME]);
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