import { Injectable } from '@angular/core';
import * as AspNetData from "devextreme-aspnet-data-nojquery";

// Assign the URL of your actual data service to the variable below
const url:string = '/';
const dataSource:any = AspNetData.createStore({
      key: 'AlbumId', 
      loadUrl: url + 'api/Albums/Get',
      insertUrl: url + 'api/Albums/Post',
      updateUrl: url + 'api/Albums/Put',
      deleteUrl: url + 'api/Albums/Delete',
        onBeforeSend: function(method, ajaxOptions) {
          ajaxOptions.xhrFields = { withCredentials: true };
        }
      });


const artistLookupData = AspNetData.createStore({
      key: "Value",
      loadUrl: url + 'api/Albums/ArtistLookup',
      onBeforeSend: function(method, ajaxOptions) {
        ajaxOptions.xhrFields = { withCredentials: true };
      }
    });


@Injectable()
export class Service { 
  getAlbum() { return dataSource; }
  getArtistLookup() { return artistLookupData; }

}
