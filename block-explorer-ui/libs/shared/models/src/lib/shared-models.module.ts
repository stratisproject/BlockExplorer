import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
export * from './generated/api.models';

@NgModule({
  imports: [CommonModule]
})
export class SharedModelsModule {}
