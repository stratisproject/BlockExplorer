import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Route } from '@angular/router';
import { TransactionsPageComponent } from './containers/transactions-page/transactions-page.component';
import { TransactionsListComponent } from './components/transactions-list/transactions-list.component';

export { TransactionsPageComponent } from './containers/transactions-page/transactions-page.component';

export const uiTransactionsRoutes: Route[] = [
  { path: '', component: TransactionsPageComponent }
];

@NgModule({
  imports: [CommonModule, RouterModule],
  declarations: [TransactionsPageComponent, TransactionsListComponent],
  exports: [TransactionsPageComponent]
})
export class UiTransactionsModule {}
