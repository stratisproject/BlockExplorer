import { mergeMap, catchError } from 'rxjs/operators';
import { Observable, throwError, of } from 'rxjs';
import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';
import { AppConfigService } from '../../core/services/app-config.service';
import *  as utils from '@shared/utils';

@Injectable({
  providedIn: "root"
})
export class FinderService {
  protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

  constructor(private appConfig: AppConfigService, private http: HttpClient) { }

  /**
   * @return Success
   */
  whatIsIt(identifier: string): Observable<any> {
    let url_ = this.appConfig.Config.apiBaseUrl + "/api/v1/finder/{identifier}";

    if (identifier === undefined || identifier === null)
      throw new Error("The parameter 'identifier' must be defined.");

    url_ = url_
      .replace("{identifier}", encodeURIComponent("" + identifier))
      .replace(/[?&]$/, "");

    const options_: any = {
      observe: "response",
      responseType: "blob",
      headers: new HttpHeaders({
      })
    };

    return this.http.request("get", url_, options_)
      .pipe(mergeMap((response_: any) => {
        return this.processWhatIsIt(response_);
      }))
      .pipe(
        catchError((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
            try {
              return this.processWhatIsIt(<any>response_);
            } catch (e) {
              return <Observable<any>><any>throwError(e);
            }
          } else
            return <Observable<any>><any>throwError(response_);
        })
      );
  }

  protected processWhatIsIt(response: HttpResponseBase): Observable<any> {
    const status = response.status;
    const responseBlob =
      response instanceof HttpResponse ? response.body :
        (<any>response).error instanceof Blob ? (<any>response).error : undefined;

    const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); } };
    if (status === 200) {
      return utils.blobToText(responseBlob).pipe(mergeMap(_responseText => {
        return of<any>(<any>JSON.parse(_responseText));
      }));
    } else if (status !== 200 && status !== 204) {
      return utils.blobToText(responseBlob).pipe(mergeMap(_responseText => {
        return utils.throwException("An unexpected server error occurred.", status, _responseText, _headers);
      }));
    }
    return of<any>(<any>responseBlob);
  }
}
