import { mergeMap, catchError } from 'rxjs/operators';
import { Observable, throwError, of } from 'rxjs';
import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';
import { AppConfigService } from '@core/services/app-config.service';
import *  as utils from '@shared/utils';
import { SmartContractAction, StandardToken } from '../models';

@Injectable({
    providedIn: "root"
})
export class SmartContractService {
    public apiBaseUrl: string;

    constructor(private appConfig: AppConfigService, private http: HttpClient) {
        appConfig.$configurationLoaded.subscribe(value => {
            if (value === true) {
                this.apiBaseUrl = this.appConfig.getConfiguration().apiBaseUrl + "/api/v1/smartcontracts/"
            }
        });
    }

    /**
     * @param txId hash of the transaction where the smart contract action happened.
     * @return Success
     */
    getSmartContractAction(txId: string): Observable<SmartContractAction> {
        let url_ = this.apiBaseUrl + "action/{txId}?";

        if (txId === undefined || txId === null)
            throw new Error("The parameter 'txId' must be defined.");
        url_ = url_.replace("{txId}", encodeURIComponent("" + txId));

        url_ = url_.replace(/[?&]$/, "");

        const options_: any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.get(url_, options_)
            .pipe(
                mergeMap((response_: any) => this.processSmartContractAction(response_)),
                catchError((response_: any) => {
                    if (response_ instanceof HttpResponseBase) {
                        try {
                            return this.processSmartContractAction(<any>response_);
                        } catch (e) {
                            return <Observable<SmartContractAction>><any>throwError(e);
                        }
                    } else
                        return <Observable<SmartContractAction>><any>throwError(response_);
                })
            );
    }

    getStandardTokens(from: number = 0, take: number = 10): Observable<StandardToken[]> {
        let url_ = this.apiBaseUrl + `?from=${from}&take=${take}`;
        url_ = url_.replace(/[?&]$/, "");

        const options_: any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_)
            .pipe(
                mergeMap((response_: any) => this.processStandardToken(response_)),
                catchError((response_: any) => {
                    if (response_ instanceof HttpResponseBase) {
                        try {
                            return this.processStandardToken(<any>response_);
                        } catch (e) {
                            return <Observable<StandardToken[]>><any>throwError(e);
                        }
                    } else
                        return <Observable<StandardToken[]>><any>throwError(response_);
                })
            );
    }

    protected processSmartContractAction(response: HttpResponseBase): Observable<SmartContractAction> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
                (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); } };
        if (status === 200) {
            return utils.blobToText(responseBlob)
                .pipe(
                    mergeMap(_responseText => {
                        let result200: SmartContractAction = null;
                        const resultData200 = _responseText === "" ? null : JSON.parse(_responseText);
                        result200 = resultData200 ? <SmartContractAction>(resultData200) : new SmartContractAction();
                        return of(result200);
                    })
                );
        } else if (status !== 200 && status !== 204) {
            return utils.blobToText(responseBlob)
                .pipe(
                    mergeMap(_responseText => utils.throwException("An unexpected server error occurred.", status, _responseText, _headers))
                );
        }
        return of<SmartContractAction>(<any>null);
    }

    protected processStandardToken(response: HttpResponseBase): Observable<StandardToken[]> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
                (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); } };
        if (status === 200) {
            return utils.blobToText(responseBlob)
                .pipe(
                    mergeMap(_responseText => {
                        let result200: any = null;
                        const resultData200 = _responseText === "" ? null : JSON.parse(_responseText);

                        result200 = resultData200 ? resultData200.map(r => <StandardToken>r) : [];
                        return of(result200);
                    })
                );
        } else if (status !== 200 && status !== 204) {
            return utils.blobToText(responseBlob)
                .pipe(
                    mergeMap(_responseText => utils.throwException("An unexpected server error occurred.", status, _responseText, _headers))
                );
        }
        return of<StandardToken[]>(<any>null);
    }
}
