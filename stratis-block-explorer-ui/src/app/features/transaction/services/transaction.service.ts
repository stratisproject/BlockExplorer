import { mergeMap, catchError } from 'rxjs/operators';
import { Observable, throwError, of } from 'rxjs';
import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';
import { AppConfigService } from '@core/services/app-config.service';
import *  as utils from '@shared/utils';
import { Transaction, TransactionModel } from '../models';
import { TransactionSummaryModel } from '../models/transaction-summary.model';

@Injectable({
    providedIn: "root"
})
export class TransactionService {
    public apiBaseUrl: string;

    constructor(private appConfig: AppConfigService, private http: HttpClient) {
        appConfig.$configurationLoaded.subscribe(value => {
            if (value === true) {
                this.apiBaseUrl = this.appConfig.getConfiguration().apiBaseUrl + "/api/v1/transactions/"
            }
        });
    }

    /**
     * @param colored (optional)
     * @return Success
     */
    transactions(): Observable<TransactionModel[]> {
        return of([]);
    }

    /**
     * @param colored (optional)
     * @return Success
     */
    transaction(txId: string, colored: boolean | null | undefined, loadSmartContractIfExists: boolean | null | undefined): Observable<Transaction> {
        let url_ = this.apiBaseUrl + "{txId}?";

        if (txId === undefined || txId === null)
            throw new Error("The parameter 'txId' must be defined.");
        url_ = url_.replace("{txId}", encodeURIComponent("" + txId));
        if (colored !== undefined)
            url_ += "colored=" + encodeURIComponent("" + colored) + "&";
        if (loadSmartContractIfExists !== undefined)
            url_ += "loadSmartContractIfExists=" + encodeURIComponent("" + loadSmartContractIfExists) + "&";
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
                mergeMap((response_: any) => this.processTransaction(response_)),
                catchError((response_: any) => {
                    if (response_ instanceof HttpResponseBase) {
                        try {
                            return this.processTransaction(<any>response_);
                        } catch (e) {
                            return <Observable<Transaction>><any>throwError(e);
                        }
                    } else
                        return <Observable<Transaction>><any>throwError(response_);
                })
            );
    }

    getSmartContractTransactions(loadDetails: boolean = false, take: number = 10): Observable<TransactionSummaryModel[]> {
        let url_ = `${this.appConfig.getConfiguration().apiBaseUrl}/api/v1/smart-contracts?loadDetails=${loadDetails}&take=${take}`;
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
                mergeMap((response_: any) => this.processTransactions(response_)),
                catchError((response_: any) => {
                    if (response_ instanceof HttpResponseBase) {
                        try {
                            return this.processTransactions(<any>response_);
                        } catch (e) {
                            return <Observable<TransactionSummaryModel[]>><any>throwError(e);
                        }
                    } else
                        return <Observable<TransactionSummaryModel[]>><any>throwError(response_);
                })
            );
    }

    protected processTransaction(response: HttpResponseBase): Observable<Transaction> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
                (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); } };
        if (status === 200) {
            return utils.blobToText(responseBlob)
                .pipe(
                    mergeMap(_responseText => {
                        let result200: Transaction = null;
                        const resultData200 = _responseText === "" ? null : JSON.parse(_responseText);
                        result200 = resultData200 ? Transaction.fromTransactionSummaryModel(resultData200) : new Transaction();
                        return of(result200);
                    })
                );
        } else if (status !== 200 && status !== 204) {
            return utils.blobToText(responseBlob)
                .pipe(
                    mergeMap(_responseText => utils.throwException("An unexpected server error occurred.", status, _responseText, _headers))
                );
        }
        return of<Transaction>(<any>null);
    }

    protected processTransactions(response: HttpResponseBase): Observable<TransactionSummaryModel[]> {
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

                        result200 = resultData200 ? resultData200.map(r => <TransactionSummaryModel>r) : [];
                        return of(result200);
                    })
                );
        } else if (status !== 200 && status !== 204) {
            return utils.blobToText(responseBlob)
                .pipe(
                    mergeMap(_responseText => utils.throwException("An unexpected server error occurred.", status, _responseText, _headers))
                );
        }
        return of<TransactionSummaryModel[]>(<any>null);
    }
}
