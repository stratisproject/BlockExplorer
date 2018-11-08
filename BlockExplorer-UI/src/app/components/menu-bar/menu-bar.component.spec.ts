import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MenuBarComponent } from './menu-bar.component';
import { FormsModule } from '@angular/forms';

describe('MenuBarComponent', () => {
    let component: MenuBarComponent;
    let fixture: ComponentFixture<MenuBarComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            imports: [FormsModule],
            declarations: [MenuBarComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(MenuBarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
