import { Injectable } from '@angular/core';
import { ApiServiceBase } from '@core/services';
import { Transaction, ITransactionSummaryModel } from '../models';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
    providedIn: "root"
})
export class TransactionService extends ApiServiceBase {
    /**
     * @param txId (transaction hash)
     * @return Success
     */
    transaction(txId: string, colored: boolean = false, loadSmartContractIfExists: boolean = false): Observable<Transaction> {
        if (txId === undefined || txId === null)
            throw new Error("The parameter 'txId' must be defined.");

        return this.get<ITransactionSummaryModel>(`transactions/${txId}`, {
            "colored": colored.toString(),
            "loadSmartContractIfExists": loadSmartContractIfExists.toString()
        }).pipe(
            map(data => Transaction.fromTransactionSummaryModel(data))
        );
    }
}
