import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { LogService } from './logger.service';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { SwaggerException } from '../../shared/models/swagger-exception.model';

export class ApiServiceBase {
    constructor(protected http: HttpClient, protected logger: LogService) {
    }


    protected handleErrors<T>(operation = 'operation', result?: T) {
        return (error, any): Observable<T> => {
            this.logger.error(`${operation} failed: ${error.message}`);
            return of(result as T);
        };
    }

    protected handleError<T>(operation = 'operation', error: HttpErrorResponse) {
        if (error.error instanceof ErrorEvent) {
            // A client-side or network error occurred. Handle it accordingly.
            console.error('An error occurred:', error.error.message);
        } else {
            if (error.error.errors) {
                if (error.error.errors.length > 0) {
                    console.error('swagger exception:', error.error.errors[0]);
                    return throwError(error.error.errors[0].message);
                }
            }
            // The backend returned an unsuccessful response code.
            // The response body may contain clues as to what went wrong,
            console.error(
                `Backend returned code ${error.status}, ` +
                `body was: ${error.error}`);
        }

        // return an observable with a user-facing error message
        return throwError('Something bad happened; please try again later.');
    };

    protected get<T>(resourcePath: string, paramsObject: { [param: string]: string | string[] } = null): Observable<T> {
        let params: HttpParams = null;

        if (paramsObject != null) {
            params = new HttpParams({
                fromObject: paramsObject
            })
        }
        return this.http.get<T>(resourcePath, { params: params }).pipe(
            catchError(err => this.handleError<T>(resourcePath, err))
        );
    }
}
