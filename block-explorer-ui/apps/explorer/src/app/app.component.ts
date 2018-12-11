import { Component, OnDestroy } from '@angular/core';
import { GlobalFacade } from '@blockexplorer/state/global-state';
import { Observable, ReplaySubject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'blockexplorer-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnDestroy {
  title = 'explorer';
  identifiedEntity$: Observable<any>;
  destroyed$ = new ReplaySubject<any>();

  constructor(private globalFacade: GlobalFacade, private router: Router) {

  }

  find(text: string){
    this.identifiedEntity$ = this.globalFacade.identifiedEntity$;
    this.identifiedEntity$.pipe(takeUntil(this.destroyed$))
        .subscribe(entity => {
          if (!entity) return;
          if (!entity.type) return;

          switch (<string>entity.type) {
            case 'PUBKEY_ADDRESS':
              this.router.navigate(['addresses', text]);
              break;
            default:
              break;
          }
        });

    this.globalFacade.identify(text);
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
