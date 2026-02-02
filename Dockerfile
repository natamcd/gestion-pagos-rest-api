FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GestionPagos.Api/GestionPagos.Api.csproj", "GestionPagos.Api/"]
COPY ["GestionPagos.Core/GestionPagos.Core.csproj", "GestionPagos.Core/"]
COPY ["GestionPagos.Infrastructure/GestionPagos.Infrastructure.csproj", "GestionPagos.Infrastructure/"]
RUN dotnet restore "GestionPagos.Api/GestionPagos.Api.csproj"
COPY . .
WORKDIR "/src/GestionPagos.Api"
RUN dotnet build "GestionPagos.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GestionPagos.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GestionPagos.Api.dll"]
