using FluentAssertions;
using Paschoalotto.Application.Validation;
using Xunit;

namespace Paschoalotto.Tests.Domain.Tests.Validation;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("111.444.777-35")] // CPF válido calculado
    [InlineData("11144477735")] // Mesmo CPF sem formatação
    [InlineData("111.444.77735")] // Formatação parcial
    [InlineData("111444777-35")] // Formatação alternativa
    public void IsValid_CpfValido_DeveRetornarTrue(string cpf)
    {
        // Act
        var resultado = CpfValidator.IsValid(cpf);

        // Assert
        resultado.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123")]
    [InlineData("1234567890")] // 10 dígitos
    [InlineData("123456789012")] // 12 dígitos
    [InlineData("111.111.111-11")] // todos números iguais
    [InlineData("222.222.222-22")] // todos números iguais
    [InlineData("000.000.000-00")] // todos zeros
    [InlineData("123.456.789-00")] // dígitos verificadores incorretos
    [InlineData("abc.def.ghi-jk")] // letras
    public void IsValid_CpfInvalido_DeveRetornarFalse(string cpf)
    {
        // Act
        var resultado = CpfValidator.IsValid(cpf);

        // Assert
        resultado.Should().BeFalse();
    }

    [Theory]
    [InlineData("111.444.777-35", "11144477735")]
    [InlineData("111.444.77735", "11144477735")]
    [InlineData("111444777-35", "11144477735")]
    [InlineData("11144477735", "11144477735")]
    [InlineData("111.444.777-35 ", "11144477735")]
    public void Sanitize_DeveRemoverCaracteresNaoNumericos(string cpfOriginal, string cpfEsperado)
    {
        // Act
        var resultado = CpfValidator.Sanitize(cpfOriginal);

        // Assert
        resultado.Should().Be(cpfEsperado);
    }

    [Fact]
    public void IsValid_CpfComEspacos_DeveRetornarTrue()
    {
        // Arrange
        var cpfComEspacos = " 111.444.777-35 ";

        // Act
        var resultado = CpfValidator.IsValid(cpfComEspacos);

        // Assert
        resultado.Should().BeTrue(); // Validador remove espaços automaticamente
    }

    [Theory]
    [InlineData("111.444.777-35")] // CPF válido calculado
    [InlineData("123.456.789-09")] // CPF válido calculado
    [InlineData("987.654.321-00")] // CPF válido calculado
    public void IsValid_CpfsValidosReais_DeveRetornarTrue(string cpf)
    {
        // Act
        var resultado = CpfValidator.IsValid(cpf);

        // Assert
        resultado.Should().BeTrue();
    }
}
