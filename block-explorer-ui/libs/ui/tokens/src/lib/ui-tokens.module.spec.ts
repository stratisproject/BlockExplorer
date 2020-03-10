import { async, TestBed } from '@angular/core/testing';
import { UiTokensModule } from './ui-tokens.module';

describe('UiTokensModule', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [UiTokensModule]
    }).compileComponents();
  }));

  it('should create', () => {
    expect(UiTokensModule).toBeDefined();
  });
});
