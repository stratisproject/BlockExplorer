import { async, TestBed } from '@angular/core/testing';
import { UiLayoutModule } from './ui-layout.module';

describe('UiLayoutModule', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [UiLayoutModule]
    }).compileComponents();
  }));

  it('should create', () => {
    expect(UiLayoutModule).toBeDefined();
  });
});
