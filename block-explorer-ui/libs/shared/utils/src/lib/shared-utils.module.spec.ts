import { async, TestBed } from '@angular/core/testing';
import { SharedUtilsModule } from './shared-utils.module';

describe('SharedUtilsModule', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [SharedUtilsModule]
    }).compileComponents();
  }));

  it('should create', () => {
    expect(SharedUtilsModule).toBeDefined();
  });
});
