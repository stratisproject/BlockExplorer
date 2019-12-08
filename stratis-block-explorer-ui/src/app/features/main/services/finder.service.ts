import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ApiServiceBase } from '@core/services';

@Injectable({
    providedIn: "root"
})
export class FinderService extends ApiServiceBase {
    /**
     * @return Success
     */
    whatIsIt(identifier: string): Observable<any> {
        if (identifier === undefined || identifier === null)
            throw new Error("The parameter 'identifier' must be defined.");

        return this.get<any>(`finder/${identifier}`);
    }
}
