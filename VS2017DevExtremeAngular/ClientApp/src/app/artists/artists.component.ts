import { Component } from '@angular/core';
import { Service } from './artists.service';

@Component({
  selector: 'Artists',
  templateUrl: './artists.component.html',
  styleUrls: ['./artists.component.css'],
  providers: [Service]
})

export class ArtistsComponent {
  dataSource: any;

  constructor(service: Service) { 
    this.dataSource = service.getArtist();

  }
}
