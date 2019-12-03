import { Component, Input, OnInit } from '@angular/core';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';

@Component({
    selector: 'fa-animated-icon',
    templateUrl: './fa-animated-icon.component.html',
    styleUrls: ['./fa-animated-icon.component.scss']
})
export class FaAnimatedIconComponent implements OnInit {

    @Input() startIcon: IconDefinition;
    @Input() endIcon: IconDefinition;
    @Input() startClass: string;
    @Input() endClass: string;
    @Input() animate: boolean;
    @Input() animateFromParent?: boolean = false;

    @Input() startColor: string;
    @Input() endColor: string;

    constructor() { }

    ngOnInit() {
    }

    toggle() {
        if (!this.animateFromParent) this.animate = !this.animate;
    }

    get getStyle() {
        if (this.animate)
            return [`animate mat-icon mat-${this.endColor} ${this.endClass} `];
        else
            return [`mat-icon mat-${this.startColor} ${this.startClass}`];
    }
}
