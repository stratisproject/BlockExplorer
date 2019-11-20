import { Component, OnInit, OnDestroy, ViewChild, ChangeDetectorRef, EventEmitter, Output } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { MainState } from '../../store/main.reducer';
import { Router } from '@angular/router';
import { Observable, ReplaySubject } from 'rxjs';
import * as action from '../../store/main.actions';
import * as selector from '../../store/main.selectors';
import { takeUntil } from 'rxjs/operators';
import { AppConfigService } from '@core/services/app-config.service';

@Component({
   selector: 'app-main',
   templateUrl: './main.component.html',
   styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnDestroy {
   private mobileQueryListener: () => void;

   title = 'Stratis Block Explorer';
   mobileQuery: MediaQueryList;

   identifiedEntity$: Observable<any>;
   found$: Observable<boolean>;
   found: boolean;
   destroyed$ = new ReplaySubject<any>();
   text = '';

   @Output() public sidenavToggle = new EventEmitter();

   public links: { title: string, url: string }[];

   constructor(appConfig: AppConfigService, private store: Store<MainState>, private router: Router) {

      this.links = appConfig.getKnownLinks();

      this.found$ = this.store.pipe(select(selector.getLoaded));
      this.found$
         .pipe(takeUntil(this.destroyed$))
         .subscribe(data => this.found = data);

      this.identifiedEntity$ = this.store.pipe(select(selector.getIdentifiedEntity));
      this.identifiedEntity$
         .pipe(takeUntil(this.destroyed$))
         .subscribe(entity => {
            let type = 'UNKNOWN';
            if (entity === null) {
               this.router.navigate(['search', 'not-found']);
               return;
            }

            if (entity === undefined) return;

            if (!!entity.type) type = entity.type;
            if (!!entity.transaction) type = 'TRANSACTION';
            if (!!entity.additionalInformation) type = 'BLOCK';

            switch (type) {
               case 'PUBKEY_ADDRESS':
               case 'SCRIPT_ADDRESS':
                  this.router.navigate(['addresses', this.text]);
                  break;
               case 'TRANSACTION':
                  this.router.navigate(['transactions', this.text]);
                  break;
               case 'BLOCK':
                  this.router.navigate(['block', this.text]);
                  break;
               case 'SMART_CONTRACT':
                  this.router.navigate(['smartcontracts', this.text]);
                  break;
               default:
                  this.router.navigate(['search', 'not-found']);
                  break;
            }
         });
   }

   ngOnDestroy(): void {
      this.mobileQuery.removeListener(this.mobileQueryListener);

      this.destroyed$.next(true);
      this.destroyed$.complete();
   }

   public find(text: string) {
      this.text = text;
      this.store.dispatch(action.identifyEntity({ text }));
   }

   public onToggleSidenav = () => {
      this.sidenavToggle.emit();
   }
}
