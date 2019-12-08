import { HttpClient, HttpParams } from '@angular/common/http';
import { LogService } from './logger.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

export class ApiServiceBase {
    constructor(protected http: HttpClient, protected logger: LogService) {
    }


    protected handleError<T>(operation = 'operation', result?: T) {
        return (error, any): Observable<T> => {
            this.logger.error(`${operation} failed: ${error.message}`);
            return of(result as T);
        };
    }

    protected get<T>(resourcePath: string, paramsObject: { [param: string]: string | string[] } = null): Observable<T> {
        let params: HttpParams = null;

        if (paramsObject != null) {
            params = new HttpParams({
                fromObject: paramsObject
            })
        }
        return this.http.get<T>(resourcePath, { params: params }).pipe(
            catchError(this.handleError<T>(resourcePath))
        );
    }
}
