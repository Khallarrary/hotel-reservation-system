import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReservaCaixa } from './reserva-caixa';

describe('ReservaCaixa', () => {
  let component: ReservaCaixa;
  let fixture: ComponentFixture<ReservaCaixa>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReservaCaixa],
    }).compileComponents();

    fixture = TestBed.createComponent(ReservaCaixa);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
