import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmartContractOpcodeComponent } from './smart-contract-opcode.component';

describe('SmartContractOpcodeComponent', () => {
  let component: SmartContractOpcodeComponent;
  let fixture: ComponentFixture<SmartContractOpcodeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmartContractOpcodeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmartContractOpcodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
