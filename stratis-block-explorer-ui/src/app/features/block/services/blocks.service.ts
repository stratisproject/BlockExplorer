import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BlockResponseModel } from '../models/block-response.model';
import { BlockHeaderResponseModel } from '../models/block-header-response.model';
import { StatsModel } from '../models/stats.model';
import { ApiServiceBase } from '@core/services';

@Injectable({
   providedIn: "root"
})
export class BlocksService extends ApiServiceBase {

   /**
    * @param block block hash or height to fetch
    * @param headerOnly specify if only header should be returned
    * @param extended specify if extended information has to be returned
    */
   block(block: string, headerOnly: boolean = false, extended: boolean = false) {
      if (block === undefined || block === null)
         throw new Error("The parameter 'block' must be defined.");

      return this.get<BlockResponseModel>(`blocks/${block}`, {
         "headerOnly": headerOnly.toString(),
         "extended": extended.toString()
      });
   }

   /**
    * @param block block hash or height to fetch
    * @return Success
    */
   blockHeader(block: string): Observable<BlockHeaderResponseModel> {
      if (block === undefined || block === null)
         throw new Error("The parameter 'block' must be defined.");

      return this.get<BlockHeaderResponseModel>(`blocks/${block}/header`);
   }

   /**
    * @param records number of records to return
    * @return Success
    */
   blocks(records: number = 25): Observable<BlockResponseModel[]> {
      return this.get<BlockResponseModel[]>(`blocks/top`, {
         "start": "0",
         "top": records.toString()
      });
   }

   /**
    * @return Success
    */
   stats(): Observable<StatsModel> {
      return this.get<StatsModel>("blocks/last24stats");
   }
}
