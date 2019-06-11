import { Injectable } from '@angular/core';
import * as AspNetData from "devextreme-aspnet-data-nojquery";

// Assign the URL of your actual data service to the variable below
const url:string = '/';
const dataSource:any = AspNetData.createStore({
      key: 'artistId', 
      loadUrl: url + 'api/Artists/Get',
      insertUrl: url + 'api/Artists/Post',
      updateUrl: url + 'api/Artists/Put',
      deleteUrl: url + 'api/Artists/Delete',
        onBeforeSend: function(method, ajaxOptions) {
          ajaxOptions.xhrFields = { withCredentials: true };
        }
      });



@Injectable()
export class Service { 
  getArtist() { return dataSource; }

}
