import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BlockSummaryPageComponent } from './block-summary-page.component';

describe('BlockSummaryPageComponent', () => {
  let component: BlockSummaryPageComponent;
  let fixture: ComponentFixture<BlockSummaryPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BlockSummaryPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BlockSummaryPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
