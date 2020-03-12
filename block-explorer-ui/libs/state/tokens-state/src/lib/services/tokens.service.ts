/* tslint:disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpResponse, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base-service';
import { ApiConfiguration } from './api-configuration';
import { StrictHttpResponse } from './strict-http-response';
import { Observable } from 'rxjs';
import { map as __map, filter as __filter } from 'rxjs/operators';
import { TokenTransactionResponse } from './token-transaction-response';
import { TokenDetail } from './token-detail';

@Injectable()
class TokensService extends BaseService {
  constructor(
    http: HttpClient
  ) {
    super(http);
  }

  TokenDetailResponse(params: TokensService.TokenDetailParams): Observable<StrictHttpResponse<TokenDetail> {
    let __params = this.newParams();
    let __headers = new HttpHeaders();
    let __body: any = null;

    if (params.from != null) __params = __params.set('from', params.from.toString());
    console.log(this.rootUrl);
    let req = new HttpRequest<any>(
      'GET',
      this.rootUrl + `/api/v1/tokens/${params.tokenAddress}`,
      __body,
      {
        headers: __headers,
        params: __params,
        responseType: 'json'
      });

    return this.http.request<any>(req).pipe(
      __filter(_r => _r instanceof HttpResponse),
      __map((_r) => {
        return _r as StrictHttpResponse<TokenDetail>;
      })
    );
  }

  TokenDetail(params: TokensService.TransactionsForTokenParams): Observable<TokenDetail> {
    return this.TokenDetailResponse(params).pipe(
      __map(_r => _r.body as TokenDetail)
    );
  }

  /**
   * @param params The `TokensService.TransactionsForTokenParams` containing the following parameters:
   *
   * - `tokenAddress`:
   *
   * - `from`:
   *
   * @return Success
   */
  TransactionsForTokenResponse(params: TokensService.TransactionsForTokenParams): Observable<StrictHttpResponse<Array<TokenTransactionResponse>>> {
    let __params = this.newParams();
    let __headers = new HttpHeaders();
    let __body: any = null;

    if (params.from != null) __params = __params.set('from', params.from.toString());
    let req = new HttpRequest<any>(
      'GET',
      this.rootUrl + `/api/v1/tokens/${params.tokenAddress}`,
      __body,
      {
        headers: __headers,
        params: __params,
        responseType: 'json'
      });

    return this.http.request<any>(req).pipe(
      __filter(_r => _r instanceof HttpResponse),
      __map((_r) => {
        return _r as StrictHttpResponse<Array<TokenTransactionResponse>>;
      })
    );
  }
  /**
   * @param params The `TokensService.TransactionsForTokenParams` containing the following parameters:
   *
   * - `tokenAddress`:
   *
   * - `from`:
   *
   * @return Success
   */
  TransactionsForToken(params: TokensService.TransactionsForTokenParams): Observable<Array<TokenTransactionResponse>> {
    return this.TransactionsForTokenResponse(params).pipe(
      __map(_r => _r.body as Array<TokenTransactionResponse>)
    );
  }

  /**
   * @param tokenAddress undefined
   * @return Success
   */
  RecentTransactionsForTokenResponse(tokenAddress: string): Observable<StrictHttpResponse<Array<TokenTransactionResponse>>> {
    let __params = this.newParams();
    let __headers = new HttpHeaders();
    let __body: any = null;

    let req = new HttpRequest<any>(
      'GET',
      this.rootUrl + `/api/v1/tokens/${tokenAddress}/recent`,
      __body,
      {
        headers: __headers,
        params: __params,
        responseType: 'json'
      });

    return this.http.request<any>(req).pipe(
      __filter(_r => _r instanceof HttpResponse),
      __map((_r) => {
        return _r as StrictHttpResponse<Array<TokenTransactionResponse>>;
      })
    );
  }
  /**
   * @param tokenAddress undefined
   * @return Success
   */
  RecentTransactionsForToken(tokenAddress: string): Observable<Array<TokenTransactionResponse>> {
    return this.RecentTransactionsForTokenResponse(tokenAddress).pipe(
      __map(_r => _r.body as Array<TokenTransactionResponse>)
    );
  }
}

module TokensService {

  /**
   * Parameters for TransactionsForToken
   */
  export interface TransactionsForTokenParams {
    tokenAddress: string;
    from?: number;
  }

  export interface TokenDetailParams {
    tokenAddress: string;
  }
}

export { TokensService }
