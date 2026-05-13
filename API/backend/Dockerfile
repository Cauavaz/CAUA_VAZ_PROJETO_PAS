FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia tudo sem exceções
COPY . .

# Restaura as dependências
RUN dotnet restore "src/Paschoalotto.API/Paschoalotto.API.csproj"

# Publica diretamente (evita problemas de build separado)
RUN dotnet publish "src/Paschoalotto.API/Paschoalotto.API.csproj" -c Release -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Paschoalotto.API.dll"]