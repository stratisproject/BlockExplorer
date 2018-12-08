import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { Observable, throwError as _observableThrow, of as _observableOf } from 'rxjs';
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';
import { BalanceResponseModel, BalanceSummaryModel, blobToText, throwException, API_BASE_URL } from '@blockexplorer/shared/models';

@Injectable()
export class BalancesService {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : "";
    }

    /**
     * @param continuation (optional)
     * @param until (optional)
     * @param from (optional)
     * @param includeImmature (optional)
     * @param unspentOnly (optional)
     * @param colored (optional)
     * @return Success
     */
    addressBalance(balanceId: string, continuation: string | null | undefined, until: string | null | undefined, from: string | null | undefined, includeImmature: boolean | null | undefined, unspentOnly: boolean | null | undefined, colored: boolean | null | undefined): Observable<BalanceResponseModel> {
        let url_ = this.baseUrl + "/api/v1/balances/{balanceId}?";
        if (balanceId === undefined || balanceId === null)
            throw new Error("The parameter 'balanceId' must be defined.");
        url_ = url_.replace("{balanceId}", encodeURIComponent("" + balanceId));
        if (continuation !== undefined)
            url_ += "continuation=" + encodeURIComponent("" + continuation) + "&";
        if (until !== undefined)
            url_ += "until=" + encodeURIComponent("" + until) + "&";
        if (from !== undefined)
            url_ += "from=" + encodeURIComponent("" + from) + "&";
        if (includeImmature !== undefined)
            url_ += "includeImmature=" + encodeURIComponent("" + includeImmature) + "&";
        if (unspentOnly !== undefined)
            url_ += "unspentOnly=" + encodeURIComponent("" + unspentOnly) + "&";
        if (colored !== undefined)
            url_ += "colored=" + encodeURIComponent("" + colored) + "&";
        url_ = url_.replace(/[?&]$/, "");

        const options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processAddressBalance(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processAddressBalance(<any>response_);
                } catch (e) {
                    return <Observable<BalanceResponseModel>><any>_observableThrow(e);
                }
            } else
                return <Observable<BalanceResponseModel>><any>_observableThrow(response_);
        }));
    }

    protected processAddressBalance(response: HttpResponseBase): Observable<BalanceResponseModel> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            const resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 ? BalanceResponseModel.fromJS(resultData200) : new BalanceResponseModel();
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<BalanceResponseModel>(<any>null);
    }

    /**
     * @param at (optional)
     * @param debug (optional)
     * @param colored (optional)
     * @return Success
     */
    addressBalanceSummary(balanceId: string, at: string | null | undefined, debug: boolean | null | undefined, colored: boolean | null | undefined): Observable<BalanceSummaryModel> {
        let url_ = this.baseUrl + "/api/v1/balances/{balanceId}/summary?";
        if (balanceId === undefined || balanceId === null)
            throw new Error("The parameter 'balanceId' must be defined.");
        url_ = url_.replace("{balanceId}", encodeURIComponent("" + balanceId));
        if (at !== undefined)
            url_ += "at=" + encodeURIComponent("" + at) + "&";
        if (debug !== undefined)
            url_ += "debug=" + encodeURIComponent("" + debug) + "&";
        if (colored !== undefined)
            url_ += "colored=" + encodeURIComponent("" + colored) + "&";
        url_ = url_.replace(/[?&]$/, "");

        const options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processAddressBalanceSummary(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processAddressBalanceSummary(<any>response_);
                } catch (e) {
                    return <Observable<BalanceSummaryModel>><any>_observableThrow(e);
                }
            } else
                return <Observable<BalanceSummaryModel>><any>_observableThrow(response_);
        }));
    }

    protected processAddressBalanceSummary(response: HttpResponseBase): Observable<BalanceSummaryModel> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

            const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            const resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 ? BalanceSummaryModel.fromJS(resultData200) : new BalanceSummaryModel();
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<BalanceSummaryModel>(<any>null);
    }
}
