import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { Observable, throwError as _observableThrow, of as _observableOf, of } from 'rxjs';
import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';
import { blobToText, throwException, API_BASE_URL, BlockResponseModel, BlockHeaderResponseModel } from '@blockexplorer/shared/models';

@Injectable()
export class BlocksService {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl ? baseUrl : "";
    }

    /**
     * @param headerOnly (optional)
     * @param extended (optional)
     * @return Success
     */
    block(block: string, headerOnly: boolean | null | undefined, extended: boolean | null | undefined): Observable<BlockResponseModel> {
        let url_ = this.baseUrl + "/api/v1/blocks/{block}?";
        if (block === undefined || block === null)
            throw new Error("The parameter 'block' must be defined.");
        url_ = url_.replace("{block}", encodeURIComponent("" + block));
        if (headerOnly !== undefined)
            url_ += "headerOnly=" + encodeURIComponent("" + headerOnly) + "&";
        if (extended !== undefined)
            url_ += "extended=" + encodeURIComponent("" + extended) + "&";
        url_ = url_.replace(/[?&]$/, "");

        const options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processBlock(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processBlock(<any>response_);
                } catch (e) {
                    return <Observable<BlockResponseModel>><any>_observableThrow(e);
                }
            } else
                return <Observable<BlockResponseModel>><any>_observableThrow(response_);
        }));
    }

    protected processBlock(response: HttpResponseBase): Observable<BlockResponseModel> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            const resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 ? BlockResponseModel.fromJS(resultData200) : new BlockResponseModel();
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<BlockResponseModel>(<any>null);
    }

    /**
     * @return Success
     */
    blockHeader(block: string): Observable<BlockHeaderResponseModel> {
        let url_ = this.baseUrl + "/api/v1/blocks/{block}/header";
        if (block === undefined || block === null)
            throw new Error("The parameter 'block' must be defined.");
        url_ = url_.replace("{block}", encodeURIComponent("" + block));
        url_ = url_.replace(/[?&]$/, "");

        const options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processBlockHeader(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processBlockHeader(<any>response_);
                } catch (e) {
                    return <Observable<BlockHeaderResponseModel>><any>_observableThrow(e);
                }
            } else
                return <Observable<BlockHeaderResponseModel>><any>_observableThrow(response_);
        }));
    }

    protected processBlockHeader(response: HttpResponseBase): Observable<BlockHeaderResponseModel> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            const resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 ? BlockHeaderResponseModel.fromJS(resultData200) : new BlockHeaderResponseModel();
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<BlockHeaderResponseModel>(<any>null);
    }

    /**
     * @param headerOnly (optional)
     * @param extended (optional)
     * @return Success
     */
    blocks(): Observable<BlockResponseModel[]> {
        let url_ = this.baseUrl + "/api/v1/blocks/top";
        url_ = url_.replace(/[?&]$/, "");

        const options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processBlocks(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processBlocks(<any>response_);
                } catch (e) {
                    return <Observable<BlockResponseModel[]>><any>_observableThrow(e);
                }
            } else
                return <Observable<BlockResponseModel[]>><any>_observableThrow(response_);
        }));
    }

    protected processBlocks(response: HttpResponseBase): Observable<BlockResponseModel[]> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            const resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);

            result200 = resultData200 ? resultData200.map(r => BlockResponseModel.fromJS(r)) : [];
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<BlockResponseModel[]>(<any>null);
    }
}
