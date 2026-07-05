FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /src
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_USE_POLLING_FILE_WATCHER=1 \
    DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER=1
EXPOSE 8080
CMD ["dotnet", "watch", "--project", "src/Fiap.TechChallenge.Api/Fiap.TechChallenge.Api.csproj", "run", "--no-launch-profile"]