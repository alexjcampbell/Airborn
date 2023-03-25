FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["airborn.web/airborn.web.csproj", "airborn.web/"]
RUN dotnet restore "airborn.web/airborn.web.csproj"
COPY . .
WORKDIR "/src/airborn.web"
RUN dotnet build "airborn.web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "airborn.web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["airborn.web/Data/SR22_G2_2900.json", "/app/publish"]
COPY ["airborn.web/Data/SR22_G2_3400.json", "/app/publish"]
COPY ["airborn.web/Data/SR22T_G5_2900.json", "/app/publish"]
COPY ["airborn.web/Data/SR22T_G5_3600.json", "/app/publish"]
COPY ["airborn.web/Data/C172_SP_2200.json", "/app/publish"]
COPY ["airborn.web/Data/C172_SP_2550.json", "/app/publish"]
COPY ["airborn.web/airborn.db", "/app"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet airborn.web.dll

