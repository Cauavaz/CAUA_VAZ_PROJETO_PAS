import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Component } from '@angular/core';
import { App } from './app';

// Componentes mock para as rotas
@Component({ template: '' })
class MockLoginComponent {}

@Component({ template: '' })
class MockDashboardComponent {}

describe('App', () => {
  let fixture: any;
  let component: App;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideRouter([
          { path: 'login', component: MockLoginComponent },
          { path: 'dashboard', component: MockDashboardComponent },
          { path: '', redirectTo: '/dashboard', pathMatch: 'full' }
        ])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(App);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve ser criado', () => {
    expect(component).toBeTruthy();
  });

  it('deve ter o serviço de tema injetado', () => {
    expect(component.themeService).toBeTruthy();
  });

  it('deve ter o menu de usuário fechado por padrão', () => {
    expect(component.userMenuOpen()).toBe(false);
  });

  it('deve alternar o menu de usuário', () => {
    component.toggleUserMenu();
    expect(component.userMenuOpen()).toBe(true);

    component.toggleUserMenu();
    expect(component.userMenuOpen()).toBe(false);
  });

  it('deve fechar o menu de usuário', () => {
    component.userMenuOpen.set(true);
    component.closeUserMenu();
    expect(component.userMenuOpen()).toBe(false);
  });

  it('deve chamar toggleTheme do ThemeService', () => {
    expect(() => component.toggleTheme()).not.toThrow();
  });

  it('deve fazer logout', () => {
    expect(() => component.logout()).not.toThrow();
  });

  it('deve identificar rotas de autenticação', () => {
    expect(component['isAuthRoute']('/login')).toBe(true);
    expect(component['isAuthRoute']('/register')).toBe(true);
    expect(component['isAuthRoute']('/dashboard')).toBe(false);
    expect(component['isAuthRoute']('/titulos')).toBe(false);
  });

  it('deve mostrar cabeçalho em rotas não autenticadas', () => {
    expect(component.showHeader()).toBe(true);
  });

  it('deve ter o usuário atual disponível', () => {
    expect(component.currentUser).toBeDefined();
  });
});
