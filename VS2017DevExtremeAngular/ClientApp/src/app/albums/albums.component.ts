import { Component } from '@angular/core';
import { Service } from './albums.service';

@Component({
  selector: 'albums',
  templateUrl: './albums.component.html',
  styleUrls: ['./albums.component.css'],
  providers: [Service]
})

export class AlbumsComponent {
  dataSource: any;
  artistLookupData: any;

  constructor(service: Service) { 
    this.dataSource = service.getAlbum();
    this.artistLookupData = service.getArtistLookup();

  }
}
