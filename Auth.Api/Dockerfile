FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Instalar libicu-dev
RUN apk add --no-cache icu-dev libc6-compat

# Copia los archivos del proyecto y restaura las dependencias
COPY ./Auth ./
RUN dotnet restore "Auth.Api/Auth.Api.csproj"

# Compila y publica la aplicación en una sola etapa
RUN dotnet publish "Auth.Api/Auth.Api.csproj" -c Release -o /app/publish

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 7130
# Copia los archivos publicados desde la etapa de construcción
COPY --from=build /app/publish .

# Configura las variables de entorno necesarias
ENV ASPNETCORE_URLS=http://+:7130 \
    ASPNETCORE_ENVIRONMENT=Production

# Configura el punto de entrada de la aplicación
ENTRYPOINT ["dotnet", "Auth.Api.dll"]
