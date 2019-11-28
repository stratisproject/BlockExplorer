import { Component, Input, OnInit } from '@angular/core';

@Component({
    selector: 'mat-animated-icon',
    templateUrl: './mat-animated-icon.component.html',
    styleUrls: ['./mat-animated-icon.component.scss']
})
export class MatAnimatedIconComponent implements OnInit {

    @Input() start: String;
    @Input() end: String;
    @Input() colorStart: String;
    @Input() colorEnd: String;
    @Input() animate: boolean;
    @Input() animateFromParent?: boolean = false;

    constructor() { }

    ngOnInit() {
    }

    toggle() {
        if (!this.animateFromParent) this.animate = !this.animate;
    }
}
