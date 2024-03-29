FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

ARG TARGETOS
ARG TARGETARCH

ENV CONTAINER_IMAGE_BUILD=true

WORKDIR /app

COPY ./global.json ./
COPY ./MuzakBot.sln ./
COPY ./Directory.Build.props ./
COPY ./Directory.Packages.props ./
COPY ./nuget.config ./
COPY ./src ./src

RUN dotnet publish --configuration "Release" --os "${TARGETOS}" --arch "${TARGETARCH}"

RUN rm -rf /app/artifacts/publish/App/release/*.pdb; \
    rm -rf /app/artifacts/publish/App/release/*.dbg

FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/runtime:8.0

ARG TARGETOS
ARG TARGETARCH

COPY --from=build-env /app/artifacts/publish/App/release /app

WORKDIR /app
ENTRYPOINT ["dotnet", "MuzakBot.dll"]