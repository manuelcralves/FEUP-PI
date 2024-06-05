import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelecionarUnidadeComponent } from './selecionar-unidade.component';

describe('SelecionarUnidadeComponent', () => {
  let component: SelecionarUnidadeComponent;
  let fixture: ComponentFixture<SelecionarUnidadeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelecionarUnidadeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SelecionarUnidadeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
