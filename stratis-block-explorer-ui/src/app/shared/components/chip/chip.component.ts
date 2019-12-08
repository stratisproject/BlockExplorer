import { Component, OnInit, Input, AfterViewInit, ViewChild, ViewContainerRef, TemplateRef } from '@angular/core';

@Component({
    selector: 'chip',
    template: `
<ng-template #fixTemplate>
   <!--ugly fix for chips, add an hidden mat-chip-list to be able to add mat-chip without including them in the list-->
   <mat-chip-list style="display:none" id="chip-fix" disabled></mat-chip-list>
</ng-template>
<ng-container #fixContainer></ng-container>
<ng-content></ng-content>
`,
    host: {
        class: 'mat-chip mat-chip-selected mat-standard-chip ',
        '[class.mat-accent]': 'color=="accent"',
        '[class.mat-primary]': 'color=="primary"',
        '[class.mat-warn]': 'color=="warn"'
    }
})
export class ChipComponent implements OnInit {
    @Input() color: string;

    @ViewChild('fixContainer', { read: ViewContainerRef, static: true })
    chipFix: ViewContainerRef;

    @ViewChild('fixTemplate', { read: TemplateRef, static: true })
    fixTemplate: TemplateRef<null>;

    ngOnInit(): void {
        if (!document.getElementById("chip-fix")) {
            this.chipFix.createEmbeddedView(this.fixTemplate);
        }
    }
}
