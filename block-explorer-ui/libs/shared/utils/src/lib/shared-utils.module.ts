import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Log } from './logger.service';

export * from './logger.service';

@NgModule({
  imports: [CommonModule]
})
export class SharedUtilsModule  {
  static forRoot(): ModuleWithProviders {
      return {
          ngModule: SharedUtilsModule,
          providers: [Log]
      };
  }
}
