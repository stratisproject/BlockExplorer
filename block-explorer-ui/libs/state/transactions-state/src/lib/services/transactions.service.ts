import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { Observable, throwError as _observableThrow, of as _observableOf, of } from 'rxjs';
import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';
import { blobToText, throwException, TransactionModel, API_BASE_URL, TransactionSummaryModel } from '@blockexplorer/shared/models';

@Injectable()
export class TransactionsService {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : "";
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
    transaction(txId: string, colored: boolean | null | undefined, loadSmartContractIfExists: boolean | null | undefined): Observable<TransactionSummaryModel> {
        let url_ = this.baseUrl + "/api/v1/transactions/{txId}?";
        if (txId === undefined || txId === null)
            throw new Error("The parameter 'txId' must be defined.");
        url_ = url_.replace("{txId}", encodeURIComponent("" + txId));
        if (colored !== undefined)
            url_ += "colored=" + encodeURIComponent("" + colored) + "&";
        if (loadSmartContractIfExists !== undefined)
            url_ += "loadSmartContractIfExists=" + encodeURIComponent("" + loadSmartContractIfExists) + "&";
        url_ = url_.replace(/[?&]$/, "");

        const options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processTransaction(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processTransaction(<any>response_);
                } catch (e) {
                    return <Observable<TransactionSummaryModel>><any>_observableThrow(e);
                }
            } else
                return <Observable<TransactionSummaryModel>><any>_observableThrow(response_);
        }));
    }

    protected processTransaction(response: HttpResponseBase): Observable<TransactionSummaryModel> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            const resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 ? TransactionSummaryModel.fromJS(resultData200) : new TransactionSummaryModel();
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<TransactionSummaryModel>(<any>null);
    }
}
