import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'blockexplorer-smart-contract-opcode',
  templateUrl: './smart-contract-opcode.component.html',
  styleUrls: ['./smart-contract-opcode.component.css']
})
export class SmartContractOpcodeComponent implements OnInit {
  @Input() opCode: string;

  constructor() { }

  ngOnInit() {
  }
}
