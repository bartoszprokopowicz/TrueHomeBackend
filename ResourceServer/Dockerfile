FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["ResourceServer/ResourceServer.csproj", "ResourceServer/"]
RUN dotnet restore "ResourceServer/ResourceServer.csproj"
COPY . .
WORKDIR "/src/ResourceServer"
RUN dotnet build "ResourceServer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ResourceServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ResourceServer.dll"]