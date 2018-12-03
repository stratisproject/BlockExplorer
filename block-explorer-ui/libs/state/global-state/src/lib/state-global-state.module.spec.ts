import { async, TestBed } from '@angular/core/testing';
import { StateGlobalStateModule } from './state-global-state.module';

describe('StateGlobalStateModule', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [StateGlobalStateModule]
    }).compileComponents();
  }));

  it('should create', () => {
    expect(StateGlobalStateModule).toBeDefined();
  });
});
