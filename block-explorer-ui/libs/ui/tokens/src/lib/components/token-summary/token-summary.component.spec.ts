import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TokenSummaryComponent } from './token-summary.component';

describe('TokenSummaryComponent', () => {
  let component: TokenSummaryComponent;
  let fixture: ComponentFixture<TokenSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TokenSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TokenSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
