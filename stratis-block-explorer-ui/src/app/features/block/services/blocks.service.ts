import { mergeMap, catchError } from 'rxjs/operators';
import { Observable, throwError, of } from 'rxjs';
import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';
import { AppConfigService } from '@core/services/app-config.service';
import *  as utils from '@shared/utils';
import { BlockResponseModel } from '../models/block-response.model';
import { BlockHeaderResponseModel } from '../models/block-header-response.model';
import { StatsModel } from '../models/stats.model';

@Injectable({
    providedIn: "root"
})
export class BlocksService {
    public apiBaseUrl: string;

    constructor(private appConfig: AppConfigService, private http: HttpClient) {
        appConfig.$configurationLoaded.subscribe(value => {
            if (value === true) {
                this.apiBaseUrl = this.appConfig.getConfiguration().apiBaseUrl + "/api/v1/blocks/"
            }
        });
    }

    /**
     * @param headerOnly (optional)
     * @param extended (optional)
     * @return Success
     */
    block(block: string, headerOnly: boolean | null | undefined, extended: boolean | null | undefined): Observable<BlockResponseModel> {
        if (block === undefined || block === null)
            throw new Error("The parameter 'block' must be defined.");

        let url_ = this.apiBaseUrl + "{block}?";
        url_ = url_.replace("{block}", encodeURIComponent("" + block));

        if (headerOnly !== undefined)
            url_ += "headerOnly=" + encodeURIComponent("" + headerOnly) + "&";

        if (extended !== undefined)
            url_ += "extended=" + encodeURIComponent("" + extended) + "&";

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
                mergeMap((response_: any) => this.processBlock(response_)),
                catchError((response_: any) => {
                    if (response_ instanceof HttpResponseBase) {
                        try {
                            return this.processBlock(<any>response_);
                        } catch (e) {
                            return <Observable<BlockResponseModel>><any>throwError(e);
                        }
                    } else
                        return <Observable<BlockResponseModel>><any>throwError(response_);
                }));
    }

    protected processBlock(response: HttpResponseBase): Observable<BlockResponseModel> {
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
                        result200 = resultData200 ? <BlockResponseModel>(resultData200) : new BlockResponseModel();
                        return of(result200);
                    }));
        }
        else if (status !== 200 && status !== 204) {
            return utils.blobToText(responseBlob)
                .pipe(
                    mergeMap(_responseText => utils.throwException("An unexpected server error occurred.", status, _responseText, _headers))
                );
        }
        return of<BlockResponseModel>(<any>null);
    }

    /**
     * @return Success
     */
    blockHeader(block: string): Observable<BlockHeaderResponseModel> {
        if (block === undefined || block === null)
            throw new Error("The parameter 'block' must be defined.");

        let url_ = this.apiBaseUrl + "{block}/header";
        url_ = url_.replace("{block}", encodeURIComponent("" + block));
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
                mergeMap((response_: any) => this.processBlockHeader(response_)),
                catchError((response_: any) => {
                    if (response_ instanceof HttpResponseBase) {
                        try {
                            return this.processBlockHeader(<any>response_);
                        } catch (e) {
                            return <Observable<BlockHeaderResponseModel>><any>throwError(e);
                        }
                    } else
                        return <Observable<BlockHeaderResponseModel>><any>throwError(response_);
                }));
    }

    protected processBlockHeader(response: HttpResponseBase): Observable<BlockHeaderResponseModel> {
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
                        result200 = resultData200 ? <BlockHeaderResponseModel>resultData200 : new BlockHeaderResponseModel();
                        return of(result200);
                    })
                );
        }
        else if (status !== 200 && status !== 204) {
            return utils.blobToText(responseBlob)
                .pipe(
                    mergeMap(_responseText => utils.throwException("An unexpected server error occurred.", status, _responseText, _headers))
                );
        }
        return of<BlockHeaderResponseModel>(<any>null);
    }

    /**
     * @param headerOnly (optional)
     * @param extended (optional)
     * @return Success
     */
    blocks(records: number): Observable<BlockResponseModel[]> {
        let url_ = this.apiBaseUrl + `top?start=0&top=${records}`;
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
                mergeMap((response_: any) => this.processBlocks(response_)),
                catchError((response_: any) => {
                    if (response_ instanceof HttpResponseBase) {
                        try {
                            return this.processBlocks(<any>response_);
                        } catch (e) {
                            return <Observable<BlockResponseModel[]>><any>throwError(e);
                        }
                    } else
                        return <Observable<BlockResponseModel[]>><any>throwError(response_);
                })
            );
    }

    protected processBlocks(response: HttpResponseBase): Observable<BlockResponseModel[]> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
                (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        const _headers: any = {}; if (response.headers) { for (const key of response.headers.keys()) { _headers[key] = response.headers.get(key); } };
        if (status === 200) {
            return utils.blobToText(responseBlob).pipe(mergeMap(_responseText => {
                let result200: any = null;
                const resultData200 = _responseText === "" ? null : JSON.parse(_responseText);
                result200 = resultData200 ? resultData200.map(r => <BlockResponseModel>r) : [];
                return of(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return utils.blobToText(responseBlob).pipe(mergeMap(_responseText => {
                return utils.throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return of<BlockResponseModel[]>(<any>null);
    }

    /**
     * @return Success
     */
    stats(): Observable<StatsModel> {
        let url_ = this.apiBaseUrl + "last24";

        const options_: any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.get(url_, options_)
            .pipe(
                mergeMap((response_: any) => this.processStats(response_)),
                catchError((response_: any) => {
                    if (response_ instanceof HttpResponseBase) {
                        try {
                            return this.processStats(<any>response_);
                        } catch (e) {
                            return <Observable<StatsModel>><any>throwError(e);
                        }
                    } else
                        return <Observable<StatsModel>><any>throwError(response_);
                })
            );
    }

    protected processStats(response: HttpResponseBase): Observable<StatsModel> {
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
                        result200 = resultData200 ? <StatsModel>resultData200 : new StatsModel();
                        return of(result200);
                    }));
        }
        else if (status !== 200 && status !== 204) {
            return utils.blobToText(responseBlob)
                .pipe(
                    mergeMap(_responseText => utils.throwException("An unexpected server error occurred.", status, _responseText, _headers))
                );
        }
        return of<StatsModel>(<any>null);
    }
}
