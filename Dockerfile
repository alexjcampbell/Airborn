FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 3000

ENV ASPNETCORE_URLS=http://+:3000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
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
COPY airborn.web/*.json /app/publish/
CMD ASPNETCORE_URLS=http://*:$PORT dotnet airborn.web.dll

