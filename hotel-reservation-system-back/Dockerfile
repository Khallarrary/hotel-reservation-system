FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["HotelApp.Api/HotelApp.Api.csproj", "HotelApp.Api/"]
COPY ["HotelApp.Application/HotelApp.Application.csproj", "HotelApp.Application/"]
COPY ["HotelApp.Domain/HotelApp.Domain.csproj", "HotelApp.Domain/"]
COPY ["HotelApp.Infrastructure/HotelApp.Infrastructure.csproj", "HotelApp.Infrastructure/"]

RUN dotnet restore "HotelApp.Api/HotelApp.Api.csproj"

COPY . .

RUN dotnet publish "HotelApp.Api/HotelApp.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "HotelApp.Api.dll"]

