import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { Observable, throwError as _observableThrow, of as _observableOf, of } from 'rxjs';
import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';
import { blobToText, throwException, API_BASE_URL } from '@blockexplorer/shared/models';

@Injectable()
export class FinderService {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : "";
    }

    /**
     * @return Success
     */
    whatIsIt(identifier: string): Observable<any> {
      let url_ = this.baseUrl + "/api/v1/finder/{identifier}";
      if (identifier === undefined || identifier === null)
          throw new Error("The parameter 'identifier' must be defined.");
      url_ = url_.replace("{identifier}", encodeURIComponent("" + identifier));
      url_ = url_.replace(/[?&]$/, "");

      const options_ : any = {
          observe: "response",
          responseType: "blob",
          headers: new HttpHeaders({
          })
      };

      return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
          return this.processWhatIsIt(response_);
      })).pipe(_observableCatch((response_: any) => {
          if (response_ instanceof HttpResponseBase) {
              try {
                  return this.processWhatIsIt(<any>response_);
              } catch (e) {
                  return <Observable<any>><any>_observableThrow(e);
              }
          } else
              return <Observable<any>><any>_observableThrow(response_);
      }));
  }

  protected processWhatIsIt(response: HttpResponseBase): Observable<any> {
      const status = response.status;
      const responseBlob =
          response instanceof HttpResponse ? response.body :
          (<any>response).error instanceof Blob ? (<any>response).error : undefined;

      const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
      if (status === 200) {
          return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
          return _observableOf<void>(<any>null);
          }));
      } else if (status !== 200 && status !== 204) {
          return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
          return throwException("An unexpected server error occurred.", status, _responseText, _headers);
          }));
      }
      return _observableOf<any>(<any>null);
  }
}
