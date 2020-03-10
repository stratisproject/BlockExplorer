import { async, TestBed } from '@angular/core/testing';
import { StateTokensStateModule } from './state-tokens-state.module';

describe('StateTokensStateModule', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [StateTokensStateModule]
    }).compileComponents();
  }));

  it('should create', () => {
    expect(StateTokensStateModule).toBeDefined();
  });
});
