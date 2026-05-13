import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    
    // Limpar localStorage antes de cada teste
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('deve ser criado', () => {
    expect(service).toBeTruthy();
  });

  describe('Estado de Autenticação', () => {
    it('deve começar como não autenticado', () => {
      expect(service.isAuthenticated()).toBe(false);
      expect(service.currentUser()).toBeNull();
    });

    it('deve autenticar usuário com login', () => {
      const mockRequest = { email: 'test@test.com', password: 'password' };
      const mockResponse = {
        token: 'mock-jwt-token',
        name: 'Test User',
        email: 'test@test.com'
      };

      service.login(mockRequest).subscribe(response => {
        expect(response).toEqual(mockResponse);
      });
      debugger
      const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/login`);
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);

      expect(service.isAuthenticated()).toBe(true);
      expect(service.currentUser()?.name).toBe('Test User');
      expect(localStorage.getItem('auth_token')).toBe('mock-jwt-token');
    });

    it('deve autenticar usuário com registro', () => {
      const mockRequest = { 
        name: 'Test User', 
        email: 'test@test.com', 
        password: 'password' 
      };
      const mockResponse = {
        token: 'mock-jwt-token',
        name: 'Test User',
        email: 'test@test.com'
      };

      service.register(mockRequest).subscribe(response => {
        expect(response).toEqual(mockResponse);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/register`);
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);

      expect(service.isAuthenticated()).toBe(true);
      expect(service.currentUser()?.name).toBe('Test User');
    });

    it('deve fazer logout e limpar autenticação', () => {
      // Primeiro fazer login
      const mockRequest = { email: 'test@test.com', password: 'password' };
      const mockResponse = {
        token: 'mock-jwt-token',
        name: 'Test User',
        email: 'test@test.com'
      };

      service.login(mockRequest).subscribe();
      const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/login`);
      req.flush(mockResponse);

      // Depois fazer logout
      service.logout();

      expect(service.isAuthenticated()).toBe(false);
      expect(service.currentUser()).toBeNull();
      expect(localStorage.getItem('auth_token')).toBeNull();
      expect(localStorage.getItem('auth_user')).toBeNull();
    });
  });

  describe('Gerenciamento de Token', () => {
    it('deve retornar token quando autenticado', () => {
      const mockRequest = { email: 'test@test.com', password: 'password' };
      const mockResponse = {
        token: 'mock-jwt-token',
        name: 'Test User',
        email: 'test@test.com'
      };
      
      service.login(mockRequest).subscribe();
      const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/login`);
      req.flush(mockResponse);

      expect(service.getToken()).toBe('mock-jwt-token');
    });

    it('deve retornar null quando não autenticado', () => {
      expect(service.getToken()).toBeNull();
    });
  });

  describe('Inicialização do Storage', () => {
    it('deve carregar estado de autenticação do localStorage na inicialização', () => {
      const mockUser = { id: '1', name: 'Test User', email: 'test@test.com', createdAt: new Date() };
      const mockToken = 'mock-jwt-token';

      localStorage.clear();
      localStorage.setItem('auth_token', mockToken);
      localStorage.setItem('auth_user', JSON.stringify(mockUser));

      // Criar nova instância do serviço via TestBed reset
      TestBed.resetTestingModule();
      TestBed.configureTestingModule({
        imports: [HttpClientTestingModule],
        providers: [AuthService]
      });
      const newService = TestBed.inject(AuthService);
      
      expect(newService.isAuthenticated()).toBe(true);
      expect(newService.currentUser()?.name).toBe('Test User');
      expect(newService.currentUser()?.email).toBe('test@test.com');
      expect(newService.getToken()).toBe(mockToken);
    });

    it('deve lidar com dados de usuário corrompidos no localStorage', () => {
      localStorage.setItem('auth_token', 'valid-token');
      localStorage.setItem('auth_user', 'invalid-json');

      // Criar nova instância do serviço
      const newService = TestBed.inject(AuthService);
      
      expect(newService.isAuthenticated()).toBe(false);
      expect(newService.currentUser()).toBeNull();
    });

    it('deve lidar com token ausente no localStorage', () => {
      localStorage.setItem('auth_user', JSON.stringify({ id: '1', name: 'Test' }));

      // Criar nova instância do serviço
      const newService = TestBed.inject(AuthService);
      
      expect(newService.isAuthenticated()).toBe(false);
      expect(newService.currentUser()).toBeNull();
    });
  });

  describe('Obter Usuário Atual', () => {
    it('deve obter usuário atual da API', () => {
      const mockUser = { id: '1', name: 'Test User', email: 'test@test.com', createdAt: new Date() };

      service.getCurrentUser().subscribe(user => {
        expect(user).toEqual(mockUser);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/me`);
      expect(req.request.method).toBe('GET');
      req.flush(mockUser);
    });
  });
});
