import { Component, OnInit, OnDestroy } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { MainState } from '../../store/reducers/main.reducer';
import { Router } from '@angular/router';
import { Observable, ReplaySubject } from 'rxjs';
import * as action from '../../store/actions/main.actions';
import * as selector from '../../store/selectors/main.selectors';
import { takeUntil } from 'rxjs/operators';

@Component({
   selector: 'app-main',
   templateUrl: './main.component.html',
   styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnDestroy {
   identifiedEntity$: Observable<any>;
   found$: Observable<boolean>;
   found: boolean;
   destroyed$ = new ReplaySubject<any>();
   text = '';

   constructor(private store: Store<MainState>, private router: Router) {
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
      this.destroyed$.next(true);
      this.destroyed$.complete();
   }

   find(text: string) {
      this.text = text;
      this.store.dispatch(action.identifyEntity({ text }));
   }
}
