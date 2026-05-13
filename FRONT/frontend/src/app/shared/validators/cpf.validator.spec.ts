import { FormControl } from '@angular/forms';
import { cpfValidator } from './cpf.validator';

describe('cpfValidator', () => {
  it('deve validar CPF formatado corretamente', () => {
    const validator = cpfValidator();
    const validControl = new FormControl('111.444.777-35');
    const result = validator(validControl);
    expect(result).toBeNull();
  });

  it('deve validar CPF sem formatação corretamente', () => {
    const validator = cpfValidator();
    const validControl = new FormControl('11144477735');
    const result = validator(validControl);
    expect(result).toBeNull();
  });

  it('deve invalidar CPF com dígitos verificadores incorretos', () => {
    const validator = cpfValidator();
    const invalidControl = new FormControl('111.444.777-00');
    const result = validator(invalidControl);
    expect(result).toEqual({ cpfInvalid: true });
  });

  it('deve invalidar CPF com todos os dígitos iguais', () => {
    const validator = cpfValidator();
    const invalidControl = new FormControl('111.111.111-11');
    const result = validator(invalidControl);
    expect(result).toEqual({ cpfInvalid: true });
  });

  it('deve invalidar CPF com menos de 11 dígitos', () => {
    const validator = cpfValidator();
    const invalidControl = new FormControl('123456789');
    const result = validator(invalidControl);
    expect(result).toEqual({ cpfInvalid: true });
  });

  it('deve invalidar CPF com mais de 11 dígitos', () => {
    const validator = cpfValidator();
    const invalidControl = new FormControl('123456789012');
    const result = validator(invalidControl);
    expect(result).toEqual({ cpfInvalid: true });
  });

  it('deve aceitar CPF vazio ou nulo', () => {
    const validator = cpfValidator();
    const emptyControl = new FormControl('');
    const nullControl = new FormControl(null);
    const undefinedControl = new FormControl(undefined);
    
    expect(validator(emptyControl)).toBeNull();
    expect(validator(nullControl)).toBeNull();
    expect(validator(undefinedControl)).toBeNull();
  });

  it('deve aceitar CPF com letras como vazio', () => {
    const validator = cpfValidator();
    const invalidControl = new FormControl('abc.def.ghi-jk');
    const result = validator(invalidControl);
    // CPF com apenas letras vira string vazia após sanitização
    expect(result).toBeNull();
  });

  it('deve validar CPF com caracteres especiais removendo-os', () => {
    const validator = cpfValidator();
    const invalidControl = new FormControl('111!444@777#35');
    const result = validator(invalidControl);
    // Remove caracteres especiais e valida: 11144477735
    expect(result).toBeNull();
  });

  it('deve validar CPF com diferentes formatações', () => {
    const validator = cpfValidator();
    const testCases = [
      '111.444.777-35',
      '11144477735',
      '111.444.77735',
      '111444777-35'
    ];

    testCases.forEach(cpf => {
      const control = new FormControl(cpf);
      const result = validator(control);
      expect(result).toBeNull();
    });
  });

  it('deve calcular dígitos verificadores corretamente', () => {
    const validator = cpfValidator();
    // Teste com CPF conhecido válido
    const validControl = new FormControl('123.456.789-09');
    const result = validator(validControl);
    expect(result).toBeNull();
  });

  it('deve remover caracteres não numéricos antes da validação', () => {
    const validator = cpfValidator();
    const control = new FormControl('111.444.777-35');
    const result = validator(control);
    expect(result).toBeNull();
  });
});
