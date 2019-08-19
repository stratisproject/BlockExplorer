import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APP_CONFIG, AppConfig } from '@blockexplorer/shared/models';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { environment } from '../../environments/environment';

/**
 * Service in charge of dynamically initialising configuration
 */
@Injectable()
export class AppConfigService {

  constructor(private http: HttpClient) {
  }

  public load() {
    return new Promise((resolve, reject) => {

      this.http.get('/assets/config/config.json')
        .pipe(catchError((error: any): any => {
          reject(true);
          return Observable.throw('Server error');
        }))
        .subscribe((envResponse: any) => {
          const config = new AppConfig();

          if (!environment.production) {
            APP_CONFIG.apiBaseUrl = 'https://cirrusmainindexer1.azurewebsites.net';
          } else {
            APP_CONFIG.apiBaseUrl = envResponse.apiBaseUrl;
          }

          APP_CONFIG.symbol = envResponse.symbol;
          APP_CONFIG.chain = envResponse.chain;
          APP_CONFIG.sidechainColor = envResponse.sidechainColor;
          APP_CONFIG.stratColor = envResponse.stratColor;
          APP_CONFIG.sidechainMainUrl = envResponse.sidechainMainUrl;
          APP_CONFIG.sidechainTestUrl = envResponse.sidechainTestUrl;
          APP_CONFIG.stratMainUrl = envResponse.stratMainUrl;
          APP_CONFIG.stratTestUrl = envResponse.stratTestUrl;

          resolve(true);
        });

    });
  }
}
