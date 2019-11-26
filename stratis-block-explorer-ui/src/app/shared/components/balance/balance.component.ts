import { Component, OnInit, Input } from '@angular/core';
import { AppConfigService } from '@core/services/app-config.service';


@Component({
    selector: 'app-balance',
    templateUrl: './balance.component.html',
    styleUrls: ['./balance.component.scss']
})
export class BalanceComponent implements OnInit {

    @Input() balance = 0;
    @Input() decimalPlaces = 8;

    constructor(private appConfig: AppConfigService) { }

    ngOnInit() {
    }

    get value() {
        return this.balance / Math.pow(10, 8);
    }

    get symbol() {
        return this.appConfig.getConfiguration().symbol;
    }

    get whole() {
        return this.value.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,').split('.')[0];
    }

    get decimal() {
        if (this.value % 1 === 0) return `.${'0'.repeat(this.decimalPlaces)}`;
        const fraction = parseInt(((this.value % 1) * Math.pow(10, this.decimalPlaces)).toString(), 10);
        return `.${fraction}`;
    }

}
