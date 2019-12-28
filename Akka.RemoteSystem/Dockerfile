FROM mcr.microsoft.com/dotnet/core/runtime:3.0
COPY prod/ prod/
EXPOSE 8080
ENTRYPOINT ["dotnet", "prod/Akka.RemoteSystem.dll"]