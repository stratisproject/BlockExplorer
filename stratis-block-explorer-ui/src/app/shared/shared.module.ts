import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as components from './components';
import { MatIconModule } from '@angular/material/icon';
import { CustomMaterialModuleModule } from './custom-material-module.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ClipboardModule } from 'ngx-clipboard';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { HighlightModule, HIGHLIGHT_OPTIONS } from 'ngx-highlightjs';

@NgModule({
   declarations: [
      components.BalanceComponent,
      components.BusyIndicatorComponent,
      components.HashViewComponent,
      components.AddressViewComponent,
      components.MatAnimatedIconComponent,
      components.FaAnimatedIconComponent,
      components.CopyToClipboardButtonComponent,
      components.ChipComponent
   ],
   imports: [
      MatIconModule,
      CustomMaterialModuleModule,
      CommonModule,
      RouterModule,
      FlexLayoutModule,
      ClipboardModule,
      FontAwesomeModule,
      HighlightModule,
   ],
   exports: [
      components.BalanceComponent,
      components.BusyIndicatorComponent,
      components.HashViewComponent,
      components.AddressViewComponent,
      components.MatAnimatedIconComponent,
      components.FaAnimatedIconComponent,
      components.CopyToClipboardButtonComponent,
      components.ChipComponent,
      CustomMaterialModuleModule,
      FlexLayoutModule,
      ClipboardModule,
      FontAwesomeModule,
      HighlightModule
   ],
   providers: [
      { provide: HIGHLIGHT_OPTIONS, useValue: { languages: getHighlightLanguages() } }
   ]
})
export class SharedModule { }

export { takeUntilDestroyed } from './rxjs/operators/take-until-destroyed';

/**
 * Import specific languages to avoid importing everything
 * The following will lazy load highlight.js core script (~10KB)
 */
export function getHighlightLanguages() {
   return {
      csharp: () => import('highlight.js/lib/languages/cs'),
      json: () => import('highlight.js/lib/languages/json')
   };
}
