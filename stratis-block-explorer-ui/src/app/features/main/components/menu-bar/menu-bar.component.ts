import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AppConfigService } from '@core/services/app-config.service';
import { Log } from '@shared/logger.service';
import { MatSelectChange } from '@angular/material/select';
import { MatAnimatedIconComponent } from '../../../../shared/components';

@Component({
    selector: 'app-menu-bar',
    templateUrl: './menu-bar.component.html',
    styleUrls: ['./menu-bar.component.scss']
})
export class MenuBarComponent implements OnInit {
    title = 'Stratis Explorer';

    @Input() public searchText = '';
    @Input() public links: { title: string, url: string };

    @Output() public find = new EventEmitter<string>();
    @Output() public sidenavToggle = new EventEmitter();

    constructor(private appConfig: AppConfigService, private log: Log) {
        this.title = `${appConfig.getConfiguration().networkName} Explorer`;
    }

    ngOnInit(): void {
    }

    enterPressed() {
        this.log.info("search for", this.searchText);
        this.find.emit(this.searchText);
    }

    public onToggleSidenav = () => {
        this.sidenavToggle.emit();
    }

    public navigateTo($event: MatSelectChange) {
        window.location.href = $event.value;
    }

    public toggleTheme(animatedIcon: MatAnimatedIconComponent) {
        animatedIcon.animate = !animatedIcon.animate;

        // if animate, shown icon is "end" (dark theme), otherwise it's "start" (light theme)
        if (animatedIcon.animate) {
            document.body.classList.remove("dark-theme"); //in case it was already there previously
            document.body.classList.add("dark-theme");
        }
        else {
            document.body.classList.remove("dark-theme");
        }
    }
}
