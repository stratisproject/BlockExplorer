import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AppConfigService } from '@core/services/app-config.service';


@Component({
    selector: 'app-balance',
    templateUrl: './balance.component.html',
    styleUrls: ['./balance.component.scss']
})
export class BalanceComponent implements OnChanges {
    @Input() balance = 0;
    @Input() decimalPlaces = 8;

    symbol: string;
    value: number;

    wholePart: string;
    decimalPart: string;

    constructor(private appConfig: AppConfigService) {
        this.symbol = this.appConfig.getConfiguration().symbol;
    }

    ngOnChanges(changes: SimpleChanges): void {
        this.value = this.balance / Math.pow(10, 8);
        let whole = Math.floor(this.value);

        //TODO: this part should be localized
        this.wholePart = whole.toLocaleString("en-us");
        this.decimalPart = `.${(this.value - whole).toFixed(this.decimalPlaces).split('.')[1]}`;
    }
}
