import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppConfigService } from '@core/services/app-config.service';
import { Log } from '@shared/logger.service';
import { MatSelectChange } from '@angular/material/select';
import { MatAnimatedIconComponent } from '@shared/components';

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

    selectedNetwork: string;

    @ViewChild('themeButton', { static: true }) animatedIcon: MatAnimatedIconComponent;

    constructor(private appConfig: AppConfigService, private log: Log) {
        this.title = `${appConfig.getConfiguration().networkName} Explorer`;
    }

    ngOnInit(): void {
        this.loadTheme();
        this.selectedNetwork = this.appConfig.getConfiguration().url;
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

    public toggleTheme() {
        this.animatedIcon.animate = !this.animatedIcon.animate;

        // if animate, shown icon is "end" (dark theme), otherwise it's "start" (light theme)
        if (this.animatedIcon.animate) {
            this.setDarkTheme();
        }
        else {
            this.setLightTheme();
        }
    }

    private loadTheme() {
        let darkThemeEnabled = localStorage.getItem("dark-theme");
        if (darkThemeEnabled === "true") {
            document.body.classList.add("dark-theme");
            this.animatedIcon.animate = true;
        } else {
            this.animatedIcon.animate = false;
        }
    }

    private setDarkTheme() {
        localStorage.setItem("dark-theme", "true");
        document.body.classList.remove("dark-theme"); //in case it was already there previously
        document.body.classList.add("dark-theme");
    }

    private setLightTheme() {
        localStorage.setItem("dark-theme", "false");
        document.body.classList.remove("dark-theme");
    }
}
