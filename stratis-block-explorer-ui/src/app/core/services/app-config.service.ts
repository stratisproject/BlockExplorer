import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppConfig, CoinConfiguration } from './../models/app-config.model';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { environment } from '@app/../environments/environment';

interface ILink {
   title: string,
   url: string
}

/**
 * Dynamically initializes configuration.
 */
@Injectable({
   providedIn: 'root'
})
export class AppConfigService {
   config: AppConfig;
   links: { title: string, url: string }[];
   $configurationLoaded: BehaviorSubject<boolean>;

   constructor(private http: HttpClient) {
      this.$configurationLoaded = new BehaviorSubject(false);
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
               this.config = <AppConfig>data;

               this.links = [];
               // load links links
               for (let key in this.config.coins) {
                  const coinConfiguration = this.config.coins[key];
                  this.links.push({ title: coinConfiguration.networkName, url: coinConfiguration.url });
               }

               this.$configurationLoaded.next(true);

               resolve(true);
            });
      });
   }

   public getConfiguration(): CoinConfiguration {
      return this.config.coins[this.config.currentCoin];
   }

   public getKnownLinks(): { title: string, url: string }[] {
      return this.links;
   }
}
