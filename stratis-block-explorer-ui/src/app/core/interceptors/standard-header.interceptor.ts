import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfigService } from '../services/app-config.service';
import { LogService } from '../services/logger.service';

@Injectable()
export class StandardHeaderInterceptor implements HttpInterceptor {
    public apiBaseUrl: string;

    constructor(appConfig: AppConfigService, private logger: LogService) {
        appConfig.$configurationLoaded.subscribe(value => {
            if (value === true) {
                this.apiBaseUrl = appConfig.getConfiguration().apiBaseUrl + "/api/v1/"
            }
        });
    }

    intercept(
        req: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        if (!req.headers.has('Content-Type')) {
            req = req.clone({
                headers: req.headers.set('Content-Type', 'application/json')
            });
        }

        let url = req.url;

        if (!req.headers.has('NoUrlRewrite')) {
            //do not intercept the AppConfig requests
            url = `${this.apiBaseUrl}${req.url}`
        }

        req = req.clone({
            headers: req.headers.set('Accept', 'application/json'),
            url: url
        });

        this.logger.debug(`called url ${url}`);

        return next.handle(req);
    }
}
