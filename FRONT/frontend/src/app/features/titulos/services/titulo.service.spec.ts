import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TituloService } from './titulo.service';
import { environment } from '../../../../environments/environment';

describe('TituloService', () => {
  let service: TituloService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TituloService]
    });
    service = TestBed.inject(TituloService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('deve ser criado', () => {
    expect(service).toBeTruthy();
  });

  describe('Listar Títulos', () => {
    it('deve retornar lista de títulos', () => {
      const mockTitulos = [
        {
          id: '1',
          numeroTitulo: 'T-001',
          nomeDevedor: 'João Silva',
          cpfDevedor: '11144477735',
          quantidadeParcelas: 2,
          valorOriginal: 1000,
          valorAtualizado: 1100,
          diasAtraso: 30,
          criadoEm: '2026-05-12T10:00:00Z'
        }
      ];

      service.listar().subscribe(titulos => {
        expect(titulos).toEqual(mockTitulos);
        expect(titulos.length).toBe(1);
        expect(titulos[0].numeroTitulo).toBe('T-001');
        expect(titulos[0].nomeDevedor).toBe('João Silva');
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/titulos`);
      expect(req.request.method).toBe('GET');
      req.flush(mockTitulos);
    });

    it('deve tratar erro na listagem de títulos', () => {
      const errorMessage = 'Erro ao carregar títulos';

      service.listar().subscribe({
        next: () => expect.fail('deveria falhar'),
        error: (error: any) => {
          expect(error).toBeTruthy();
        }
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/titulos`);
      req.flush(errorMessage, { status: 500, statusText: 'Internal Server Error' });
    });
  });

  describe('Criar Título', () => {
    it('deve criar um novo título', () => {
      const mockRequest = {
        numeroTitulo: 'T-002',
        nomeDevedor: 'Maria Santos',
        cpfDevedor: '11144477735',
        percentualJuros: 1,
        percentualMulta: 10,
        parcelas: [
          {
            numero: 1,
            dataVencimento: '2026-06-12',
            valor: 1000
          }
        ]
      };

      const mockResponse = {
        id: '2',
        numeroTitulo: 'T-002',
        nomeDevedor: 'Maria Santos',
        cpfDevedor: '11144477735',
        quantidadeParcelas: 1,
        valorOriginal: 1000,
        valorAtualizado: 1000,
        diasAtraso: 0,
        criadoEm: '2026-05-12T10:00:00Z'
      };

      service.criar(mockRequest).subscribe(response => {
        expect(response).toEqual(mockResponse);
        expect(response.numeroTitulo).toBe('T-002');
        expect(response.nomeDevedor).toBe('Maria Santos');
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/titulos`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockRequest);
      req.flush(mockResponse);
    });

    it('deve tratar erro na criação de título', () => {
      const mockRequest = {
        numeroTitulo: 'T-002',
        nomeDevedor: 'Maria Santos',
        cpfDevedor: '11144477735',
        percentualJuros: 1,
        percentualMulta: 10,
        parcelas: [
          {
            numero: 1,
            dataVencimento: '2026-06-12',
            valor: 1000
          }
        ]
      };

      const errorMessage = 'CPF inválido';

      service.criar(mockRequest).subscribe({
        next: () => expect.fail('deveria falhar'),
        error: (error: any) => {
          expect(error).toBeTruthy();
        }
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/titulos`);
      req.flush(errorMessage, { status: 400, statusText: 'Bad Request' });
    });
  });
});
