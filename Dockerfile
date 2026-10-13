# Build : SDK .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore d'abord (couche mise en cache tant que le .csproj ne change pas)
COPY CasinoDeYann.csproj ./
RUN dotnet restore CasinoDeYann.csproj

COPY . .
RUN dotnet publish CasinoDeYann.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Exécution : runtime ASP.NET seul
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080 \
    TZ=Europe/Paris

# Dossier des clés Data Protection (monté en volume), accessible à l'utilisateur
# non-root "app" fourni par l'image.
RUN mkdir -p /keys && chown "$APP_UID" /keys

COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "CasinoDeYann.dll"]
