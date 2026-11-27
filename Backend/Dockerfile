FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src

COPY TeamLink.slnx ./
COPY src/TeamLink.Api/TeamLink.Api.csproj src/TeamLink.Api/
RUN dotnet restore src/TeamLink.Api/TeamLink.Api.csproj

COPY . .
WORKDIR /src/src/TeamLink.Api
RUN dotnet publish TeamLink.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TeamLink.Api.dll"]