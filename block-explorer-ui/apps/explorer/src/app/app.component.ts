import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { GlobalFacade } from '@blockexplorer/state/global-state';
import { Observable, ReplaySubject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'blockexplorer-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnDestroy {
  title = 'explorer';
  identifiedEntity$: Observable<any>;
  found$: Observable<boolean>;
  found: boolean;
  destroyed$ = new ReplaySubject<any>();
  text = '';

  constructor(private globalFacade: GlobalFacade, private router: Router) {
    this.found$ = this.globalFacade.loaded$;
    this.found$.pipe(takeUntil(this.destroyed$))
      .subscribe(data => {
        this.found = data;
      });
    this.identifiedEntity$ = this.globalFacade.identifiedEntity$;
    this.identifiedEntity$.pipe(takeUntil(this.destroyed$))
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
            this.router.navigate(['blocks', this.text]);
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

  find(text: string) {
    this.text = text;
    this.globalFacade.identify(text);
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
