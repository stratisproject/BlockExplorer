import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { SmartContractAction, StandardToken } from '../models';
import { ApiServiceBase } from '@core/services/api-service-base';

@Injectable({
    providedIn: "root"
})
export class SmartContractService extends ApiServiceBase {
    /**
     * @param txId hash of the transaction where the smart contract action happened.
     * @return Success
     */
    getSmartContractAction(txId: string): Observable<SmartContractAction> {
        if (txId === undefined || txId === null)
            throw new Error("The parameter 'txId' must be defined.");

        return this.get<SmartContractAction>(`smartcontracts/action/${txId}`);
    }

    getStandardTokens(from: number = 0, take: number = 10): Observable<StandardToken[]> {
        return this.get<StandardToken[]>("smartcontracts", {
            "from": from.toString(),
            "take": take.toString()
        });
    }

    getStandardToken(address: string): Observable<StandardToken> {
        return this.get<StandardToken>("smartcontracts", {
            "address": address
        });
    }
}
