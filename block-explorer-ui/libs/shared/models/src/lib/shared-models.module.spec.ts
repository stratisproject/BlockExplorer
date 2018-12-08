import { async, TestBed } from '@angular/core/testing';
import { SharedModelsModule } from './shared-models.module';

describe('SharedModelsModule', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [SharedModelsModule]
    }).compileComponents();
  }));

  it('should create', () => {
    expect(SharedModelsModule).toBeDefined();
  });
});
