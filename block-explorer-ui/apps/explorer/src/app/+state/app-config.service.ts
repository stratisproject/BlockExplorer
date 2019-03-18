import { IndentifyEntity } from './../../../../../libs/state/global-state/src/lib/+state/global.actions';
import { Inject, Injectable, InjectionToken } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {catchError} from 'rxjs/operators';
import { Observable } from 'rxjs';
import { APP_CONFIG, AppConfig } from '@blockexplorer/shared/models';

/**
 * Service in charge of dynamically initialising configuration
 */
@Injectable()
export class AppConfigService
{

  constructor(private http: HttpClient)
  {
  }

  public load()
  {
    return new Promise((resolve, reject) => {

      this.http.get('/assets/config/config.json')
          .pipe(catchError((error: any): any => {
                    reject(true);
                    return Observable.throw('Server error');
                }))
          .subscribe((envResponse :any) => {
                const config = new AppConfig();
                
                if (!environment.production) {
                    APP_CONFIG.apiBaseUrl = 'http://localhost:5000';
                } else {
                    APP_CONFIG.apiBaseUrl = envResponse.apiBaseUrl;
                }

                resolve(true);
            });

    });
  }
}