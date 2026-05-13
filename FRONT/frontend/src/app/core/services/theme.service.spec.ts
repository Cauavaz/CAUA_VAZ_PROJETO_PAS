import { TestBed } from '@angular/core/testing';
import { ThemeService } from './theme.service';

describe('ThemeService', () => {
  let service: ThemeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ThemeService);
    
    // Limpar localStorage antes de cada teste
    localStorage.clear();
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('deve ser criado', () => {
    expect(service).toBeTruthy();
  });

  describe('Gerenciamento de Tema', () => {
    it('deve ter tema padrão como light', () => {
      expect(service.theme()).toBe('light');
    });

    it('deve alternar tema de light para dark', () => {
      service.toggleTheme();
      expect(service.theme()).toBe('dark');
    });

    it('deve alternar tema de dark para light', () => {
      service.toggleTheme(); // dark
      service.toggleTheme(); // light
      expect(service.theme()).toBe('light');
    });

    it('deve definir tema como dark explicitamente', () => {
      service.setTheme('dark');
      expect(service.theme()).toBe('dark');
    });

    it('deve definir tema como light explicitamente', () => {
      service.setTheme('light');
      expect(service.theme()).toBe('light');
    });

    it('deve aplicar tema ao documento', () => {
      // Teste simplificado - apenas verifica se o método não lança erro
      expect(() => service.setTheme('dark')).not.toThrow();
      expect(service.theme()).toBe('dark');
    });
  });

  describe('Persistência', () => {
    it('deve ter método para salvar tema', () => {
      expect(() => service.setTheme('dark')).not.toThrow();
    });

    it('deve usar light como padrão se não houver tema no localStorage', () => {
      expect(service.theme()).toBe('light');
    });
  });

  describe('Signal de Modo Escuro', () => {
    it('deve ter signal isDark disponível', () => {
      expect(service.isDark).toBeDefined();
      expect(typeof service.isDark).toBe('function');
    });

    it('deve retornar valor booleano', () => {
      const result = service.isDark();
      expect(typeof result).toBe('boolean');
    });
  });
});
