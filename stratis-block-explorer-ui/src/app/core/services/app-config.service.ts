import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppConfig } from './../models/app-config.model';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { environment } from '@app/../environments/environment';


/**
 * Dynamically initializes configuration.
 */
@Injectable({
  providedIn: 'root'
})
export class AppConfigService {
  public Config: AppConfig;

  constructor(private http: HttpClient) {
  }

  public load() {
    return new Promise((resolve, reject) => {
      this.http.get(environment.configFile)
        .pipe(
          catchError((error: any): any => {
            reject(true);
            return Observable.throw('Server error');
          }))
        .subscribe((data: any) => {
          this.Config = <AppConfig>data;

          resolve(true);
        });
    });
  }
}
