import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BlockSummaryComponent } from './block-summary.component';

describe('BlockSummaryComponent', () => {
  let component: BlockSummaryComponent;
  let fixture: ComponentFixture<BlockSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BlockSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BlockSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
