import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { AppConfigService } from '@core/services/app-config.service';

@Component({
   selector: 'app-sidenav-list',
   templateUrl: './sidenav-list.component.html',
   styleUrls: ['./sidenav-list.component.scss']
})
export class SidenavListComponent implements OnInit {
   @Input() public links: { title: string, url: string };

   @Output() sidenavClose = new EventEmitter();

   ngOnInit() {
   }

   public onSidenavClose = () => {
      this.sidenavClose.emit();
   }
}
