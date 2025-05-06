# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante dos arquivos e publica o app
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .

# Porta exposta (Render escuta na 10000, mas mapeia automaticamente)
EXPOSE 80

# Comando de inicialização
ENTRYPOINT ["dotnet", "NewRepository.dll"]
