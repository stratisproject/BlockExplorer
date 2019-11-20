import { Component } from '@angular/core';
import { AppConfigService } from './core/services/app-config.service';
import { StyleManagerService } from './core/services/style-manager.service';

@Component({
   selector: 'app-root',
   templateUrl: './app.component.html',
   styleUrls: ['./app.component.scss']
})
export class AppComponent {
   constructor(appConfig: AppConfigService, styleManager: StyleManagerService) {
      const configuration = appConfig.getConfiguration();

      styleManager.removeStyle('theme');
      styleManager.setStyle('theme', `assets/styles/${configuration.themeName}.css`);
   }
}
